using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShutdownLights : MonoBehaviour
{
    public GameObject lights;
    public AudioSource audioSource;
    
    void OnTriggerEnter(Collider other)
    {
       audioSource.Play();
       lights.gameObject.SetActive(false);
    }
}
