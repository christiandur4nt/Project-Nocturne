using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class EnemyKillParent : MonoBehaviour
{
    public void CollisionDetected(EnemyKill enemyKill)
    {
        
        Destroy(gameObject);
    }
}
