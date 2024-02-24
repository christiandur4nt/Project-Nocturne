using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class FirstPassout : MonoBehaviour
{
    public float force = 10f;
    public Boolean passingOut = false;
    private Collider player;

    public PostProcessProfile brightness;
    public PostProcessLayer layer;

    AutoExposure exposure;

    private void OnTriggerEnter(Collider other)
    {

        passingOut = true;
        other.GetComponent<PlayerMovement>().enabled = false;
        other.transform.Find("Player Camera").GetComponent<CameraMovement>().enabled = false;
        player = other;

        brightness.TryGetSettings(out exposure);

        print("Player is passing out...");
    }

    private void FixedUpdate()
    {
        if (passingOut)
        {
            player.GetComponent<Rigidbody>().AddForce(Vector3.right * -force, ForceMode.VelocityChange);

            while(exposure.keyValue >= 0)
            {
                exposure.keyValue.value -= 0.1f;
            }

            
        }
    }

    private void swayLeft() { 
       
    }
}
