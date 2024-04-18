using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class resetPlayerPosition : MonoBehaviour
{
    private Vector3 startingPosition;

    void Start()
    {
        startingPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.position = startingPosition;
        }
    }
}
