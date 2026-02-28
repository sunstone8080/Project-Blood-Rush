using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObjectSelection : MonoBehaviour
{
    public Camera playerCamera;      // Assign your player camera here
    public float maxDistance = 10f;  // How far the ray reaches
    public LayerMask interactableLayer; // Optional: only hit objects on certain layers

    // track the last selectable the player was looking at so we can call OnUnhover
    private SelectableObject lastSelectable;

    private void Update()
    {
        RaycastHit hit;

        // 1. Create a ray from the center of the screen
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));

        // 2. Cast the ray
        if (Physics.Raycast(ray, out hit, maxDistance, interactableLayer))
        {
            // 3. We hit something!
            GameObject hitObject = hit.collider.gameObject;
            Debug.Log("Looking at: " + hitObject.name);

            // Optional: highlight the object and manage unhovering
            SelectableObject selectable = hitObject.GetComponent<SelectableObject>();
            if (selectable != null)
            {
                if (selectable != lastSelectable)
                {
                    if (lastSelectable != null)
                        lastSelectable.OnUnhover();
                    selectable.OnHover();
                    lastSelectable = selectable;
                }
            }
            else
            {
                // hit something not selectable -> clear last selectable
                if (lastSelectable != null)
                {
                    lastSelectable.OnUnhover();
                    lastSelectable = null;
                }
            }

            // Check for player interaction input
            if (Input.GetKeyDown(KeyCode.Mouse0)) // this is left mouse button, can be changed later
            {
                if (selectable != null)
                {
                    // Call object-level interaction and let the selectable handle  creating the held instance
                    selectable.OnInteract(playerCamera);
                }
            }
        }
        else
        {
            // Raycast didn't hit anything: ensure previously hovered object is unhovered
            if (lastSelectable != null)
            {
                lastSelectable.OnUnhover();
                lastSelectable = null;
            }
        }
    }
}
