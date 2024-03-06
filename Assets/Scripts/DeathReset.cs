using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DeathReset : MonoBehaviour
{
    private PlayerManager pm;

    void Awake() {
        pm = FindFirstObjectByType<PlayerManager>();
    } 

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            Debug.Log("Resetting to checkpoint: " + pm.CheckpointPos);
            pm.ResetToCheckpoint();
        }
    }
}
