using UnityEngine;

public class BarTrigger : MonoBehaviour
{
    private PlayerMovement playerInRange;
    private InputHandler inputHandler;

    [Header("NPC Dialogue")]
    public string npcDialogueTag = "WEREWOLF";
    public string monsterHunterTag = "MonsterHunter";

    [Header("Monster Hunter Settings")]
    public bool isMonsterHunterNPC = false;

    private BarMat barMat;
    private DialogueUiManager dialogueUI;

    private int drinkSubmissionCount = 0;

    private void Start()
    {
        inputHandler = FindObjectOfType<InputHandler>();
        barMat = GetComponent<BarMat>();
        dialogueUI = FindObjectOfType<DialogueUiManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerMovement player = other.GetComponent<PlayerMovement>();
        if (player != null)
        {
            playerInRange = player;

            GameManager.Instance.SetPrompt("Press E to talk", 2);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerMovement player = other.GetComponent<PlayerMovement>();
        if (player != null && player == playerInRange)
        {
            playerInRange = null;
            GameManager.Instance.ClearPrompt();
        }
    }

    private void Update()
    {
        if (playerInRange == null || inputHandler == null)
            return;

        if (!inputHandler.GetInteractPressed())
            return;

        if (dialogueUI == null)
            return;

        if (dialogueUI.IsDialogueActive() && !dialogueUI.IsWaitingForDrink())
        {
            Debug.Log("Dialogue running ignoring input");
            return;
        }

        if (isMonsterHunterNPC && drinkSubmissionCount < 3)
        {
            GameManager.Instance.SetPrompt("Serve 3 drinks first", 3);
            Debug.Log("MonsterHunter locked need 3 drinks first");
            return;
        }

        playerInRange.EnterBarState(barMat);

        GameManager.Instance.SetPrompt("Press Q to leave", 4);

        if (dialogueUI.IsWaitingForDrink() && SelectableObject.HasHeldObject)
        {
            Debug.Log("Submitting drink");

            CocktailSystem.Instance.SubmitDrink();

            drinkSubmissionCount++;

            return;
        }

        string tagToLoad = npcDialogueTag;

        if (isMonsterHunterNPC)
            tagToLoad = monsterHunterTag;

        TextAsset dialogue = Resources.Load<TextAsset>(tagToLoad);

        if (dialogue != null)
            dialogueUI.StartDialogue(dialogue);
        else
            Debug.LogError("Dialogue file not found");
    }
}