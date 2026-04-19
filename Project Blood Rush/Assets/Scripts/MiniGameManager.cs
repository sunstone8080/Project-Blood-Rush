using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MiniGameManager : MonoBehaviour
{
    [Header("Arrow UI")]
    public GameObject arrowContainerObject;
    public Transform arrowContainer;
    public GameObject arrowPrefab;

    [Header("Optional Text Fallback")]
    public TMP_Text sequenceText;

    [Header("Arrow Sprites")]
    public Sprite upArrow;
    public Sprite downArrow;
    public Sprite leftArrow;
    public Sprite rightArrow;

    [Header("Settings")]
    public int sequenceLength = 5;

    private List<string> directionsSequence = new List<string>();
    private List<GameObject> arrowObjects = new List<GameObject>();

    private bool isMiniGameActive = false;
    private bool isProcessing = false;

    private InputHandler inputHandler;
    private GameManager gameManagerInstance;
    private SelectableObject pendingObject;

    void Start()
    {
        gameManagerInstance = FindObjectOfType<GameManager>();
        inputHandler = FindObjectOfType<InputHandler>();

        if (sequenceText != null)
            sequenceText.text = "+";

        if (arrowContainerObject != null)
            arrowContainerObject.SetActive(false);
    }

    void Update()
    {
        if (isMiniGameActive)
            CheckPlayerInput();
    }

    public void StartMiniGameForObject(SelectableObject obj)
    {
        if (isMiniGameActive || isProcessing || obj == null)
            return;

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

            if (arrowContainerObject != null)
                arrowContainerObject.SetActive(true);

            GenerateSequence();
            UpdateSequenceDisplay();
        }
        else
        {
            CocktailSystem.Instance.AddIngredient(ingredient.ingredientName);
            obj.OnInteract(Camera.main);
            isProcessing = false;
        }
    }

    private void GenerateSequence()
    {
        string[] options = { "Up", "Down", "Left", "Right" };

        for (int i = 0; i < sequenceLength; i++)
        {
            directionsSequence.Add(options[Random.Range(0, options.Length)]);
        }
    }

    private void UpdateSequenceDisplay()
    {
        ClearArrows();

        foreach (string dir in directionsSequence)
        {
            GameObject arrowGO = Instantiate(arrowPrefab, arrowContainer);

            Image[] images = arrowGO.GetComponentsInChildren<Image>(true);

            Sprite s = GetArrowSprite(dir);

            foreach (Image img in images)
            {

                if (s != null)
                {
                    img.sprite = s;
                    img.enabled = true;
                }
                else
                {
                    img.enabled = false;
                }
                img.preserveAspect = true;

              
                if (s != null)
                {
                    img.sprite = s;
                    break;
                }
            }

            arrowObjects.Add(arrowGO);
        }

        if (sequenceText != null)
            sequenceText.text = "";
    }

    private Sprite GetArrowSprite(string direction)
    {
        switch (direction)
        {
            case "Up": return upArrow;
            case "Down": return downArrow;
            case "Left": return leftArrow;
            case "Right": return rightArrow;
            default: return null;
        }
    }

    private void CheckPlayerInput()
    {
        if (inputHandler == null || directionsSequence.Count == 0)
            return;

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

            if (arrowObjects.Count > 0)
            {
                Destroy(arrowObjects[0]);
                arrowObjects.RemoveAt(0);
            }

            if (directionsSequence.Count == 0)
                EndMiniGame(true);
        }
    }

    private void EndMiniGame(bool success)
    {
        isMiniGameActive = false;

        if (gameManagerInstance != null)
            gameManagerInstance.CurrentGameState = GameManager.GameState.FreeRoam;

        if (sequenceText != null)
            sequenceText.text = "+";

        if (arrowContainerObject != null)
            arrowContainerObject.SetActive(false);

        ClearArrows();

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

    private void ClearArrows()
    {
        foreach (GameObject arrow in arrowObjects)
        {
            if (arrow != null)
                Destroy(arrow);
        }

        arrowObjects.Clear();
    }

    private void DisableExtraImages(GameObject arrowGO)
    {
        Image[] images = arrowGO.GetComponentsInChildren<Image>(true);

        foreach (Image img in images)
        {
            if (img.sprite == null)
                img.enabled = false;
            else
                img.enabled = true;
        }
    }
}