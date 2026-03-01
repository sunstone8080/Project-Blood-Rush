using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MiniGameManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text sequenceText; // TMP for DDR arrows / reticle

    [Header("Settings")]
    public int sequenceLength = 5;

    private List<string> directionsSequence = new List<string>();
    private bool isMiniGameActive = false;

    private InputHandler inputHandler;
    private GameManager gameManagerInstance;

    // The object we're trying to collect
    private SelectableObject pendingObject;

    void Start()
    {
        gameManagerInstance = GetComponent<GameManager>();
        inputHandler = GetComponent<InputHandler>();

        if (gameManagerInstance == null)
            Debug.LogError("No GameManager found on this GameObject!");
        if (inputHandler == null)
            Debug.LogError("No InputHandler found on this GameObject!");

        if (sequenceText != null)
            sequenceText.text = "+";
    }

    void Update()
    {
        // Process DDR inputs if mini-game is active
        if (isMiniGameActive)
            CheckPlayerInput();
    }

    /// <summary>
    /// Call this to start the mini-game for a specific object.
    /// </summary>
    public void StartMiniGameForObject(SelectableObject obj)
    {
        if (isMiniGameActive) return;

        pendingObject = obj;

        if (gameManagerInstance != null)
            gameManagerInstance.CurrentGameState = GameManager.GameState.Collection;

        isMiniGameActive = true;
        directionsSequence.Clear();

        string[] options = { "Up", "Down", "Left", "Right" };
        for (int i = 0; i < sequenceLength; i++)
        {
            string randomDir = options[Random.Range(0, options.Length)];
            directionsSequence.Add(randomDir);
        }

        UpdateSequenceDisplay();
    }

    void UpdateSequenceDisplay()
    {
        if (sequenceText == null) return;

        string display = "";
        foreach (string dir in directionsSequence)
        {
            switch (dir)
            {
                case "Up": display += "^ "; break;
                case "Down": display += "V "; break;
                case "Left": display += "< "; break;
                case "Right": display += "> "; break;
            }
        }

        sequenceText.text = display;
    }

    void CheckPlayerInput()
    {
        if (inputHandler == null || directionsSequence.Count == 0) return;

    string currentInput = directionsSequence[0];
    bool correct = false;
    bool wrongInput = false;

    // Check player input
    if (currentInput == "Up")
    {
        if (inputHandler.GetUpInput()) correct = true;
        else if (inputHandler.GetDownInput() || inputHandler.getLeftInput() || inputHandler.getRightInput())
            wrongInput = true;
    }
    else if (currentInput == "Down")
    {
        if (inputHandler.GetDownInput()) correct = true;
        else if (inputHandler.GetUpInput() || inputHandler.getLeftInput() || inputHandler.getRightInput())
            wrongInput = true;
    }
    else if (currentInput == "Left")
    {
        if (inputHandler.getLeftInput()) correct = true;
        else if (inputHandler.GetUpInput() || inputHandler.GetDownInput() || inputHandler.getRightInput())
            wrongInput = true;
    }
    else if (currentInput == "Right")
    {
        if (inputHandler.getRightInput()) correct = true;
        else if (inputHandler.GetUpInput() || inputHandler.GetDownInput() || inputHandler.getLeftInput())
            wrongInput = true;
    }

    if (wrongInput)
    {
        // Player pressed the wrong button -> fail
        EndMiniGame(false);
        return;
    }

    if (correct)
    {
        // Remove the first input since it was pressed correctly
        directionsSequence.RemoveAt(0);
        UpdateSequenceDisplay();

        // Check if mini-game is complete
        if (directionsSequence.Count == 0)
        {
            EndMiniGame(true);
        }
    }
    }

    void EndMiniGame(bool success)
    {
        isMiniGameActive = false;

        if (gameManagerInstance != null)
            gameManagerInstance.CurrentGameState = GameManager.GameState.FreeRoam;

        if (sequenceText != null)
            sequenceText.text = "+";

        if (success && pendingObject != null)
        {
            // Give the object to the player
            pendingObject.OnInteract(Camera.main);
        }

        pendingObject = null;

        Debug.Log(success ? "Mini-game completed!" : "Mini-game failed!");
    }
}