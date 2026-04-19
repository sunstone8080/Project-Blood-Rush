using UnityEngine;

public class RaycastSelector : MonoBehaviour
{
    public Camera playerCamera;
    public float maxDistance = 10f;
    public LayerMask interactableLayer;

    private SelectableObject currentlyHighlighted;

    private InteractionUIState uiState = InteractionUIState.None;

    private enum InteractionUIState
    {
        None,
        Hovering
    }

    void Update()
    {
        Ray ray = playerCamera.ScreenPointToRay(
            new Vector3(Screen.width / 2f, Screen.height / 2f, 0));

        RaycastHit hit;
        SelectableObject hitObject = null;

        if (Physics.Raycast(ray, out hit, maxDistance, interactableLayer))
        {
            hitObject = hit.collider.GetComponent<SelectableObject>();
        }

        // =========================
        // HANDLE HOVER VISUALS
        // =========================
        if (hitObject != currentlyHighlighted)
        {
            if (currentlyHighlighted != null)
                currentlyHighlighted.OnUnhover();

            if (hitObject != null)
                hitObject.OnHover();

            currentlyHighlighted = hitObject;
        }

        // =========================
        // INTERACTION INPUT
        // =========================
        if (currentlyHighlighted != null && Input.GetKeyDown(KeyCode.E))
        {
            currentlyHighlighted.OnInteract();
        }

        // =========================
        // UI PROMPT LOGIC (FIXED)
        // =========================

        // Only show hover prompt if something is actually being hit
        if (hitObject != null)
        {
            Debug.Log("1");
            if (uiState != InteractionUIState.Hovering)
            {
                Debug.Log("2");
                GameManager.Instance.SetPrompt("Click to pick up", 1);
                uiState = InteractionUIState.Hovering;
            }
        }
        else
        {
            Debug.Log("3");
            if (uiState != InteractionUIState.None)
            {
                Debug.Log("4");
                GameManager.Instance.ClearPrompt();
                uiState = InteractionUIState.None;
            }
        }
    }
}