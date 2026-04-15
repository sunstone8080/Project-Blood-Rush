using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueUiManager : MonoBehaviour
{
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

    private string goodReaction = "";
    private string badReaction = "";
    private string wrongReaction = "";

    void Start()
    {
        _inputHandler = FindObjectOfType<InputHandler>();
        dialogueBoxPlaceholder.SetActive(false);

        if (CocktailSystem.Instance != null)
        {
            CocktailSystem.Instance.OnDrinkSubmitted += OnDrinkSubmitted;
        }
    }

    void Update()
    {
        if (!_isDialogueActive) return;

        Debug.Log("Dialogue Active + waiting input");

        if (_inputHandler != null && Input.GetMouseButtonDown(0))
        {
            Debug.Log("E PRESSED - advancing dialogue");

            if (waitingForDrink)
            {
                Debug.Log("Blocked: waiting for drink");
                return;
            }

            DisplayNextLine();
        }
    }

    // ===== START =====
    public void StartDialogue(TextAsset dialogue)
    {
        _isDialogueActive = true;
        dialogueBoxPlaceholder.SetActive(true);

        names.Clear();
        sentences.Clear();

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
                    goodReaction = line.Replace("Good:", "").Trim();
                else if (line.StartsWith("Bad:"))
                    badReaction = line.Replace("Bad:", "").Trim();
                else if (line.StartsWith("Wrong:"))
                    wrongReaction = line.Replace("Wrong:", "").Trim();

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

    // ===== NEXT LINE =====
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

            nameText.text = "Grumpy Werewolf";
            currentSentence = "Bring me a " + requestedDrink;

            typingCoroutine = StartCoroutine(TypeSentence(currentSentence));
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

    // ===== DRINK RESULT =====
    void OnDrinkSubmitted(string drinkName)
    {
        if (!_isDialogueActive || !waitingForDrink) return;

        string normalizedInput = Normalize(drinkName);
        string normalizedTarget = Normalize(requestedDrink);

        waitingForDrink = false;

        if (normalizedInput == normalizedTarget)
        {
            ShowReaction(goodReaction);
        }
        else if (drinkName == "Unknown Drink")
        {
            ShowReaction(badReaction);
        }
        else
        {
            ShowReaction(wrongReaction);
        }
    }

    void ShowReaction(string reaction)
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        isTyping = false;

        nameText.text = "Grumpy Werewolf";
        dialogueText.text = reaction;
    }

    // ===== END =====
    public void EndDialogue()
    {
        _isDialogueActive = false;
        waitingForDrink = false;
        dialogueBoxPlaceholder.SetActive(false);
    }

    // ===== HELPERS =====
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