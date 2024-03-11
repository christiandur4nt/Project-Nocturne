using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoRespawn : MonoBehaviour
{
    public bool respawn = false;
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            respawn = true;
        }
    }
}
