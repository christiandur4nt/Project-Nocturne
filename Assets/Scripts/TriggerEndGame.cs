using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEndGame : MonoBehaviour
{
    [Header("Components")]
    public GameObject playerStartPos;
    public Camera camera1;
    public Camera camera2;
    private Transform cameraHolder;
    private GameObject playerCamera;
    private PlayerMovement playerMovement;

    [SerializeField] private Animator doorAnimator;
    [SerializeField] private Animator dog;

    void Awake() {
        cameraHolder = GameObject.Find("Camera Holder").transform;
        playerCamera = GameObject.Find("Player Camera");
        playerMovement = FindFirstObjectByType<PlayerMovement>();
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            doorAnimator.SetBool("CutScene", true);
            dog.SetBool("CutScene", true);
            other.transform.position = playerStartPos.transform.position;
            playerMovement.DisableMovement();
            // WIP: Rotate camera
            Debug.Log(playerStartPos.transform.right);
            cameraHolder.rotation = Quaternion.LookRotation(-playerStartPos.transform.forward);
            StartCoroutine(ActionSequence());
            // playerMovement.EnableMovement();
        }
        
    }

    public IEnumerator ActionSequence() {
        playerCamera.SetActive(false);
        camera1.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        camera1.gameObject.SetActive(false);
        camera2.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        camera2.gameObject.SetActive(false);
        playerCamera.SetActive(true);
    }
}
