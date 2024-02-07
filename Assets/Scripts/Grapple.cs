using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapple : MonoBehaviour
{
    [Header("Component References")]
    public Transform playerCamera;
    public Transform grappleTip;
    public LineRenderer grappleRope;
    public PlayerMovement pm;

    [Header("Variables")]
    public float maxGrappleDistance;
    public float overshootYAxis;
    public int grappleMouseKey = 0;
    public LayerMask wallMask;

    // Logic
    private bool isGrappling = false;
    private Vector3 grapplePoint;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(grappleMouseKey)) {
            InitGrapple();
        }
    }
    
    private void LateUpdate() {
        if (isGrappling) {
            grappleRope.SetPosition(0, new Vector3());
            grappleRope.SetPosition(1, grappleTip.InverseTransformPoint(grapplePoint));
        }
    }

    // Functions for grappling
    void InitGrapple() {
        isGrappling = true;
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, maxGrappleDistance, wallMask)) {
            grapplePoint = hit.point;
            PerformGrapple();
        } else {
            grapplePoint = playerCamera.position + playerCamera.forward*maxGrappleDistance;
            Invoke(nameof(StopGrapple), 1f);
        }

        grappleRope.enabled = true;
        grappleRope.SetPosition(1, grappleTip.InverseTransformPoint(grapplePoint));
    }

    void PerformGrapple() {

        Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);
        float grapplePointRelativeYPos = grapplePoint.y - lowestPoint.y;
        float highestPointOnArc = grapplePointRelativeYPos + overshootYAxis;

        if (grapplePointRelativeYPos < 0) highestPointOnArc = overshootYAxis;

        pm.JumpToPosition(grapplePoint, highestPointOnArc);

        Invoke(nameof(StopGrapple), 1f);
    }

    void StopGrapple() {
        isGrappling = false;
        grappleRope.enabled = false;
    }
}
