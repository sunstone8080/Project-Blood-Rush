using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MiniGameManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text sequenceText;

    [Header("Settings")]
    public int sequenceLength = 5;

    private List<string> directionsSequence = new List<string>();
    private bool isMiniGameActive = false;
    private bool isProcessing = false;

    private InputHandler inputHandler;
    private GameManager gameManagerInstance;
    private SelectableObject pendingObject;

    void Start()
    {
        gameManagerInstance = GetComponent<GameManager>();
        inputHandler = GetComponent<InputHandler>();

        if (sequenceText != null)
            sequenceText.text = "+";
    }

    void Update()
    {
        if (isMiniGameActive)
            CheckPlayerInput();
    }

    public void StartMiniGameForObject(SelectableObject obj)
    {
        if (isMiniGameActive || isProcessing) return;

        isProcessing = true;
        pendingObject = obj;

        Ingredient ingredient = obj.GetComponent<Ingredient>();
        if (ingredient == null)
        {
            isProcessing = false;
            obj.OnInteract(Camera.main);
            return;
        }

       
        if (ingredient.isBaseIngredient)
        {
            if (gameManagerInstance != null)
                gameManagerInstance.CurrentGameState = GameManager.GameState.Collection;

            isMiniGameActive = true;
            directionsSequence.Clear();

            string[] options = { "Up", "Down", "Left", "Right" };
            for (int i = 0; i < sequenceLength; i++)
            {
                directionsSequence.Add(options[Random.Range(0, options.Length)]);
            }

            UpdateSequenceDisplay();
        }
        else
        {

            CocktailSystem.Instance.AddIngredient(ingredient.ingredientName);
            obj.OnInteract(Camera.main);

            isProcessing = false;
        }
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

        string current = directionsSequence[0];
        bool correct = false;
        bool wrong = false;

        if (current == "Up")
        {
            if (inputHandler.GetUpInput()) correct = true;
            else if (inputHandler.GetDownInput() || inputHandler.getLeftInput() || inputHandler.getRightInput()) wrong = true;
        }
        else if (current == "Down")
        {
            if (inputHandler.GetDownInput()) correct = true;
            else if (inputHandler.GetUpInput() || inputHandler.getLeftInput() || inputHandler.getRightInput()) wrong = true;
        }
        else if (current == "Left")
        {
            if (inputHandler.getLeftInput()) correct = true;
            else if (inputHandler.GetUpInput() || inputHandler.GetDownInput() || inputHandler.getRightInput()) wrong = true;
        }
        else if (current == "Right")
        {
            if (inputHandler.getRightInput()) correct = true;
            else if (inputHandler.GetUpInput() || inputHandler.GetDownInput() || inputHandler.getLeftInput()) wrong = true;
        }

        if (wrong)
        {
            EndMiniGame(false);
            return;
        }

        if (correct)
        {
            directionsSequence.RemoveAt(0);
            UpdateSequenceDisplay();

            if (directionsSequence.Count == 0)
                EndMiniGame(true);
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
            Ingredient ingredient = pendingObject.GetComponent<Ingredient>();

            if (ingredient != null)
            {
                
                CocktailSystem.Instance.StartNewDrink(ingredient.ingredientName);
                pendingObject.OnInteract(Camera.main);
            }
        }

        pendingObject = null;
        isProcessing = false;

        Debug.Log(success ? "Mini-game completed!" : "Mini-game failed!");
    }
}