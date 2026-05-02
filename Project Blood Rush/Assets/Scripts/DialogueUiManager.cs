using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueUiManager : MonoBehaviour
{
    private PlayerMovement playerMovement;
    public TMP_Text nameText;
    public TMP_Text dialogueText;
    public GameObject dialogueBoxPlaceholder;

    private Queue<string> names = new Queue<string>();
    private Queue<string> sentences = new Queue<string>();

    [SerializeField] private float typingSpeed = 0.02f;

    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private string currentSentence;

    private bool _isDialogueActive = false;

    private InputHandler _inputHandler;

    // ===== DRINK SYSTEM =====
    private bool waitingForDrink = false;
    private string requestedDrink = "";

    private List<string> goodReactions = new List<string>();
    private List<string> badReactions = new List<string>();
    private List<string> wrongReactions = new List<string>();

    public bool IsWaitingForDrink() => waitingForDrink;
    public bool IsDialogueActive() => _isDialogueActive;

    void Start()
    {
        _inputHandler = FindObjectOfType<InputHandler>();
        dialogueBoxPlaceholder.SetActive(false);

        playerMovement = FindObjectOfType<PlayerMovement>();

        if (CocktailSystem.Instance != null)
        {
            CocktailSystem.Instance.OnDrinkSubmitted += OnDrinkSubmitted;
        }
    }

    void Update()
    {
        if (!_isDialogueActive) return;

        if (_inputHandler != null && Input.GetMouseButtonDown(0))
        {
            if (waitingForDrink)
                return;

            DisplayNextLine();
        }
    }

    public void StartDialogue(TextAsset dialogue)
    {
        _isDialogueActive = true;
        dialogueBoxPlaceholder.SetActive(true);

        names.Clear();
        sentences.Clear();
        goodReactions.Clear();
        badReactions.Clear();
        wrongReactions.Clear();

        string[] lines = dialogue.text.Split('\n');

        bool inReactions = false;

        foreach (string raw in lines)
        {
            string line = raw.Trim();
            if (string.IsNullOrEmpty(line)) continue;

            if (line.StartsWith("Drink Order:"))
            {
                requestedDrink = line.Replace("Drink Order:", "").Trim();

                names.Enqueue("SYSTEM");
                sentences.Enqueue("[REQUEST_TRIGGER]");
                continue;
            }

            if (line.StartsWith("Reactions"))
            {
                inReactions = true;
                continue;
            }

            if (inReactions)
            {
                if (line.StartsWith("Good:"))
                    goodReactions.Add(line.Replace("Good:", "").Trim());
                else if (line.StartsWith("Bad:"))
                    badReactions.Add(line.Replace("Bad:", "").Trim());
                else if (line.StartsWith("Wrong:"))
                    wrongReactions.Add(line.Replace("Wrong:", "").Trim());
                continue;
            }

            string[] parts = line.Split(':');

            if (parts.Length == 2)
            {
                names.Enqueue(parts[0].Trim());
                sentences.Enqueue(parts[1].Trim());
            }
        }

        DisplayNextLine();
    }

    public void DisplayNextLine()
    {
        if (isTyping)
        {
            Skip();
            return;
        }

        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        nameText.text = names.Dequeue();
        currentSentence = sentences.Dequeue();

        if (currentSentence == "[REQUEST_TRIGGER]")
        {
            waitingForDrink = true;

            
            DisplayNextLine(); 
            return;
        }

        typingCoroutine = StartCoroutine(TypeSentence(currentSentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char letter in sentence)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

  
    void OnDrinkSubmitted(string drinkName)
    {
        if (!_isDialogueActive || !waitingForDrink) return;

        waitingForDrink = false;

        string normalizedInput = Normalize(drinkName);
        string normalizedTarget = Normalize(requestedDrink);

        if (normalizedInput == normalizedTarget)
        {
            ShowReaction(goodReactions);
        }
        else if (drinkName == "Unknown Drink")
        {
            ShowReaction(badReactions);
        }
        else
        {
            ShowReaction(wrongReactions);
        }
    }

    void ShowReaction(List<string> reactionLines)
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        isTyping = false;

        names.Clear();
        sentences.Clear();

        foreach (string line in reactionLines)
        {
            string[] parts = line.Split(':');
            if (parts.Length == 2)
            {
                names.Enqueue(parts[0].Trim());
                sentences.Enqueue(parts[1].Trim());
            }
            else
            {
                // fallback
                names.Enqueue("???");
                sentences.Enqueue(line.Trim());
            }
        }

        DisplayNextLine();
    }

    public void EndDialogue()
    {
        _isDialogueActive = false;
        waitingForDrink = false;
        dialogueBoxPlaceholder.SetActive(false);

       
        if (playerMovement != null)
        {
            playerMovement.ExitBarState();
        }
    }

    public bool IsTyping() => isTyping;

    public void Skip()
    {
        if (isTyping)
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                typingCoroutine = null;
            }

            dialogueText.text = currentSentence;
            isTyping = false;
        }
    }

    private string Normalize(string s)
    {
        return s.Replace(" ", "").ToLower();
    }
}