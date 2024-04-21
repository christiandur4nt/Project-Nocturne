using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKill : MonoBehaviour
{
    public AudioClip[] killSFX;

    void OnCollisionEnter(Collision collisionInfo)
    {
        if (collisionInfo.collider.CompareTag("Player") && !PlayerManager.isDead)
        {
            if (killSFX.Length != 0)
            {
                foreach (AudioClip SFX in killSFX)
                    SoundManager.Instance.PlaySoundClip(SFX, transform, 100f);
            }
            PlayerUIManager.Instance.Die();
        }
    }
}
