using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashAbility : MonoBehaviour
{
    public float dashForce = 10f;
    public float dashDuration = 0.5f;
    public float gravityScaleDuringDash = 0;

    public KeyCode dashKey = KeyCode.LeftShift;
    public float cooldownDuration = 0.5f;
    private bool isDashing = false;
    private bool onCooldown = false;
    private bool grounded = true;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        grounded = PlayerMovement.instance.isGrounded;

        if (Input.GetKeyDown(dashKey) && !isDashing && !onCooldown)
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

        Vector3 dashDirection = transform.forward;

        rb.AddForce(dashDirection * dashForce, ForceMode.Impulse);

        StartCoroutine(EndDashAfterDelay());
        
        
    }

    IEnumerator EndDashAfterDelay()
    {
        yield return new WaitForSeconds(dashDuration);

        rb.useGravity = true;
        isDashing = false;
        if (grounded)
            StartCoroutine(Cooldown());
    }

    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(cooldownDuration);
        onCooldown = false;
    }
}
