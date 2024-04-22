using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TriggerEndGame : MonoBehaviour
{
    const float FADE_TIME_SECONDS = 5;

    [Header("Components")]
    public GameObject playerStartPos;
    public Rigidbody playerRB;
    public Camera camera1;
    public Camera camera2;
    public AudioClip endMusic;
    public AudioSource musicManager;
    [SerializeField] private GameObject flashlight;
    private Transform cameraHolder;
    private GameObject playerCamera;
    private PlayerMovement playerMovement;

    [SerializeField] private GameObject dogGO;
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private Animator dog;

    void Awake() {
        cameraHolder = GameObject.Find("Camera Holder").transform;
        playerCamera = GameObject.Find("Player Camera");
        playerMovement = FindFirstObjectByType<PlayerMovement>();
        dogGO.GetComponent<NavMeshAgent>().isStopped = true;
        dogGO.GetComponent<Rigidbody>().freezeRotation = true;
    }

    void PlayRandom()
    {
        StartCoroutine(FadeIn());
        
        StartCoroutine(FadeOut(musicManager.clip.length - FADE_TIME_SECONDS));
    }

    IEnumerator FadeOut(float delay) 
    {
        yield return new WaitForSeconds(delay);
        float timeElapsed = 0;

        while (musicManager.volume > 0) 
        {
            musicManager.volume = Mathf.Lerp(1, 0, timeElapsed / FADE_TIME_SECONDS);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator FadeIn() 
    {
        float timeElapsed = 0;
        musicManager.volume = 0;
        while (musicManager.volume < 1) 
        {
            musicManager.volume = Mathf.Lerp(0, 1, timeElapsed / FADE_TIME_SECONDS);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            musicManager.Stop();
            musicManager.clip = endMusic;
            other.transform.position = playerStartPos.transform.position;
            cameraHolder.rotation = Quaternion.LookRotation(-playerStartPos.transform.forward);
            StartCoroutine(ActionSequence());
        }
    }

    public IEnumerator ActionSequence() {
        playerRB.velocity = Vector3.zero;
        playerMovement.DisableMovement();
        playerCamera.SetActive(false);
        camera1.gameObject.SetActive(true);
        StartCoroutine(FadeIn());
        musicManager.Play();
        yield return new WaitForSeconds(2f);
        camera1.gameObject.SetActive(false);
        camera2.gameObject.SetActive(true);
        doorAnimator.SetBool("CutScene", true);
        dog.SetBool("CutScene", true);
        yield return new WaitForSeconds(5f);
        doorAnimator.SetBool("CutScene", false);
        dog.SetBool("CutScene", false);
        camera2.gameObject.SetActive(false);
        playerCamera.SetActive(true);
        dogGO.GetComponent<NavMeshAgent>().acceleration = 240;
        dogGO.GetComponent<NavMeshAgent>().angularSpeed = 300;
        dogGO.GetComponent<Rigidbody>().freezeRotation = false;
        dogGO.GetComponent<NavMeshAgent>().isStopped = false;
        playerMovement.EnableMovement();
        flashlight.SetActive(true);
        PlayerUIManager.Instance.ResetDog = true;
    }
}
