using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShutdownLights : MonoBehaviour
{
    public GameObject lights;
    public AudioSource audioSource;
    bool triggered = false;
    
    void OnTriggerEnter(Collider other)
    {
        // Play once
        if (!triggered) {
            audioSource.Play();
            lights.gameObject.SetActive(false);
            triggered = true;
        }
    }
}
