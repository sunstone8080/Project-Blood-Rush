using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CocktailSystem : MonoBehaviour
{
    public event System.Action<string> OnDrinkSubmitted;
    public static CocktailSystem Instance { get; private set; }

    [Header("UI Text")]
    public TMP_Text cocktailDisplayText;

    [Header("Held Position")]
    private Vector3 holdLocalPosition = new Vector3(0.85f, -0.35f, 1f);

    [Header("Cocktail Prefabs")]
    public GameObject eyeballitiPrefab;
    public GameObject cranialPrefab;
    public GameObject fingerFloatPrefab;
    public GameObject darkGrowlyPrefab;
    public GameObject unknownCocktailPrefab;

    private List<string> currentDrink = new List<string>();
    private bool drinkCompleted = false;

    private Dictionary<string, List<string>> recipes = new Dictionary<string, List<string>>()
    {
        { "Eyeballiti", new List<string> { "eyeballs", "bloodtonic", "syrup" } },
        { "Cranial Cocktail", new List<string> { "brain", "syrup", "bloodtonic", "orange" } },
        { "Finger Float", new List<string> { "fingers", "tears", "fawnka" } },
        { "Dark & Growly", new List<string> { "moonrocks", "tears", "fawnka", "orange" } }
    };

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void StartNewDrink(string baseIngredient)
    {
        currentDrink.Clear();
        currentDrink.Add(Normalize(baseIngredient));
        drinkCompleted = false;

        UpdateUI();
    }

    public void AddIngredient(string ingredientName)
    {
        if (drinkCompleted) return;

        if (currentDrink.Count == 0)
        {
            Debug.LogWarning("Start with a base ingredient first.");
            return;
        }

        currentDrink.Add(Normalize(ingredientName));

        UpdateUI();

        //  FIX: delayed evaluation prevents missing last ingredient
        StartCoroutine(EvaluateAfterFrame());
    }

    private IEnumerator EvaluateAfterFrame()
    {
        yield return null; // wait 1 frame so all systems update

        if (drinkCompleted) yield break;

        string match = GetMatchedRecipe();

        if (match != null)
        {
            Debug.Log("MATCH FOUND: " + match);

            drinkCompleted = true;

            ReplaceHeldCocktail(match);
        }
    }

    public void SubmitDrink()
    {
        string result = GetMatchedRecipe();
        if (result == null) result = "Unknown Drink";

        Debug.Log("Submitted: " + result);

     
        OnDrinkSubmitted?.Invoke(result);

        currentDrink.Clear();
        drinkCompleted = false;

        UpdateUI();
    }

    private void ReplaceHeldCocktail(string recipeName)
    {
        GameObject prefab = GetPrefab(recipeName);

        if (prefab == null)
        {
            Debug.LogWarning("No prefab for: " + recipeName);
            return;
        }

        SelectableObject.ReplaceHeldItem(prefab, holdLocalPosition);
    }

    private GameObject GetPrefab(string recipe)
    {
        switch (recipe)
        {
            case "Eyeballiti": return eyeballitiPrefab;
            case "Cranial Cocktail": return cranialPrefab;
            case "Finger Float": return fingerFloatPrefab;
            case "Dark & Growly": return darkGrowlyPrefab;
            default: return unknownCocktailPrefab;
        }
    }
    public void ResetDrink()
    {
        currentDrink.Clear();
        drinkCompleted = false;

        UpdateUI();

        Debug.Log("Drink reset.");

        // optional: remove held item if one exists
        SelectableObject.ReplaceHeldItem(unknownCocktailPrefab, new Vector3(0.85f, -0.35f, 1f));
    }
    private string GetMatchedRecipe()
    {
        foreach (var recipe in recipes)
        {
            if (Matches(recipe.Value))
                return recipe.Key;
        }

        return null;
    }

    private bool Matches(List<string> recipe)
    {
        if (recipe.Count != currentDrink.Count)
            return false;

        List<string> temp = new List<string>(currentDrink);

        foreach (string r in recipe)
        {
            if (!temp.Remove(Normalize(r)))
                return false;
        }

        return true;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetDrink();
        }
    }
    private void UpdateUI()
    {
        Debug.Log("UpdateUI called");

        if (cocktailDisplayText == null)
        {
            Debug.LogError("UI TEXT IS NULL");
            return;
        }

        string text =
            currentDrink.Count == 0
            ? "Cocktail: (empty)"
            : "Cocktail: " + string.Join(", ", currentDrink);

        cocktailDisplayText.text = text;

        Debug.Log("UI SET TO: " + text);
    }

    private string Normalize(string s)
    {
        return s.Replace(" ", "").ToLower();
    }
}