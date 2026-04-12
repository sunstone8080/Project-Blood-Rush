using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueUiManager : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text dialogueText;
    public Camera playerCamera;
    //public LayerMask interactableLayer;
    public GameObject dialogueBoxPlaceholder;

    //// temp list of dialogue
    //private List<string> _dialogueLines;
    private Queue<string> names = new Queue<string>();
    private Queue<string> sentences = new Queue<string>();

    //private GameObject _dialogueBoxPlaceholder;
    private bool _isDialogueActive = false;
    private bool _skipNextClick = false;
    private InputHandler _inputHandler;

    //public void displayMessage(List<string> dialogueLines)
    //{
    //    // for now just display the first line of dialogue
    //    _dialogueBoxPlaceholder.SetActive(true);
    //    dialogueText.text = dialogueLines[0];
    //    nameText.text = "Vampire";

    //    // add functionality to cycle through dialogue lines on click, target sprites
    //}


    public void StartDialogue(TextAsset dialogue)
    {
        if(_isDialogueActive) return;

        _isDialogueActive = true;
        _skipNextClick = true;
        dialogueBoxPlaceholder.SetActive(true);

        names.Clear();
        sentences.Clear();

        string[] lines = dialogue.text.Split('\n');
        foreach (string line in lines)
        {
            string[] parts = line.Split(':');

            if (line.Contains("Drink Order") || line.Contains("Reactions")) break;
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
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }
        string currentName = names.Dequeue();
        string currentSentence = sentences.Dequeue();
        nameText.text = currentName;
        dialogueText.text = currentSentence;
    }

    public void EndDialogue()
    {
        _isDialogueActive = false;
        dialogueBoxPlaceholder.SetActive(false);
    }

    //Start is called before the first frame update
    void Start()
    {
        _inputHandler = GameObject.Find("GameManager").GetComponent<InputHandler>();

        //_dialogueBoxPlaceholder = GameObject.Find("DialogueBoxPlaceholder");
        dialogueBoxPlaceholder.SetActive(false);
        //_dialogueLines = new List<string>()
        //{
        //    "Hello, I'd like a drink please.",
        //    "I'd like a strong drink.",
        //    "I just got off a terrible shift at work."
        //};
    }

    // Update is called once per frame
    void Update()
    {  
        //if(_inputHandler.GetLeftMouseDown())
        //{
        //    Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
        //    RaycastHit hit;

        //    // Raycast to detect interactable object
        //    if (Physics.Raycast(ray, out hit, maxDistance, interactableLayer))
        //    {
        //        if (hit.collider != null)
        //        {
        //            Debug.Log("Clicked on " + hit.collider.gameObject.name);
        //            displayMessage(_dialogueLines);
        //        }
        //    }
        //}

        if(_isDialogueActive && _inputHandler.GetLeftMouseDown())
        {
            if (_skipNextClick)
            {
                _skipNextClick = false;
                return;
            }
            DisplayNextLine();
        }
    }
}
