using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceAbility : MonoBehaviour
{
    private bool dashing = false;
    private Rigidbody rb;
    public float bounceForce = 20f;
    public float bounceDuration = 0.25f;
    private bool touchingEnemy = false;

    public KeyCode bounceKey = KeyCode.Space;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        dashing = DashAbility.instance.isDashing;
        if (Input.GetKey(bounceKey) && dashing && touchingEnemy)
        {
            Debug.Log("Can bounce");
            BounceUp();

        }
        else
            touchingEnemy = false;
    }

    void BounceUp()
    {
        rb.AddForce(Vector3.up * bounceForce, ForceMode.VelocityChange);
    }

    IEnumerator EndBounceDelay()
    {
        yield return new WaitForSeconds(bounceDuration);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            touchingEnemy = true;
        }
    }

}
