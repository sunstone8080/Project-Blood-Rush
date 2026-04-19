using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState
    {
        FreeRoam,
        Bar,
        Collection
    }

    public GameState CurrentGameState = GameState.FreeRoam;

    [Header("UI")]
    public TMP_Text interactionText;

    private string currentPrompt = "";
    private int currentPriority = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // =========================
    // UI PROMPT SYSTEM
    // =========================

    public void SetPrompt(string message, int priority)
    {
        if (interactionText == null) return;

        if (priority < currentPriority)
            return;

        currentPrompt = message;
        currentPriority = priority;

        interactionText.text = message;
    }

    public void ClearPrompt()
    {
        currentPrompt = "";
        currentPriority = 0;

        if (interactionText != null)
            interactionText.text = "";
    }

    // Priority guide:
    // 1 = hover
    // 2 = npc interaction
    // 3 = locked state warning
    // 4 = bar state instruction
}