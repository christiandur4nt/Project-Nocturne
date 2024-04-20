using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoRespawn : MonoBehaviour
{
    public GameObject[] enemies;
    public bool respawn = false;
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("RESPAWN NOW!!!");
            foreach (GameObject enemy in enemies)
            {
                enemy.GetComponent<EnemyKill>().Respawn();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("RESPAWN NOW!!!");
            foreach (GameObject enemy in enemies)
            {
                enemy.GetComponent<EnemyKill>().Respawn();
            }
        }
    }
}
