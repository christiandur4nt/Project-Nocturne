using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleAbility : MonoBehaviour
{
    [Header("Components")]
    public Transform playerCamera;
    public Transform grappleTip;
    private PlayerMovement playerMovementScript;

    [Header("General Variables")]
    public LayerMask wallMask;
    public float maxGrappleDistance;
    public float animationDuration;
    public float cooldownTime;
    public int grappleZipMouseKey = 0;
    public int grappleSwingMouseKey = 1;

    [Header("Zip Variables")]
    public float overshootYAxis;

    [Header("Swing Variables")]
    public float grappleForce;
    public float damper;
    public float massScale;
    [Range(0.1f, 1.0f)]
    public float relativeSwingRadius;

    // Internal
    private SpringJoint joint;
    private Vector3 grapplePoint;
    private bool isGrappling = false;
    private float cooldownTimer;
    private enum GrappleType {ZIP, SWING};

    void Reset() {
        cooldownTime = 0.1f;
        animationDuration = 0.1f;
        maxGrappleDistance = 100f;
        grappleForce = 80f;
        relativeSwingRadius = 0.5f;
        damper = 7f;
        massScale = 4.5f;
        string[] defaultLayers = {"GrappleWalls", "Enemies"};
        wallMask = LayerMask.GetMask(defaultLayers);
    }

    void Start() {
        playerMovementScript = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(grappleZipMouseKey)) {
            InitGrapple(GrappleType.ZIP);
        } else if (Input.GetMouseButtonDown(grappleSwingMouseKey)) {
            InitGrapple(GrappleType.SWING);
        } else if (joint == null && Input.GetMouseButtonUp(grappleZipMouseKey)) {
            StopGrapple(GrappleType.ZIP);
        } else if (Input.GetMouseButtonUp(grappleSwingMouseKey)) {
            StopGrapple(GrappleType.SWING);
        }

        if (cooldownTimer > 0) cooldownTimer -= Time.deltaTime;
    }

    void InitGrapple(GrappleType type) {
        // Prevent use on cooldown or if other GrappleType is currently active
        if (cooldownTimer > 0 || isGrappling) return;

        isGrappling = true;
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, maxGrappleDistance, wallMask)) {
            grapplePoint = hit.point;
            switch (type) {
                case GrappleType.ZIP:
                    PerformGrappleZip();
                    break;
                case GrappleType.SWING:
                    PerformGrappleSwing();
                    break;
            }
        } else {
            grapplePoint = playerCamera.position + playerCamera.forward*maxGrappleDistance;
        }
    }
    
    void PerformGrappleZip() {

        Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);
        float grapplePointRelativeYPos = grapplePoint.y - lowestPoint.y;
        float highestPointOnArc = grapplePointRelativeYPos + overshootYAxis;

        if (grapplePointRelativeYPos < 0) highestPointOnArc = overshootYAxis;

        playerMovementScript.JumpToPosition(grapplePoint, highestPointOnArc);
    }

    void PerformGrappleSwing() {
        joint = gameObject.AddComponent<SpringJoint>();
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = grapplePoint;

        float distanceFromPoint = Vector3.Distance(transform.position, grapplePoint);

        //The distance grapple will try to keep from grapple point. 
        joint.maxDistance = distanceFromPoint * relativeSwingRadius;
        joint.minDistance = 0;

        //Adjust these values to fit your game.
        joint.spring = grappleForce;
        joint.damper = damper;
        joint.massScale = massScale;
    }

    void StopGrapple(GrappleType type) {
        isGrappling = false;

        if (type == GrappleType.SWING)
            Destroy(joint);

        if (cooldownTimer <= 0)
            cooldownTimer = cooldownTime;
    }

    public bool IsGrappling() {
        return isGrappling;
    }

    public Vector3 GetGrapplePoint() {
        return grapplePoint;
    }

    // Old simple animation
    // private IEnumerator AnimateLine() {
    //     float startTime = Time.time;
        
    //     Vector3 pos = grappleRope.GetPosition(0);
    //     while (pos != grapplePoint) {
    //         float t = (Time.time - startTime) / animationDuration;
    //         pos = Vector3.Lerp(grappleRope.GetPosition(0), grapplePoint, t);
    //         grappleRope.SetPosition(1, pos);
    //         yield return null;
    //     }
    // }
}
