using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private bool reached = false;
    private PlayerManager pm;
    private Vector3 teleportPos;

    void Awake() {
        pm = FindFirstObjectByType<PlayerManager>();
        teleportPos = transform.Find("Checkpoint").position;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!reached) {
            Debug.Log("Checkpoint Reached!");
            reached = false;
            pm.CheckpointPos = teleportPos;
        }
    }
}
