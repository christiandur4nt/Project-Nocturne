using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MenuFunctions : MonoBehaviour
{
    [Header("Global Pause Boolean")]
    public static bool gameIsPaused = false;

    [Header("Required Components")]
    public MainMixerManager mainMixerManager;
    public CameraMovement cameraMovement; // WIP: Sens must be changed using player prefs, since main menu doesn't have access to cameraMovement object
    public GameObject settingsMenu;
    public Slider sensitivitySlider;
    public Slider fovSlider;
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider soundFXSlider;
    public Toggle armsToggle;
    public GameObject arms;

    [Header("Components for Main Menu")]
    [Tooltip("Used for showing/hiding level menu. N/A to pause menu.")]
    public GameObject levelMenu;

    [Header("Components for Pause Menu")]
    [Tooltip("Used for showing/hiding pause menu. N/A to main menu.")]
    public GameObject pauseMenu;
    [Tooltip("Used to disable movement when paused")]
    public PlayerMovement playerMovement;
    public AudioMixerSnapshot pausedSnapshot;
    public AudioMixerSnapshot unpausedSnapshot;

    // Internal
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
        unpausedSnapshot.TransitionTo(0.1f);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        pauseMenu.SetActive(false);
        Time.timeScale = previousTimeFlow;
        playerMovement.enableMovement();
        gameIsPaused = false;
    }

    void Pause()
    {
        pausedSnapshot.TransitionTo(0);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        pauseMenu.SetActive(true);
        previousTimeFlow = Time.timeScale;
        Time.timeScale = 0f;
        playerMovement.disableMovement();
        gameIsPaused = true;
    }

    // Settings Functions

    public void SetSensitivity() {
        cameraMovement.AdjustSensitivity(sensitivitySlider.value);
    }

    public void SetFOV() {
        cameraMovement.AdjustFOV(fovSlider.value);
    }

    public void SetMasterVolume() {
        mainMixerManager.SetMasterVolume(masterSlider.value);
    }

    public void SetMusicVolume() {
        mainMixerManager.SetMusicVolume(musicSlider.value);
    }

    public void SetSoundFXVolume() {
        mainMixerManager.SetSoundFXVolume(soundFXSlider.value);
    }

    public void ToggleArms() {
        arms.SetActive(!arms.activeSelf);
    }

    public void ToggleLevelMenu() {
        levelMenu.SetActive(!levelMenu.activeSelf);
        settingsMenu.SetActive(false);
    }

    public void ToggleSettingsMenu() {
        settingsMenu.SetActive(!settingsMenu.activeSelf);
        if (SceneManager.GetActiveScene().name.Equals("MainMenu"))
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
