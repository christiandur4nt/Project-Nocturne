using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class MenuFunctions : MonoBehaviour
{
    public static bool gameIsPaused = false;

    [Tooltip("Used for showing/hiding pause menu. N/A to main menu.")]
    public GameObject pauseMenu;

    [Tooltip("Used for showing/hiding level menu. N/A to pause menu.")]
    public GameObject levelMenu;

    public GameObject settingsMenu;

    private float previousTimeFlow;

    void Update()
    {
        // Only allow pausing in-game
        if (!SceneManager.GetActiveScene().name.Equals("MainMenu") && Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameIsPaused)
            {
                Debug.Log("Resuming...");
                Resume();
            }   
            else
            {
                Debug.Log("Pausing...");
                Pause();
            }
        }
    }

    void Resume()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        pauseMenu.SetActive(false);
        Time.timeScale = previousTimeFlow;
        gameIsPaused = false;
    }

    void Pause()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        pauseMenu.SetActive(true);
        previousTimeFlow = Time.timeScale;
        Time.timeScale = 0f;
        gameIsPaused = true;
    }

    public void ToggleLevelMenu() {
        levelMenu.SetActive(!levelMenu.activeSelf);
        settingsMenu.SetActive(false);
    }

    public void ToggleSettingsMenu() {
        settingsMenu.SetActive(!settingsMenu.activeSelf);
        levelMenu.SetActive(false);
    }

    public void LoadNewScene(string name)
    {
        Time.timeScale = 1f;
        gameIsPaused = false;
        SceneManager.LoadScene(name);
    }

    public void QuitGame() {
        // Exits playmode in editor
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
