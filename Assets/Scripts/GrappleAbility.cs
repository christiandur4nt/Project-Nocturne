using System;
using System.Collections;
using UnityEngine;

public class GrappleAbility : MonoBehaviour
{
    [Header("Components")]
    public Transform playerCamera;
    private PlayerMovement playerMovementScript;

    [Header("General Variables")]
    public float maxGrappleDistance;
    public float animationDuration;
    public float cooldownTime;
    public int grappleMouseKey = 0;

    [Header("Zip Variables")]
    public float overshootYAxis;

    [Header("Swing Variables")]
    public float grappleForce;
    public float damper;
    public float massScale;
    [Range(0.1f, 1.0f)]
    public float relativeSwingRadius;

    // Internal
    private LayerMask wallMask;
    private LayerMask barMask;
    private LayerMask grappleableObjects;
    private SpringJoint joint;
    private Vector3 grapplePoint;
    private RaycastHit hit;
    private ArrayList activeIcons;
    private bool isGrappling = false;
    private float cooldownTimer;

    void Reset() {
        cooldownTime = 0.1f;
        animationDuration = 0.1f;
        maxGrappleDistance = 100f;
        grappleForce = 80f;
        relativeSwingRadius = 0.5f;
        damper = 7f;
        massScale = 4.5f;
    }

    void Start() {
        playerMovementScript = GetComponent<PlayerMovement>();
        activeIcons = new();

        string[] layers = {"Grapple Walls", "Monkey Bars", "Enemies"};
        wallMask = LayerMask.GetMask("Grapple Walls");
        barMask = LayerMask.GetMask("Monkey Bars");
        grappleableObjects = LayerMask.GetMask(layers);
    }

    // Update is called once per frame
    void Update()
    {
        // Separate Ray-cast for rendering grapple icon for grapple walls and monkey bars
        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, maxGrappleDistance, wallMask | barMask)) {
            Transform grappleIcon = hit.transform.Find("Hook Icon");
            if (grappleIcon != null) {
                grappleIcon.gameObject.SetActive(true);
                activeIcons.Add(grappleIcon);
            }
        } else {
            foreach (Transform icon in activeIcons) {
                icon.gameObject.SetActive(false);
            }
            activeIcons.Clear();
        }

        if (Input.GetMouseButtonDown(grappleMouseKey)) {
            InitGrapple();
        } else if (Input.GetMouseButtonUp(grappleMouseKey)) {
            StopGrapple();
        }

        if (cooldownTimer > 0) cooldownTimer -= Time.deltaTime;
    }

    void InitGrapple() {
        // Prevent use on cooldown or if grappling is currently active somehow
        if (cooldownTimer > 0 || isGrappling) return;

        isGrappling = true;
        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, maxGrappleDistance, wallMask)) {
            grapplePoint = hit.transform.position;
            PerformGrappleZip();
        } else if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, maxGrappleDistance, barMask)) {
            grapplePoint = hit.transform.position;
            PerformGrappleSwing();
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

    void StopGrapple() {
        isGrappling = false;

        if (joint != null)
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
