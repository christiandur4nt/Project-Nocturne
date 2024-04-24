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
            PlayerUIManager.Instance.Die();
        } else if (other.CompareTag("Wall")) {
            Destroy(other.gameObject);
        }
    }
}
