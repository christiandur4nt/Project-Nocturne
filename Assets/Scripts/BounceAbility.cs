using System.Collections;
using UnityEngine;

public class BounceAbility : MonoBehaviour
{
    [Header("Bounce Variables")]
    public float bounceForce = 20f;
    public float launchForce = 10f;
    public float upwardForce = 2f;
    public float bounceDuration = 0.25f;
    public KeyCode bounceKey = KeyCode.Space;

    // Internal
    private bool dashing = false;
    private DashAbility dashAbilityScript;
    private Rigidbody rb;
    private bool touchingEnemy = false;


    // Start is called before the first frame update
    void Start()
    {
        dashAbilityScript = GetComponent<DashAbility>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        dashing = dashAbilityScript.IsDashing();
        if (dashing && touchingEnemy && Input.GetKey(bounceKey))
        {
            Debug.Log(touchingEnemy);
            BounceUp();
        }
        else if (dashing && touchingEnemy)
        {
            LaunchForward();
        }
        else
            touchingEnemy = false;
    }

    void BounceUp()
    {
        rb.AddForce(Vector3.up * bounceForce, ForceMode.VelocityChange);
    }

    void LaunchForward()
    {
        Vector3 launchDirection = transform.forward * launchForce;
        launchDirection += Vector3.up * upwardForce;
        launchDirection.Normalize();
        rb.velocity = Vector3.zero;
        rb.AddForce(launchDirection, ForceMode.VelocityChange);
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
