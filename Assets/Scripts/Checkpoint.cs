using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public bool updateRespawn;

    // Internal
    private bool reached = false;
    private PlayerManager pm;
    private TutorialUI tutorialUIScript;
    private Vector3 teleportPos;

    void Reset() {
        updateRespawn = true;
    }

    void Awake() {
        pm = FindFirstObjectByType<PlayerManager>();
        tutorialUIScript = FindFirstObjectByType<TutorialUI>();
        teleportPos = transform.Find("Checkpoint").position;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!reached) {
            Debug.Log("Checkpoint Reached!");
            reached = true;
            if (updateRespawn) pm.CheckpointPos = teleportPos;

            // If a tutorial is present, display it
            if (tutorialUIScript != null && !tutorialUIScript.tutorialOver)
            {
                tutorialUIScript.gameObject.SetActive(true);
            }
        }
    }
}
