using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObjectSelection : MonoBehaviour
{
    public Camera playerCamera;      // Assign your player camera here
    public float maxDistance = 10f;  // How far the ray reaches
    public LayerMask interactableLayer; // Optional: only hit objects on certain layers

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

            // Optional: highlight the object
            SelectableObject selectable = hitObject.GetComponent<SelectableObject>();
            if (selectable != null)
            {
                selectable.OnHover(); // call hover function if it exists
            }

            // Check for player interaction input
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (selectable != null)
                    selectable.OnInteract(); // call interaction
            }
        }
    }
}
