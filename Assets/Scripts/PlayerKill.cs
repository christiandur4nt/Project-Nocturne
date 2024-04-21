using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKill : MonoBehaviour
{
    private AudioSource killSFX;

    void Awake() {
        killSFX = GetComponent<AudioSource>();
    }

    void OnCollisionEnter(Collision collisionInfo)
    {
        if (collisionInfo.collider.CompareTag("Player") && !PlayerManager.isDead) {
            if (killSFX != null) killSFX.Play();
            PlayerUIManager.Instance.Die();
        }
    }
}
