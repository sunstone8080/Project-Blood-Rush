using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public TextAsset dialogueFile;

    public DialogueUiManager _dialogueUiManager;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        if (_dialogueUiManager == null)
        {
            Debug.LogError("DialogueUiManager reference is null.");
            return;
        }
        _dialogueUiManager.StartDialogue(dialogueFile);
    }
}
