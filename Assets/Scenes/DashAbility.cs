using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashAbility : MonoBehaviour
{
    public float dashForce = 10f;
    public float dashDuration = 0.5f;
    public float gravityScaleDuringDash = 0;

    public KeyCode dashKey = KeyCode.LeftShift;
    private bool isDashing = false;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(dashKey) && !isDashing)
        {
            StartDash();
        }
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
    }
}
