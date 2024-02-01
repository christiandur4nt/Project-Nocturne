using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceAbility : MonoBehaviour
{
    private bool dashing = false;
    private Rigidbody rb;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        dashing = DashAbility.instance.isDashing;
    }
}
