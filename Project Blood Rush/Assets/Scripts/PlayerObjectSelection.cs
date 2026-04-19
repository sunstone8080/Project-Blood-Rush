using UnityEngine;

public class PlayerObjectSelection : MonoBehaviour
{
    public Camera playerCamera;
    public float maxDistance = 2.2f;
    public LayerMask interactableLayer;

    private InputHandler m_InputHandler;
    private MiniGameManager miniGameManager;
    private SelectableObject lastSelectable;

    private bool isHovering = false;

    void Start()
    {
        m_InputHandler = FindObjectOfType<InputHandler>();
        miniGameManager = FindObjectOfType<MiniGameManager>();
    }

    void Update()
    {
        if (miniGameManager == null || playerCamera == null) return;

        Ray ray = playerCamera.ScreenPointToRay(
            new Vector3(Screen.width / 2f, Screen.height / 2f, 0));

        bool hitSomething = Physics.Raycast(
            ray,
            out RaycastHit hit,
            maxDistance,
            interactableLayer
        );

        SelectableObject selectable = null;

        if (hitSomething)
        {
            selectable = hit.collider.GetComponent<SelectableObject>();
        }

        // =========================
        // HOVER VISUALS
        // =========================
        if (selectable != lastSelectable)
        {
            if (lastSelectable != null)
                lastSelectable.OnUnhover();

            if (selectable != null)
                selectable.OnHover();

            lastSelectable = selectable;
        }

        // =========================
        // UI PROMPTS (UPDATED)
        // =========================
        if (selectable != null)
        {
            Ingredient ingredient = selectable.GetComponent<Ingredient>();

            if (!isHovering)
            {
                if (ingredient != null && ingredient.isBaseIngredient)
                {
                    GameManager.Instance.SetPrompt("Key ingredient - Click to pick up", 1);
                }
                else
                {
                    GameManager.Instance.SetPrompt("Click to pick up", 1);
                }

                isHovering = true;
            }
        }
        else
        {
            if (isHovering)
            {
                GameManager.Instance.ClearPrompt();
                isHovering = false;
            }
        }

        // =========================
        // MINI GAME INPUT
        // =========================
        if (selectable != null && m_InputHandler != null)
        {
            if (m_InputHandler.GetLeftMouseDown())
            {
                miniGameManager.StartMiniGameForObject(selectable);
            }
        }
    }
}