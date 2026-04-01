using UnityEngine;

public class BarTrigger : MonoBehaviour
{
    private PlayerMovement playerInRange;
    private InputHandler inputHandler;

    [Header("NPC Dialogue")]
    public string npcDialogueTag = "WEREWOLF"; // Set in inspector for each NPC

    private void Start()
    {
        inputHandler = FindObjectOfType<InputHandler>();
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
        if (playerInRange == null) return;
        if (GameManager.Instance.CurrentGameState != GameManager.GameState.FreeRoam) return;

        if (inputHandler != null && inputHandler.GetInteractPressed())
        {
            playerInRange.EnterBarState();
            // Start NPC dialogue
         //   DialogueManager.Instance.StartDialogue(npcDialogueTag + "_Start");
        }
    }
}