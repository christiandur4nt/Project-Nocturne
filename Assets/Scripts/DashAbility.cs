using System;
using System.Collections;
using UnityEngine;

public class DashAbility : MonoBehaviour
{
    [Header("Components")]
    public Camera playerCamera;
    private PlayerMovement playerMovementScript;
    private Rigidbody rb;

    [Header("Dash Variables")]
    public float dashFOV = 90f;
    public float fovTransitionDuration = 0.3f;
    public float dashForce = 10f;
    public float dashDuration = 0.5f;
    public float gravityScaleDuringDash = 0;
    public int dashButton = 0;
    public float cooldownDuration = 0.5f;

    // Internal
    private float transitionTimer;
    private float originalFOV;
    private bool isDashing = false;
    private bool onCooldown = false;
    private bool grounded = true;
    [SerializeField] private Animator armAnimation;

    void Reset() {
        playerCamera = Camera.main;
    }

    void Start()
    {
        playerMovementScript = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody>();
        originalFOV = playerCamera.fieldOfView;
    }

    // Update is called once per frame
    void Update()
    {
        grounded = playerMovementScript.IsGrounded();
        if (Input.GetMouseButtonDown(dashButton) && !isDashing && !onCooldown)
        {
            StartDash();
            if (!grounded)
                onCooldown = true;
        }
        if (!isDashing && grounded)
            onCooldown = false;
    }

    void StartDash()
    {
        isDashing = true;
        rb.useGravity = false;
        rb.velocity = Vector3.zero;
        armAnimation.SetBool("Is Dashing", true);
        Vector3 dashDirection = transform.forward;

        rb.AddForce(dashDirection * dashForce, ForceMode.VelocityChange);

        StartCoroutine(EndDashAfterDelay());
    }

    IEnumerator EndDashAfterDelay()
    {
        yield return new WaitForSeconds(dashDuration);

        resetFOV();
        rb.useGravity = true;
        isDashing = false;
        armAnimation.SetBool("Is Dashing", false);
        if (grounded)
            StartCoroutine(Cooldown());
    }

    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(cooldownDuration);
        onCooldown = false;
    }

    private void resetFOV()
    {
        playerCamera.fieldOfView = originalFOV;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy" && isDashing)
            onCooldown = false;
    }

    public bool IsDashing() {
        return isDashing;
    }
}
