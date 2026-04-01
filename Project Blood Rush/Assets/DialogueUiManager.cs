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
    public LayerMask interactableLayer;
    public float maxDistance = 10f;

    // temp list of dialogue
    private List<string> _dialogueLines;

    private GameObject _dialogueBoxPlaceholder;
    private GameObject _sprite;
    private InputHandler _inputHandler;

    public void displayMessage(List<string> dialogueLines)
    {
        // for now just display the first line of dialogue
        dialogueText.text = dialogueLines[0];
        nameText.text = "Vampire";

        // add functionality to cycle through dialogue lines on click, target sprites
    }

    // Start is called before the first frame update
    void Start()
    {
        _inputHandler = GameObject.Find("GameManager").GetComponent<InputHandler>();

        _dialogueBoxPlaceholder = GameObject.Find("DialogueBoxPlaceholder");
        _dialogueBoxPlaceholder.SetActive(false);
        _dialogueLines = new List<string>()
        {
            "Hello, I'd like a drink please.",
            "I'd like a strong drink.",
            "I just got off a terrible shift at work."
        };
    }

    // Update is called once per frame
    void Update()
    {  
        if(_inputHandler.GetLeftMouseDown())
        {
            Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
            RaycastHit hit;

            // Raycast to detect interactable object
            if (Physics.Raycast(ray, out hit, maxDistance, interactableLayer))
            {
                if (hit.collider != null)
                {
                    Debug.Log("Clicked on " + hit.collider.gameObject.name);
                }
            }
        }

    }
}
