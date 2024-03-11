using System;
using System.Collections;
using UnityEngine;

public class DashAbility : MonoBehaviour
{
    [Header("Components")]
    [HideInInspector] public Transform orientation;
    [HideInInspector] public Transform playerCamera;
    private PlayerMovement pm;
    private Rigidbody rb;

    [Header("Dash Variables")]
    public float dashForce;
    public float dashUpwardForce;
    public float dashDuration;
    public float maxDashYSpeed;

    [Header("Camera Effects")]
    [HideInInspector] public CameraMovement cam;
    public float dashFOV;

    [Header("Cooldown")]
    public float cooldownDuration;

    [Header("Keybinds")]
    public int dashButton = 0;

    [Header("Settings")]
    public bool useCameraForward = false;
    public bool allowAllDirections = true;
    public bool resetVelocity = true;
    public bool disableGravity = true;


    // Internal
    public bool onCooldown = false;
    private float cooldownTimer = 0;
    private bool grounded = true;
    [SerializeField] private Animator armAnimation;

    void Awake() {
        orientation = GameObject.Find("Orientation").transform;
        playerCamera = GameObject.Find("Player Camera").transform;
        cam = FindObjectOfType<CameraMovement>();
    }

    void Start()
    {
        pm = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!pm.MovementEnabled()) return;

        grounded = pm.IsGrounded();
        if (Input.GetMouseButtonDown(dashButton) && !onCooldown)
        {
            Dash();
            onCooldown = true;
        }

        if (pm.IsGrounded())
            onCooldown = false;

        if (cooldownTimer > 0)
            cooldownTimer -= Time.deltaTime;
    }

    void Dash()
    {
        if (cooldownTimer > 0) return;
        else if (pm.IsGrounded()) cooldownTimer = cooldownDuration;

        pm.dashing = true;
        pm.maxYSpeed = maxDashYSpeed;

        cam.doFOV(dashFOV);

        Transform forwardT;

        if (useCameraForward)
            forwardT = playerCamera;
        else
            forwardT = orientation;

        Vector3 direction = getDirection(forwardT);

        Vector3 forceToApply = direction * dashForce + orientation.up * dashUpwardForce;
        
        rb.useGravity = false;

        rb.AddForce(forceToApply, ForceMode.Impulse);
        delayedForceToApply = forceToApply;

        Invoke(nameof(DelayedDashForce), 0.025f);
        Invoke(nameof(ResetDash), dashDuration);


        armAnimation.SetBool("Is Dashing", true);

    }

    private Vector3 delayedForceToApply;
    private void DelayedDashForce()
    {
        if(resetVelocity)
            rb.velocity = Vector3.zero;

        rb.AddForce(delayedForceToApply, ForceMode.Impulse);
    }

    void ResetDash()
    {
        pm.dashing = false;
        pm.maxYSpeed = 0;
        armAnimation.SetBool("Is Dashing", false);

        cam.doFOV(60f);

        rb.useGravity = true;
    }

    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(cooldownDuration);
        onCooldown = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy" && pm.dashing)
            onCooldown = false;
    }

    private Vector3 getDirection(Transform forwardT)
    {
        float xInput = Input.GetAxisRaw("Horizontal");
        float zInput = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3();

        if (allowAllDirections)
            direction = forwardT.forward * zInput + forwardT.right * xInput;
        else
            direction = forwardT.forward;
        
        if (zInput == 0 && xInput == 0)
            direction = forwardT.forward;

        return direction.normalized;
    }

    public bool IsDashing() {
        return pm.dashing;
    }
}
