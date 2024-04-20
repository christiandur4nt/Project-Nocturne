using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public AudioSource checkpointSound;
    public bool updateRespawn;
    public bool triggerTutorial;

    // Internal
    private bool reached = false;
    private PlayerManager playerManager;
    private TutorialUI tutorialUIScript;
    private Vector3 teleportPos;

    void Reset() {
        updateRespawn = true;
        triggerTutorial = false;
    }

    void Awake() {
        playerManager = FindFirstObjectByType<PlayerManager>();
        tutorialUIScript = FindFirstObjectByType<TutorialUI>();
        teleportPos = transform.Find("Checkpoint").position;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!reached) {
            Debug.Log("Checkpoint Reached!");
            reached = true;
            if (updateRespawn) {
                playerManager.CheckpointPos = teleportPos;
                checkpointSound.Play();
            }

            // If a tutorial is present, display it
            if (triggerTutorial && tutorialUIScript != null)
            {
                tutorialUIScript.gameObject.SetActive(true);
            }
        }
    }
}
