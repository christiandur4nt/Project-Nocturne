using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    [Header("Spotlight Material To Edit")]
    public Material material;
    private Light lightSource;
    private AudioSource audioSource;

    private bool flickering;

    void Awake() {
        flickering = false;
        lightSource = GetComponent<Light>();
        audioSource = GetComponent<AudioSource>();
        lightSource.enabled = false;
        material.DisableKeyword("_EMISSION");
        UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
    }

    void Update()
    {
        // WIP: Make more random so lights are more out of sync
        if (!flickering) {
            int rand = UnityEngine.Random.Range(0, 5000);
            if (rand == 2500) {
                flickering = true;
                StartCoroutine(Flicker());
            }
        }
    }

    // WIP: Sync better with audio
    IEnumerator Flicker() {
        audioSource.Play(); 
        lightSource.enabled = true;
        material.EnableKeyword("_EMISSION");
        yield return new WaitForSeconds(2.0f);
        lightSource.enabled = false;
        material.DisableKeyword("_EMISSION");
        yield return new WaitForSeconds(2.0f);
        lightSource.enabled = true;
        material.EnableKeyword("_EMISSION");
        yield return new WaitForSeconds(1.0f);
        lightSource.enabled = false;
        material.DisableKeyword("_EMISSION");
        yield return new WaitForSeconds(1.0f);
        lightSource.enabled = true;
        material.EnableKeyword("_EMISSION");
        yield return new WaitForSeconds(1.0f);
        lightSource.enabled = false;
        material.DisableKeyword("_EMISSION");
        yield return new WaitForSeconds(2.5f);
        flickering = false;
    }
}
