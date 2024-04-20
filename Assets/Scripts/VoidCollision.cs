using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class VoidCollision : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            // Debug.Log("Resetting to checkpoint: " + pm.CheckpointPos);
            PlayerUIManager.Instance.Die();
        } else if (other.CompareTag("Wall")) {
            Destroy(other.gameObject);
        }
    }
}
