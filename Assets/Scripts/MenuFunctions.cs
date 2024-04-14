using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class MenuFunctions : MonoBehaviour
{
    public static bool gameIsPaused = false;

    [Tooltip("Used for showing/hiding pause menu. N/A to main menu.")]
    public GameObject pauseMenuUI;

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
        pauseMenuUI.SetActive(false);
        Time.timeScale = previousTimeFlow;
        gameIsPaused = false;
    }

    void Pause()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        pauseMenuUI.SetActive(true);
        previousTimeFlow = Time.timeScale;
        Time.timeScale = 0f;
        gameIsPaused = true;
    }

    public void LoadNewScene(string name)
    {
        Time.timeScale = 1f;
        gameIsPaused = false;
        SceneManager.LoadScene(name);
    }
}
