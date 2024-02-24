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
    public float gravityScaleDuringDash;

    [Header("Cooldown")]
    public float cooldownDuration = 10f;

    [Header("Input")]
    public int dashButton = 0;

    // Internal
    private float transitionTimer;
    private float originalFOV;
    private bool isDashing = false;
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
        if (Input.GetMouseButtonDown(dashButton) && !isDashing && !onCooldown)
        {
            Dash();
            if (!grounded)
                onCooldown = true;
        }
        if (!isDashing && grounded)
            onCooldown = false;
    }

    void Dash()
    {
        Vector3 forceToApply = orientation.forward * dashForce + orientation.up * dashUpwardForce;

        isDashing = true;
        rb.useGravity = false;
        rb.velocity = Vector3.zero;
        armAnimation.SetBool("Is Dashing", true);
        Vector3 dashDirection = transform.forward;

        rb.AddForce(forceToApply, ForceMode.Impulse);

        Invoke(nameof(ResetDash), dashDuration);
        //StartCoroutine(EndDashAfterDelay());
    }

    void ResetDash()
    {

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
