using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public bool updateRespawn;
    public bool triggerTutorial;

    // Internal
    private bool reached = false;
    private PlayerManager pm;
    private TutorialUI tutorialUIScript;
    private Vector3 teleportPos;

    void Reset() {
        updateRespawn = true;
        triggerTutorial = false;
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
            if (triggerTutorial && tutorialUIScript != null)
            {
                tutorialUIScript.gameObject.SetActive(true);
            }
        }
    }
}
