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
    public float animationDuration;
    public float overshootYAxis;
    public float cooldownTime;
    public int grappleMouseKey = 0;
    public LayerMask wallMask;

    // Logic
    private bool isGrappling = false;
    private Vector3 grapplePoint;
    private float cooldownTimer;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(grappleMouseKey)) {
            InitGrapple();
        } else if (Input.GetMouseButtonUp(grappleMouseKey)) {
            StopGrapple();
        }

        Debug.Log(cooldownTimer);
        if (cooldownTimer > 0) cooldownTimer -= Time.deltaTime;
    }
    
    private void LateUpdate() {
        if (isGrappling) {
            grappleRope.SetPosition(0, grappleTip.transform.position);
        }
    }

    void InitGrapple() {
        // Prevent use on cooldown
        if (cooldownTimer > 0) return;

        isGrappling = true;
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, maxGrappleDistance, wallMask)) {
            grapplePoint = hit.point;
            PerformGrapple();
        } else {
            grapplePoint = playerCamera.position + playerCamera.forward*maxGrappleDistance;
        }

        grappleRope.enabled = true;

        StartCoroutine(AnimateLine()); // "Animated" Grapple
        // grappleRope.SetPosition(1, grapplePoint); // Instant Grapple
    }
    
    void PerformGrapple() {

        Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);
        float grapplePointRelativeYPos = grapplePoint.y - lowestPoint.y;
        float highestPointOnArc = grapplePointRelativeYPos + overshootYAxis;

        if (grapplePointRelativeYPos < 0) highestPointOnArc = overshootYAxis;

        pm.JumpToPosition(grapplePoint, highestPointOnArc);
    }

    void StopGrapple() {
        isGrappling = false;
        grappleRope.enabled = false;

        if (cooldownTimer <= 0)
            cooldownTimer = cooldownTime;
    }

    public bool IsGrappling() {
        return isGrappling;
    }

    private IEnumerator AnimateLine() {
        float startTime = Time.time;
        
        Vector3 pos = grappleRope.GetPosition(0);
        while (pos != grapplePoint) {
            float t = (Time.time - startTime) / animationDuration;
            pos = Vector3.Lerp(grappleRope.GetPosition(0), grapplePoint, t);
            grappleRope.SetPosition(1, pos);
            yield return null;
        }
    }
}
