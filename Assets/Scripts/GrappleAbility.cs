using System;
using System.Collections;
using cakeslice;
using UnityEngine;

public class GrappleAbility : MonoBehaviour
{
    [Header("Components")]
    [HideInInspector] public Transform playerCameraT;
    [SerializeField] private Animator armAnimation;
    private PlayerMovement pm;
    public AudioClip grappleGunSound;
    public AudioClip shootSound;

    [Header("General Variables")]
    [SerializeField] private LayerMask grappleableObjects;
    public float maxGrappleDistance;
    public float cooldownDuration;
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
    private LayerMask runnableWall;
    private LayerMask checkpoint;
    private SpringJoint joint;
    private Vector3 grapplePoint;
    private RaycastHit hit;
    private ArrayList validHits;
    private float cooldownTimer;
    private bool isValidHit;

    void Reset() {
        string[] layers = {"Grapple Zip", "Grapple Swing", "Enemies"};
        grappleableObjects = LayerMask.GetMask(layers);
        grappleMouseKey = 1;
        cooldownDuration = 0.1f;
        maxGrappleDistance = 100f;
        grappleForce = 80f;
        relativeSwingRadius = 0.5f;
        damper = 7f;
        massScale = 4.5f;
    }

    void Awake() {
        gZip = LayerMask.GetMask("Grapple Zip");
        gSwing = LayerMask.GetMask("Grapple Swing");
        runnableWall = LayerMask.GetMask("Runnable");
        checkpoint = LayerMask.GetMask("Checkpoint");
        pm = GetComponent<PlayerMovement>();
        playerCameraT = Camera.main.transform;
        cooldownTimer = 0;
    }

    void Start() {
        validHits = new();
        isValidHit = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!pm.AllowMovement || pm.state == PlayerMovement.MovementState.wallrunning) {
            StartCoroutine(StopGrapple(0));
            return;
        }
        
        // Ray-cast to determine if the object currently being viewed is grappleable
        if (Physics.Raycast(playerCameraT.position, playerCameraT.forward, out hit, maxGrappleDistance, ~(runnableWall | checkpoint))) {

            // For Debugging:
            // Debug.Log(LayerMask.LayerToName(hit.transform.gameObject.layer));

            // Check if hit GameObject layer is a part of the grappleableObjects layermask
            if (((1 << hit.transform.gameObject.layer) & grappleableObjects.value) != 0) {
                isValidHit = true;
                Outline outline = hit.transform.GetComponent<Outline>();
                if (outline != null) {
                    outline.color = 1;
                    validHits.Add(outline);
                } else {
                    Debug.Log("Grappleable object " + hit.transform.gameObject.name + " has no outline script attached!");
                }
            } else {
                ResetOutlines();
                isValidHit = false;
            }
        } else {
            ResetOutlines();
            isValidHit = false;
        }
        
        if (Input.GetMouseButton(grappleMouseKey)) {
            InitGrapple();
        } else if (Input.GetMouseButtonUp(grappleMouseKey)) {
            StartCoroutine(StopGrapple(0));
        }

        Color color = PlayerUIManager.Instance.abilityIcons[(int)PlayerUIManager.Ability.GrappleAbility].color;
        color.a = Mathf.SmoothStep(1f, 0.2f, cooldownTimer/cooldownDuration);
        PlayerUIManager.Instance.abilityIcons[(int)PlayerUIManager.Ability.GrappleAbility].color = color;
        PlayerUIManager.Instance.abilityTimers[(int)PlayerUIManager.Ability.GrappleAbility].SetText(cooldownTimer <= 0 ? "<sprite=1>" : cooldownTimer.ToString("0.0"));

        if (cooldownTimer > 0) cooldownTimer -= Time.deltaTime;
    }

    void InitGrapple() {
        // Prevent use on cooldown or if grappling is currently active somehow
        if (cooldownTimer > 0 || pm.grappling) return;
        if (isValidHit) {
            armAnimation.SetBool("IsGrappling", true);
            pm.grappling = true;
            grapplePoint = hit.point;
            //SoundManager.Instance.PlaySoundClip(shootSound, transform, 1f);
            SoundManager.Instance.PlaySoundClip(grappleGunSound, transform, 1f); 
            if (((1 << hit.transform.gameObject.layer) & gZip.value) != 0) { // Grapple Zip
                PerformGrappleZip();
            } else if (((1 << hit.transform.gameObject.layer) & gSwing.value) != 0) { // Grapple Swing
                PerformGrappleSwing();
            } else { // Default to Grapple Zip for all other layers within grappleableObjects
                PerformGrappleZip();
            }
        } else {
            // Animation no longer plays for grappling an invalid area since pm.grappling=true only if hit is valid
            grapplePoint = playerCameraT.position + playerCameraT.forward*maxGrappleDistance;
        }
    }
    
    void PerformGrappleZip() {
        Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);
        float grapplePointRelativeYPos = grapplePoint.y - lowestPoint.y;
        float highestPointOnArc = grapplePointRelativeYPos + overshootYAxis;

        if (grapplePointRelativeYPos < 0) highestPointOnArc = overshootYAxis;

        pm.JumpToPosition(grapplePoint, highestPointOnArc);
        // StartCoroutine(StopGrapple(0.4f));
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

    IEnumerator StopGrapple(float waitTime) {
        yield return new WaitForSeconds(waitTime);
        if (pm.grappling) {
            pm.grappling = false;
            armAnimation.SetBool("IsGrappling", false);

            if (joint != null)
                Destroy(joint);

            if (cooldownTimer <= 0)
                cooldownTimer = cooldownDuration;
        }
    }

    private void ResetOutlines() {
        foreach (Outline outline in validHits) {
            outline.color = 0;
        }
        validHits.Clear();
    }

    public bool IsGrappling() {
        return pm.grappling;
    }

    public Vector3 GetGrapplePoint() {
        return grapplePoint;
    }
}
