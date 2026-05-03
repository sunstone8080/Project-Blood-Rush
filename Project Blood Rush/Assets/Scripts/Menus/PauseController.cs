using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseController : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject optionsMenu;
    public TMP_Text crosshair;

    private void Start()
    {
        
    }

    private void Update()
    {
        // TEMP using G key, switch to esc for final build
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePause();
            }
        }
    }

    // function that is called when pressing g(temp)/esc(final) no matter what
    public void TogglePause()
    {
        // If options is open, just close it and stay paused
        if (optionsMenu.activeSelf)
        {
            optionsMenu.SetActive(false);
            return;
        }

        bool willPause = !pauseMenu.activeSelf;

        pauseMenu.SetActive(willPause);

        if (willPause)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            if (crosshair != null) crosshair.enabled = false;

            if (GameManager.Instance != null)
            {
                GameManager.Instance.previousState = GameManager.Instance.CurrentGameState;
                GameManager.Instance.CurrentGameState = GameManager.GameState.Paused;
            }
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            if (crosshair != null) crosshair.enabled = true;

            if (GameManager.Instance != null)
            {
                GameManager.Instance.CurrentGameState = GameManager.Instance.previousState;
            }
        }
    }

    // function that is called when pressing resume button in pause menu
    public void Resume()
    {
        TogglePause();
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadSceneAsync(0);
    }
}
