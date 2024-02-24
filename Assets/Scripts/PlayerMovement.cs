using System.Collections;
using UnityEditor.Animations;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement instance;
    [SerializeField] private Animator animator;
    public float speed = 5f;
    public LayerMask groundLayer;
    private Rigidbody rb;

    [Header("Movement Variables")]
    public float topSpeed = 5f;
    public float acceleration = 5f;
    public float deceleration = 10f;
    public float jumpForce = 60f;

    // Internal
    private bool isGrounded = true;
    private float currentSpeed = 0f;
    private Vector3 moveDirection = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;
    }
    
    // Update is called once per frame
    void Update()
    {
        float x_input = Input.GetAxis("Horizontal");
        float z_input = Input.GetAxis("Vertical");
        Vector3 inputVector = new Vector3(x_input, 0f, z_input).normalized;

        if (x_input != 0 || z_input != 0)
            animator.SetBool("Movement", true);
        else
            animator.SetBool("Movement", false);
        

        if (inputVector.magnitude > 0)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, topSpeed, acceleration * Time.deltaTime);
            moveDirection = inputVector;
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, deceleration * Time.deltaTime);
        }

        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            Jump();
            isGrounded = false;
        }

        transform.Translate(moveDirection * currentSpeed * Time.deltaTime);
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

    public bool IsGrounded() {
        return isGrounded;
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
