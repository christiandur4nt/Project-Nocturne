using System.Collections;
using UnityEngine;

public class BounceAbility : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    private PlayerMovement pm;
    private Rigidbody rb;


    [Header("Bounce Variables")]
    public float bounceForce;
    public float launchForce;
    public float upwardForce;
    public float bounceDuration;

    [Header("Keybinds")]
    public KeyCode bounceKey = KeyCode.Space;

    [Header("Settings")]
    public bool resetVelocity = false;

    // Internal
    private bool dashing = false;
    private bool touchingEnemy = false;


    // Start is called before the first frame update
    void Start()
    {
        pm = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        dashing = pm.dashing;

        if (dashing && touchingEnemy && Input.GetKey(bounceKey))
        {
            BounceUp();
        }
        else if (dashing && touchingEnemy)
        {
            LaunchForward();
        }
        else
            touchingEnemy = false;
    }

    private void BounceUp()
    {
        if (resetVelocity)
            rb.velocity = Vector3.zero;
        rb.AddForce(orientation.up * bounceForce, ForceMode.Impulse);
    }

    private void LaunchForward()
    {
        if (resetVelocity)
            rb.velocity = Vector3.zero;

        Vector3 launchDirection = orientation.forward * launchForce;
        launchDirection += orientation.up * upwardForce;
        launchDirection.Normalize();
        
        rb.AddForce(launchDirection, ForceMode.Impulse);
    }

    private void ResetBounce()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            touchingEnemy = true;
        }
    }

}
