using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public TMP_Text dialogueText;

    private Dictionary<string, List<string>> dialogueBlocks = new Dictionary<string, List<string>>();

    private List<string> currentBlock;
    private int currentIndex;

    private string currentCharacter;
    private string expectedDrink;
    private bool waitingForDrink = false;

    void Start()
    {
        LoadDialogue("dialogue");
        StartDialogue("WEREWOLF_START");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !waitingForDrink)
        {
            NextLine();
        }
    }

    void LoadDialogue(string fileName)
    {
        TextAsset file = Resources.Load<TextAsset>(fileName);
        string[] lines = file.text.Split('\n');

        string currentTag = "";
        List<string> currentList = null;

        foreach (string raw in lines)
        {
            string line = raw.Trim();
            if (string.IsNullOrEmpty(line)) continue;

            if (line.StartsWith("#"))
            {
                if (line.StartsWith("#ORDER"))
                {
                    currentList.Add(line);
                    continue;
                }

                currentTag = line.Replace("#", "");
                currentList = new List<string>();
                dialogueBlocks[currentTag] = currentList;
            }
            else
            {
                currentList.Add(line);
            }
        }
    }

    public void StartDialogue(string tag)
    {
        if (!dialogueBlocks.ContainsKey(tag))
        {
            Debug.LogError("Missing tag: " + tag);
            return;
        }

        currentBlock = dialogueBlocks[tag];
        currentIndex = 0;

        // extract character name (WEREWOLF from WEREWOLF_START)
        currentCharacter = tag.Split('_')[0];

        DisplayLine();
    }

    void NextLine()
    {
        currentIndex++;

        if (currentIndex >= currentBlock.Count)
        {
            dialogueText.text = "";
            return;
        }

        DisplayLine();
    }

    void DisplayLine()
    {
        string line = currentBlock[currentIndex];

        //  ORDER DETECTED
        if (line.StartsWith("#ORDER"))
        {
            expectedDrink = line.Replace("#ORDER", "").Trim();
            waitingForDrink = true;

            dialogueText.text = currentCharacter + " wants: " + expectedDrink;
            Debug.Log("WAITING FOR DRINK: " + expectedDrink);

            return;
        }

        // NORMAL LINE
        dialogueText.text = line;
    }

    //  CALL THIS FROM YOUR DRINK SYSTEM
    public void SubmitDrink(string drinkName)
    {
        if (!waitingForDrink) return;

        waitingForDrink = false;

        if (drinkName == expectedDrink)
        {
            StartDialogue(currentCharacter + "_GOOD");
        }
        else if (IsCloseDrink(drinkName, expectedDrink))
        {
            StartDialogue(currentCharacter + "_BAD");
        }
        else
        {
            StartDialogue(currentCharacter + "_WRONG");
        }
    }

    // OPTIONAL: define "almost correct"
    bool IsCloseDrink(string given, string expected)
    {
        return given.ToLower().Contains(expected.ToLower());
    }
}