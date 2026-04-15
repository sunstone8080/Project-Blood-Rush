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
    private bool _skipNextClick = false;

    private InputHandler _inputHandler;

    void Start()
    {
        _inputHandler = GameObject.Find("GameManager").GetComponent<InputHandler>();
        dialogueBoxPlaceholder.SetActive(false);
    }

    void Update()
    {
        if (_isDialogueActive && _inputHandler.GetLeftMouseDown())
        {
            if (_skipNextClick)
            {
                _skipNextClick = false;
                return;
            }

            DisplayNextLine();
        }
    }

    // ===== PUBLIC API =====
    public void StartDialogue(TextAsset dialogue)
    {
        if (_isDialogueActive) return;

        _isDialogueActive = true;
        _skipNextClick = true;
        dialogueBoxPlaceholder.SetActive(true);

        names.Clear();
        sentences.Clear();

        string[] lines = dialogue.text.Split('\n');

        foreach (string line in lines)
        {
            if (line.Contains("Drink Order") || line.Contains("Reactions"))
                break;

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
            // finish instantly
            StopCoroutine(typingCoroutine);
            dialogueText.text = currentSentence;
            isTyping = false;
            return;
        }

        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        nameText.text = names.Dequeue();
        currentSentence = sentences.Dequeue();

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

    public void EndDialogue()
    {
        _isDialogueActive = false;
        dialogueBoxPlaceholder.SetActive(false);
    }

    // ===== HELPERS FOR PLAYER =====
    public bool IsTyping()
    {
        return isTyping;
    }

    public void Skip()
    {
        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
            dialogueText.text = currentSentence;
            isTyping = false;
        }
    }
}