using System.Collections;
//using System.Numerics;
using UnityEditor.Animations;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    [SerializeField] private Animator animator;
    

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Movement")]
    public float walkSpeed;
    public float sprintSpeed;
    public float groundDrag;
    private float speed;
    

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    private bool readyToJump;

    [Header("Crounching")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;

    [Header("Ground Check")]
    public float height;
    public LayerMask groundLayer;
    private bool isGrounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private bool exitingSlope;
    private RaycastHit slopeHit;

    // Internal
    private float xInput;
    private float zInput;
    private Rigidbody rb;
    private Vector3 moveDirection;

    public MovementState state;
    public enum MovementState
    {
        walking,
        sprinting,
        air,
        crouching,
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;
        startYScale = transform.localScale.y;
    }
    
    private void getInput()
    {
        xInput = Input.GetAxisRaw("Horizontal");
        zInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(jumpKey) && readyToJump && isGrounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if (Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }
        if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, height * 0.5f + 0.2f, groundLayer);

        getInput();
        TopSpeed();
        StateHandler();

        if (isGrounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;

        // if (xInput != 0 || zInput != 0)
        //     animator.SetBool("Movement", true);
        // else
        //     animator.SetBool("Movement", false);
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void StateHandler()
    {
        if (Input.GetKey(crouchKey))
        {
            state = MovementState.crouching;
            speed = crouchSpeed;
        }
        else if (isGrounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            speed = sprintSpeed;
        }
        else if (isGrounded)
        {
            state = MovementState.walking;
            speed = walkSpeed;
        }
        else
        {
            state = MovementState.air;
        }
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * zInput + orientation.right * xInput;

        if (OnSlope())
        {
            rb.AddForce(getSlopeMoveDirection() * speed * 20f, ForceMode.Force);

            if (rb.velocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }

        if (isGrounded)
            rb.AddForce(moveDirection.normalized * speed * 10f, ForceMode.Force);
        else if(!isGrounded)
            rb.AddForce(moveDirection.normalized * speed * 10f * airMultiplier, ForceMode.Force);
        
        rb.useGravity = !OnSlope();
    }

    void Jump()
    {
        exitingSlope = true;

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
        exitingSlope = false;
    }

    private void TopSpeed()
    {
        if (OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > speed)
                rb.velocity = rb.velocity.normalized * speed;
        }

        Vector3 rawVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (rawVelocity.magnitude > speed)
        {
            Vector3 limitedVelocity = rawVelocity.normalized * speed;
            rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);
        }
    }

    private bool OnSlope()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, height * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    private Vector3 getSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
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
