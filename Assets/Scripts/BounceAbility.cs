using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceAbility : MonoBehaviour
{
    private bool dashing = false;
    private Rigidbody rb;
    public float bounceForce = 20f;
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
        Debug.Log("Dashing: " + dashing);
        Debug.Log("Space Pressed: " + Input.GetKey(bounceKey));
        if (Input.GetKey(bounceKey) && dashing && touchingEnemy)
        {
            BounceUp();
        }
    }

    void BounceUp()
    {
        rb.AddForce(Vector3.up * bounceForce, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            touchingEnemy = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            touchingEnemy = false;
        }
    }
}
