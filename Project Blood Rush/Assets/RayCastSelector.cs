using UnityEngine;

public class RaycastSelector : MonoBehaviour
{
    public Camera playerCamera;      // Assign your player camera here
    public float maxDistance = 10f;  // How far the ray reaches
    public LayerMask interactableLayer; // Only hit objects on these layers

    private SelectableObject currentlyHighlighted;

    void Update()
    {
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
        RaycastHit hit;

        SelectableObject hitObject = null;

        // Raycast to detect interactable object
        if (Physics.Raycast(ray, out hit, maxDistance, interactableLayer))
        {
            hitObject = hit.collider.GetComponent<SelectableObject>();
        }

        // If the highlighted object changed
        if (hitObject != currentlyHighlighted)
        {
            // Unhighlight previous object
            if (currentlyHighlighted != null)
                currentlyHighlighted.OnUnhover();

            // Highlight new object
            if (hitObject != null)
                hitObject.OnHover();

            currentlyHighlighted = hitObject;
        }

        // Interact with currently highlighted object
        if (currentlyHighlighted != null && Input.GetKeyDown(KeyCode.E))
        {
            currentlyHighlighted.OnInteract();
        }
    }
}