using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKill : MonoBehaviour
{
    void OnCollisionEnter(Collision collisionInfo)
    {
        if (!PlayerManager.isDead) {
            PlayerUIManager.Instance.Die();
        }
    }
}
