using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameObject pause;
    public GameObject play;
    public GameManager.GameState previousState;
    public enum GameState
    {
        FreeRoam,
        Bar,
        Collection,
            Paused
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

    public void NextTrack()
    {
        if (MusicManager.instance != null)
            MusicManager.instance.NextTrack();
    }

    public void PreviousTrack()
    {
        if (MusicManager.instance != null)
            MusicManager.instance.PreviousTrack();
    }
    public void playTrack()
    {
        if (MusicManager.instance != null)
            MusicManager.instance.ResumeTrack();
        play.SetActive(false);
        pause.SetActive(true);
    }

    public void PauseTrack()
    {
        if (MusicManager.instance != null)
            MusicManager.instance.PauseTrack();
        play.SetActive(true);
        pause.SetActive(false);
        
    }
}