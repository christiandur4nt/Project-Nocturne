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
    [SerializeField] private LayerMask grappleableObjects;
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
    private LayerMask gZip;
    private LayerMask gSwing;
    private SpringJoint joint;
    private Vector3 grapplePoint;
    private RaycastHit hit;
    private ArrayList activeIcons;
    private float cooldownTimer;
    private bool isValidHit;

    void Reset() {
        string[] layers = {"Grapple Zip", "Grapple Swing", "Enemies"};
        grappleableObjects = LayerMask.GetMask(layers);
        grappleMouseKey = 1;
        cooldownTime = 0.1f;
        animationDuration = 0.1f;
        maxGrappleDistance = 100f;
        grappleForce = 80f;
        relativeSwingRadius = 0.5f;
        damper = 7f;
        massScale = 4.5f;
    }

    void Awake() {
        gZip = LayerMask.GetMask("Grapple Zip");
        gSwing = LayerMask.GetMask("Grapple Swing");
        pm = GetComponent<PlayerMovement>();
        playerCameraT = Camera.main.transform;
    }

    void Start() {
        activeIcons = new();
        isValidHit = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Ray-cast to determine if the object currently being viewed is grappleable
        if (Physics.Raycast(playerCameraT.position, playerCameraT.forward, out hit, maxGrappleDistance)) {
            // Check if hit GameObject layer is a part of the grappleableObjects layermask
            if (((1 << hit.transform.gameObject.layer) & grappleableObjects.value) != 0) {
                isValidHit = true;
                Transform grappleIcon = hit.transform.Find("Icon Joint").Find("Hook Icon");
                if (grappleIcon != null) {
                    grappleIcon.gameObject.SetActive(true);
                    activeIcons.Add(grappleIcon);
                } else {
                    Debug.Log("Grappleable object " + hit.transform.name + " does not have an icon!");
                }
            } else {
                isValidHit = false;
                ClearIcons();
            }
        } else {
            isValidHit = false;
            ClearIcons();
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
        // armAnimation.SetBool("IsGrappling", true);

        if (isValidHit) {
            pm.grappling = true;
            grapplePoint = hit.point;
            if (((1 << hit.transform.gameObject.layer) & gZip.value) != 0) { // Grapple Zip
                PerformGrappleZip();
            } else if (((1 << hit.transform.gameObject.layer) & gSwing.value) != 0) { // Grapple Swing
                PerformGrappleSwing();
            } else { // Default to Grapple Zip for all other layers within grappleableObjects
                PerformGrappleZip();
            }
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

        // The distance grapple will try to keep from grapple point
        joint.maxDistance = distanceFromPoint * relativeSwingRadius;
        joint.minDistance = 0;

        // Spring adjustments
        joint.spring = grappleForce;
        joint.damper = damper;
        joint.massScale = massScale;
    }

    void StopGrapple() {
        pm.grappling = false;
        // armAnimation.SetBool("IsGrappling", false);

        if (joint != null)
            Destroy(joint);

        if (cooldownTimer <= 0)
            cooldownTimer = cooldownTime;
    }

    private void ClearIcons() {
        foreach (Transform icon in activeIcons) {
            icon.gameObject.SetActive(false);
        }
        activeIcons.Clear();
    }

    public bool IsGrappling() {
        return pm.grappling;
    }

    public Vector3 GetGrapplePoint() {
        return grapplePoint;
    }
}
