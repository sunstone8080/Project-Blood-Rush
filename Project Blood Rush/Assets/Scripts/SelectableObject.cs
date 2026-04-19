using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableObject : MonoBehaviour
{
    private Renderer objRenderer;
    private Color originalColor;
    public static bool HasHeldObject => s_heldInstance != null;
    [Header("Held Item")]
    [SerializeField] private GameObject heldPrefab;

    public Vector3 holdViewportPosition = new Vector3(0.85f, -0.35f, 1f);

    private static GameObject s_heldInstance;
    private static GameObject s_heldSource;

    public static Ingredient HeldIngredient
    {
        get
        {
            if (s_heldInstance != null)
                return s_heldInstance.GetComponent<Ingredient>();
            return null;
        }
    }

    private void Awake()
    {
        objRenderer = GetComponent<Renderer>();
        if (objRenderer != null)
            originalColor = objRenderer.material.color;
    }

    public void OnHover()
    {
        if (objRenderer != null)
            objRenderer.material.color = Color.yellow;
    }

    public void OnUnhover()
    {
        if (objRenderer != null)
            objRenderer.material.color = originalColor;
    }

    public void OnInteract()
    {
        OnInteract(Camera.main);
    }

    public void OnInteract(Camera playerCamera)
    {
        if (playerCamera == null)
            playerCamera = Camera.main;
        if (playerCamera == null) return;

        // Drop if same object
        if (s_heldSource == this.gameObject && s_heldInstance != null)
        {
            ClearHeldItem();
            return;
        }

        ClearHeldItem();

        GameObject clone = Instantiate(heldPrefab != null ? heldPrefab : gameObject);

        CleanupClone(clone);

        clone.transform.SetParent(playerCamera.transform);
        clone.transform.localPosition = holdViewportPosition;
        clone.transform.localRotation = Quaternion.identity;

        SetLayerRecursive(clone, 2);

        s_heldInstance = clone;
        s_heldSource = this.gameObject;
    }

    private static void CleanupClone(GameObject clone)
    {
        var selectableOnClone = clone.GetComponentInChildren<SelectableObject>();
        if (selectableOnClone != null)
            Destroy(selectableOnClone);

        foreach (var rb in clone.GetComponentsInChildren<Rigidbody>())
            Destroy(rb);

        foreach (var col in clone.GetComponentsInChildren<Collider>())
            col.enabled = false;

        foreach (var b in clone.GetComponentsInChildren<MonoBehaviour>())
            b.enabled = false;
    }

    private static void SetLayerRecursive(GameObject go, int layer)
    {
        if (go == null) return;
        go.layer = layer;

        foreach (Transform child in go.transform)
            SetLayerRecursive(child.gameObject, layer);
    }

   
    public static void ClearHeldItem()
    {
        if (s_heldInstance != null)
        {
            Destroy(s_heldInstance);
            s_heldInstance = null;
            s_heldSource = null;
        }
    }

    public static void ReplaceHeldItem(GameObject newPrefab, Vector3 holdPos)
    {
        Camera cam = Camera.main;
        if (cam == null || newPrefab == null) return;

        ClearHeldItem();

        GameObject clone = Instantiate(newPrefab);

        CleanupClone(clone);

        clone.transform.SetParent(cam.transform);
        clone.transform.localPosition = holdPos;
        clone.transform.localRotation = Quaternion.identity;

        SetLayerRecursive(clone, 2);

        s_heldInstance = clone;
    }
}