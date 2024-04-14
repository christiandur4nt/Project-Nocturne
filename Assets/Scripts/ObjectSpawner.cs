using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject objectToSpawn;
    public float frequencyInSeconds;

    void Reset() {
        frequencyInSeconds = 5f;
    }

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("SpawnObject", 0, frequencyInSeconds);
    }
    
    void SpawnObject() {
        Quaternion randomRotation = Random.rotation;
        Instantiate(objectToSpawn, transform.position, randomRotation);
    }
}
