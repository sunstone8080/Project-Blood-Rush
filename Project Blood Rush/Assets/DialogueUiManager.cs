using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueUiManager : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text dialogueText;

    // temp list of dialogue
    private List<string> dialogueLines;

    void displayMessage(List<string> dialogueLines)
    {
        // for now just display the first line of dialogue
        dialogueText.text = dialogueLines[0];
        nameText.text = "Vampire";

        // add functionality to cycle through dialogue lines on click
    }

    // Start is called before the first frame update
    void Start()
    {
        dialogueLines = new List<string>()
        {
            "Hello, I'd like a drink please.",
            "I'd like a strong drink.",
            "I just got off a terrible shift at work."
        };

        displayMessage(dialogueLines);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
