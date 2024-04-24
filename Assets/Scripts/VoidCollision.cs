using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class VoidCollision : MonoBehaviour
{
    private PlayerManager playerManager;

    void Awake() {
        playerManager = FindFirstObjectByType<PlayerManager>();
    } 

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            // Debug.Log("Resetting to checkpoint: " + playerManager.CheckpointPos);
            PlayerUIManager.Instance.Die(); // WIP: Needs testing & fixing
        } else if (other.CompareTag("Wall")) {
            Destroy(other.gameObject);
        }
    }
}
