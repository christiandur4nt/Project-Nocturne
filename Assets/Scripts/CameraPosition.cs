using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPosition : MonoBehaviour
{
    [Header("Reference")]
    [HideInInspector] public Transform cameraPos;

    void Awake() {
        cameraPos = GameObject.Find("CameraPos").transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = cameraPos.position;
    }
}
