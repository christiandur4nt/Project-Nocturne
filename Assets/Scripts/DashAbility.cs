using System;
using System.Collections;
using UnityEngine;

public class DashAbility : MonoBehaviour
{
    [Header("Components")]
    public Transform orientation;
    public Transform playerCamera;
    private PlayerMovement pm;
    private Rigidbody rb;

    [Header("Dash Variables")]
    public float dashForce;
    public float dashUpwardForce;
    public float dashDuration;
    // public float gravityScaleDuringDash;

    [Header("Cooldown")]
    public float cooldownDuration;

    [Header("Keybinds")]
    public int dashButton = 0;

    [Header("Settings")]
    public bool disableGravity;

    // Internal
    private bool isDashing = false;
    private float cooldownTimer = 0;
    private bool onCooldown = false;
    private bool grounded = true;
    [SerializeField] private Animator armAnimation;

    void Start()
    {
        pm = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        grounded = pm.IsGrounded();
        if (Input.GetMouseButtonDown(dashButton))
        {
            Dash();
        }
        // if (!isDashing && grounded)
        //     onCooldown = false;

        if (cooldownTimer > 0)
            cooldownTimer -= Time.deltaTime;
    }

    void Dash()
    {
        if (cooldownTimer > 0) return;
        else cooldownTimer = cooldownDuration;

        if (disableGravity)
            rb.useGravity = false;

        pm.dashing = true;

        Vector3 forceToApply = orientation.forward * dashForce + orientation.up * dashUpwardForce;

        rb.AddForce(forceToApply, ForceMode.Impulse);
        delayedForceToApply = forceToApply;

        Invoke(nameof(DelayedDashForce), 0.025f);
        Invoke(nameof(ResetDash), dashDuration);

        // isDashing = true;
        // rb.useGravity = false;
        // armAnimation.SetBool("Is Dashing", true);

    }

    private Vector3 delayedForceToApply;
    private void DelayedDashForce()
    {
        rb.AddForce(delayedForceToApply, ForceMode.Impulse);
    }

    void ResetDash()
    {
        pm.dashing = false;
        if (disableGravity)
            rb.useGravity = true;
    }

    IEnumerator EndDashAfterDelay()
    {
        yield return new WaitForSeconds(dashDuration);

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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy" && isDashing)
            onCooldown = false;
    }

    public bool IsDashing() {
        return isDashing;
    }
}
