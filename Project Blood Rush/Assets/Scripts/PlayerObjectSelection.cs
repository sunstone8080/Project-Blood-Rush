using UnityEngine;

public class PlayerObjectSelection : MonoBehaviour
{
    public Camera playerCamera;
    public float maxDistance = 2.2f;
    public LayerMask interactableLayer;

    private InputHandler m_InputHandler;
    private MiniGameManager miniGameManager;
    private SelectableObject lastSelectable;

    void Start()
    {
        m_InputHandler = FindObjectOfType<InputHandler>();
        miniGameManager = FindObjectOfType<MiniGameManager>();
    }

    void Update()
    {
        if (miniGameManager == null || playerCamera == null) return;

        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, interactableLayer))
        {
            SelectableObject selectable = hit.collider.GetComponent<SelectableObject>();

            if (selectable != null)
            {
                if (selectable != lastSelectable)
                {
                    if (lastSelectable != null) lastSelectable.OnUnhover();
                    selectable.OnHover();
                    lastSelectable = selectable;
                }

                // Start collection mini-game when Mouse0 is pressed
                if (m_InputHandler.GetLeftMouseDown())
                {
                    miniGameManager.StartMiniGameForObject(selectable);
                }
            }
            else if (lastSelectable != null)
            {
                lastSelectable.OnUnhover();
                lastSelectable = null;
            }
        }
        else if (lastSelectable != null)
        {
            lastSelectable.OnUnhover();
            lastSelectable = null;
        }
    }
}