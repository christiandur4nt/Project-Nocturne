using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MainMixerManager : MonoBehaviour
{
    public static MainMixerManager Instance;
    public AudioMixer audioMixer;

    void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    void Start() {
        audioMixer.SetFloat("masterVolume", PlayerPrefs.GetFloat("masterVolume"));
        audioMixer.SetFloat("soundFXVolume", PlayerPrefs.GetFloat("soundFXVolume"));
        audioMixer.SetFloat("musicVolume", PlayerPrefs.GetFloat("musicVolume"));
    }

    public void SetMasterVolume(float level) {
        audioMixer.SetFloat("masterVolume", level);
        PlayerPrefs.SetFloat("masterVolume", level);
    }

    public void SetMusicVolume(float level) {
        audioMixer.SetFloat("musicVolume", level);
        PlayerPrefs.SetFloat("musicVolume", level);
    }

    public void SetSoundFXVolume(float level) {
        audioMixer.SetFloat("soundFXVolume", level);
        PlayerPrefs.SetFloat("soundFXVolume", level);
    }

    public float GetMasterVolume() {
        audioMixer.GetFloat("masterVolume", out float vol);
        return vol;
    }

    public float GetMusicVolume() {
        audioMixer.GetFloat("musicVolume", out float vol);
        return vol;
    }

    public float GetSoundFXVolume() {
        audioMixer.GetFloat("soundFXVolume", out float vol);
        return vol;
    }
}
