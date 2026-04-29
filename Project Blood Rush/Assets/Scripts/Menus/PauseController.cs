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
        if (Input.GetKeyDown(KeyCode.G))
        {
            TogglePause();
        }
    }

    // function that is called when pressing g(temp)/esc(final) no matter what
    public void TogglePause()
    {
        // If options is open, close it and return to pause menu
        if (optionsMenu.activeSelf)
        {
            optionsMenu.SetActive(false);
            return;
        }

        bool isPaused = !pauseMenu.activeSelf;
        pauseMenu.SetActive(isPaused);

        if (isPaused)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            if (crosshair != null) crosshair.enabled = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            if (crosshair != null) crosshair.enabled = true;
        }
    }

    // function that is called when pressing resume button in pause menu
    public void Resume()
    {
        pauseMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (crosshair != null) crosshair.enabled = true;
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadSceneAsync(0);
    }
}
