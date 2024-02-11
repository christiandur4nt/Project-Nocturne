using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement instance;
    public float speed = 5f;
    public float jumpForce = 10f;
    public LayerMask groundLayer;
    private Rigidbody rb;
    // Only public to gain access in other scripts
    public bool isGrounded = true;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;
    }
    // Update is called once per frame
    void Update()
    {
        float x_input = Input.GetAxis("Horizontal");
        float z_input = Input.GetAxis("Vertical");

        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            Jump();
            isGrounded = false;
        }

        Vector3 movement = new Vector3(x_input, 0f, z_input) * speed * Time.deltaTime;
        transform.Translate(movement);
    }

    void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
            isGrounded = true;
    }

    private IEnumerator SetVelocity(float waitTime, Vector3 vel)
    {
        yield return new WaitForSeconds(waitTime);
        rb.velocity = vel;
    }

    public Vector3 GetVelocity() {
        return rb.velocity;
    }

    // Grapple Zip Functions //

    public void JumpToPosition(Vector3 targetPosition, float trajectoryHeight)
    {
        Vector3 vel = CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight);
        StartCoroutine(SetVelocity(0.1f, vel));
    }

    // Function from external repository for calculating velocity for grappling
    private Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
    {
        float gravity = Physics.gravity.y;
        float displacementY = endPoint.y - startPoint.y;
        Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);

        // WIP: Velocity can be multiplied according to player's current velocity?
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity) 
            + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));

        return velocityXZ + velocityY;
    }
}
