using UnityEngine;

public class BarTrigger : MonoBehaviour
{
    private PlayerMovement playerInRange;
    private InputHandler inputHandler;

    [Header("NPC Dialogue")]
    public string npcDialogueTag = "WEREWOLF";

    private BarMat barMat;

    private void Start()
    {
        inputHandler = FindObjectOfType<InputHandler>();
        barMat = GetComponent<BarMat>();
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerMovement player = other.GetComponent<PlayerMovement>();
        if (player != null)
        {
            playerInRange = player;
            Debug.Log("Press E to enter bar");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerMovement player = other.GetComponent<PlayerMovement>();
        if (player != null && player == playerInRange)
        {
            playerInRange = null;
        }
    }

    private void Update()
    {
        if (playerInRange != null &&
            inputHandler != null &&
            inputHandler.GetInteractPressed())
        {
            playerInRange.EnterBarState(barMat);

            DialogueUiManager ui = FindObjectOfType<DialogueUiManager>();

            if (ui != null)
            {
                TextAsset dialogue = Resources.Load<TextAsset>(npcDialogueTag);

                if (dialogue != null)
                    ui.StartDialogue(dialogue);
                else
                    Debug.LogError("Dialogue file not found in Resources!");
            }
        }
    }
}