using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float x = gameObject.transform.rotation.x;
        float y = gameObject.transform.rotation.y; 
        float z = gameObject.transform.position.z;
        gameObject.transform.Rotate(0, 0, 0.1f);
    }

    private void OnTriggerEnter(Collider other)
    {
        gameObject.SetActive(false);
    }
}
