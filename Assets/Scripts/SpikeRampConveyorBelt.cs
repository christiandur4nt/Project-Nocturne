using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeRampConveyorBelt : MonoBehaviour
{
    public Transform endpoint;
    public float speed;

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && !PlayerManager.isDead)
        {
            Vector3 newPos = Vector3.MoveTowards(other.transform.position, endpoint.position, speed * Time.deltaTime);
            other.transform.position = newPos;
        }
    }
}
