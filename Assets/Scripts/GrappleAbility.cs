using System;
using System.Collections;
using UnityEngine;

public class GrappleAbility : MonoBehaviour
{
    [Header("Components")]
    [HideInInspector] public Transform playerCameraT;
    [SerializeField] private Animator armAnimation;
    private PlayerMovement pm;

    [Header("General Variables")]
    public float maxGrappleDistance;
    public float animationDuration;
    public float cooldownTime;
    public int grappleMouseKey;

    [Header("Zip Variables")]
    public float overshootYAxis;

    [Header("Swing Variables")]
    public float grappleForce;
    public float damper;
    public float massScale;
    [Range(0.1f, 1.0f)]
    public float relativeSwingRadius;

    // Internal
    [SerializeField] private LayerMask gZip; // WIP: gZip
    [SerializeField] private LayerMask gSwing; // WIP: gSwing
    private LayerMask grappleableObjects;
    private SpringJoint joint;
    private Vector3 grapplePoint;
    private RaycastHit hit;
    private ArrayList activeIcons;
    private float cooldownTimer;

    void Reset() {
        grappleMouseKey = 1;
        cooldownTime = 0.1f;
        animationDuration = 0.1f;
        maxGrappleDistance = 100f;
        grappleForce = 80f;
        relativeSwingRadius = 0.5f;
        damper = 7f;
        massScale = 4.5f;
    }

    void Start() {
        pm = GetComponent<PlayerMovement>();
        playerCameraT = Camera.main.transform;
        activeIcons = new();

        string[] layers = {"Grapple Zip", "Grapple Swing", "Enemies"};
        gZip = LayerMask.GetMask("Grapple Zip");
        gSwing = LayerMask.GetMask("Grapple Swing");
        grappleableObjects = LayerMask.GetMask(layers);
    }

    // Update is called once per frame
    void Update()
    {
        // Separate Ray-cast for rendering grapple icon for grappleable objects
        if (Physics.Raycast(playerCameraT.position, playerCameraT.forward, out hit, maxGrappleDistance, gZip | gSwing)) {
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
        if (cooldownTimer > 0 || pm.grappling) return;

        pm.grappling = true;
        armAnimation.SetBool("IsGrappling", true);
        if (Physics.Raycast(playerCameraT.position, playerCameraT.forward, out hit, maxGrappleDistance, gZip)) {
            grapplePoint = hit.transform.position;
            PerformGrappleZip();
        } else if (Physics.Raycast(playerCameraT.position, playerCameraT.forward, out hit, maxGrappleDistance, gSwing)) {
            grapplePoint = hit.transform.position;
            PerformGrappleSwing();
        } else {
            grapplePoint = playerCameraT.position + playerCameraT.forward*maxGrappleDistance;
        }
    }
    
    void PerformGrappleZip() {

        Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);
        float grapplePointRelativeYPos = grapplePoint.y - lowestPoint.y;
        float highestPointOnArc = grapplePointRelativeYPos + overshootYAxis;

        if (grapplePointRelativeYPos < 0) highestPointOnArc = overshootYAxis;

        pm.JumpToPosition(grapplePoint, highestPointOnArc);
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
        pm.grappling = false;
        armAnimation.SetBool("IsGrappling", false);

        if (joint != null)
            Destroy(joint);

        if (cooldownTimer <= 0)
            cooldownTimer = cooldownTime;
    }

    public bool IsGrappling() {
        return pm.grappling;
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
