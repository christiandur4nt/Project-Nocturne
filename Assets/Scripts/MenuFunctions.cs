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

    void Awake() {
        if (!PlayerPrefs.HasKey("armsOn")) PlayerPrefs.SetInt("armsOn", 1);
    }

    void Start() {
        sensitivitySlider.value = PlayerPrefs.GetFloat("sensitivity");
        fovSlider.value = PlayerPrefs.GetFloat("fieldOfView");
        masterSlider.value = PlayerPrefs.GetFloat("masterVolume");
        musicSlider.value = PlayerPrefs.GetFloat("soundFXVolume");
        soundFXSlider.value = PlayerPrefs.GetFloat("musicVolume");
        armsToggle.isOn = PlayerPrefs.GetInt("armsOn") == 1;
    }

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

    public void ResetSettings() {
        PlayerPrefs.SetFloat("sensitivity", CameraMovement.DEFAULT_SENSITIVITY);
        if (cameraMovement != null) cameraMovement.UpdateSensitivity();
        PlayerPrefs.SetFloat("fieldOfView", CameraMovement.DEFAULT_FIELD_OF_VIEW);
        if (cameraMovement != null) cameraMovement.UpdateFOV();
        MainMixerManager.Instance.SetMasterVolume(0);
        MainMixerManager.Instance.SetMusicVolume(0);
        MainMixerManager.Instance.SetSoundFXVolume(0);
        armsToggle.isOn = true;
        CalibrateSliders();
    }

    private void CalibrateSliders() {
        sensitivitySlider.value = PlayerPrefs.GetFloat("sensitivity");
        fovSlider.value = PlayerPrefs.GetFloat("fieldOfView");
        masterSlider.value = PlayerPrefs.GetFloat("masterVolume");
        musicSlider.value = PlayerPrefs.GetFloat("soundFXVolume");
        soundFXSlider.value = PlayerPrefs.GetFloat("musicVolume");
        armsToggle.SetIsOnWithoutNotify(PlayerPrefs.GetInt("armsOn") == 1);
    }

    public void SetSensitivity() {
        PlayerPrefs.SetFloat("sensitivity", sensitivitySlider.value);
        if (cameraMovement != null) cameraMovement.UpdateSensitivity();
    }

    public void SetFOV() {
        PlayerPrefs.SetFloat("fieldOfView", fovSlider.value);
        if (cameraMovement != null) cameraMovement.UpdateFOV();
    }

    public void SetMasterVolume() {
        MainMixerManager.Instance.SetMasterVolume(masterSlider.value);
    }

    public void SetMusicVolume() {
        MainMixerManager.Instance.SetMusicVolume(musicSlider.value);
    }

    public void SetSoundFXVolume() {
        MainMixerManager.Instance.SetSoundFXVolume(soundFXSlider.value);
    }

    public void ToggleArms() {
        if (arms != null) arms.SetActive(armsToggle.isOn);
        PlayerPrefs.SetInt("armsOn", armsToggle.isOn ? 1 : 0);
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
