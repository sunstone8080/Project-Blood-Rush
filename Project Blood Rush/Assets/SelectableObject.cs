using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableObject : MonoBehaviour
{
    private Renderer objRenderer;
    private Color originalColor;

    [Tooltip("Viewport X (0..1), Y (0..1) and Z (distance in world units) where the held item will appear.")]
    public Vector3 holdViewportPosition = new Vector3(0.85f, 0.15f, 1f);

    // single held instance tracked globally so picking a new item drops the previous
    private static GameObject s_heldInstance;
    // the source object (original in the world) that corresponds to the held instance
    private static GameObject s_heldSource;

    private void Awake()
    {
        objRenderer = GetComponent<Renderer>();
        if (objRenderer != null)
            originalColor = objRenderer.material.color;
    }

    // Called when player looks at object
    public void OnHover()
    {
        if (objRenderer != null)
            objRenderer.material.color = Color.yellow;
    }

    // Called when player looks away
    public void OnUnhover()
    {
        if (objRenderer != null)
            objRenderer.material.color = originalColor;
    }

    // general OnInteract() - convenience when camera isn't passed (falls back to Camera.main)
    public void OnInteract()
    {
        OnInteract(Camera.main);
    }

    // Called when player presses interact, an overload that passes camera to parent object to
    // Pass the player's camera (or it will fall back to Camera.main).
    public void OnInteract(Camera playerCamera)
    {
        Debug.Log("Interacted with " + gameObject.name);

        if (playerCamera == null)
            playerCamera = Camera.main;
        if (playerCamera == null)
        {
            Debug.LogWarning("SelectableObject.OnInteract: no camera available to parent held object to.");
            return;
        }

        // If the player is already holding this same source object, drop it.
        if (s_heldSource == this.gameObject && s_heldInstance != null)
        {
            Debug.Log("Dropping held object: " + this.gameObject.name);
            Destroy(s_heldInstance);
            s_heldInstance = null;
            s_heldSource = null;
            // restore source color when dropped
            if (objRenderer != null)
                objRenderer.material.color = originalColor;
            return;
        }

        // If there's already a different held object, remove it
        if (s_heldInstance != null)
        {
            Destroy(s_heldInstance);
            s_heldInstance = null;
            s_heldSource = null;
        }

        // Instantiate a copy of this object
        GameObject clone = Instantiate(gameObject);

        // Remove SelectableObject so held copy doesn't respond to raycasts / hover
        var selectableOnClone = clone.GetComponentInChildren<SelectableObject>();
        if (selectableOnClone != null)
            Destroy(selectableOnClone);

        // Remove Rigidbodies on the clone so physics won't apply
        var rbs = clone.GetComponentsInChildren<Rigidbody>();
        foreach (var rb in rbs)
            Destroy(rb);

        // Disable colliders so the held object doesn't collide with world or block raycasts
        var cols = clone.GetComponentsInChildren<Collider>();
        foreach (var c in cols)
            c.enabled = false;

        // Disable other MonoBehaviour scripts on the clone to avoid unwanted behavior on held copy
        var behaviours = clone.GetComponentsInChildren<MonoBehaviour>();
        foreach (var b in behaviours)
        {
            // We already removed SelectableObject above; disable remaining scripts
            b.enabled = false;
        }

        // Ensure the clone uses the original color (in case the source was hovered/colored yellow)
        var cloneRenderers = clone.GetComponentsInChildren<Renderer>();
        foreach (var r in cloneRenderers)
        {
            if (r != null)
                r.material.color = originalColor;
        }

        // Parent to the camera so it "sticks" to the view
        Vector3 worldPos = playerCamera.ViewportToWorldPoint(holdViewportPosition);
        clone.transform.SetParent(playerCamera.transform, worldPositionStays: true);
        clone.transform.localPosition = playerCamera.transform.InverseTransformPoint(worldPos);
        clone.transform.localRotation = Quaternion.identity;

        // Set clone and children to IgnoreRaycast layer so it won't be hit by subsequent raycasts (default index = 2)
        TrySetLayerRecursive(clone, 2);

        // restore source color after pick-up so the world object isn't left yellow
        if (objRenderer != null)
            objRenderer.material.color = originalColor;

        s_heldInstance = clone;
        s_heldSource = this.gameObject;
    }

    private void TrySetLayerRecursive(GameObject go, int layer)
    {
        if (go == null) return;
        go.layer = layer;
        foreach (Transform child in go.transform)
            TrySetLayerRecursive(child.gameObject, layer);
    }
}
