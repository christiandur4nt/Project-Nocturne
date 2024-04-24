using System.Collections;

//using System.Numerics;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [HideInInspector] public Transform orientation;
    [SerializeField] private Animator animator;
    private Rigidbody rb;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Movement Speed Controls")]
    public float walkSpeed;
    public float grappleLaunchSpeed;
    public float sprintSpeed;
    public float dashSpeed;
    public float wallRunSpeed;
    public float dashSpeedChangeFactor;
    public float groundDrag;
    public float maxYSpeed;
    public float diveAcceleration;
    private float speed;
    
    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    private bool readyToJump;
    public AudioClip landingSound;

    [Header("Crouching")]
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

    // Internal variables for input/movement
    private bool allowMovement;
    public bool AllowMovement { get { return allowMovement; } }
    private float xInput;
    private float zInput;
    private bool crouchKeyActive, crouchKeyDown, crouchKeyUp;
    private bool jumpKeyActive;
    private bool sprintKeyActive;
    private Vector3 moveDirection;

    public MovementState state;
    public enum MovementState
    {
        walking,
        sprinting,
        air,
        crouching,
        dashing,
        bouncing,
        grappling,
        wallrunning,
    }

    // Ability Bools: Manipulated in their respective scripts
    public bool dashing;
    public bool bouncing;
    public bool grappling;
    public bool wallrunning;

    void Awake() {
        allowMovement = true;
        orientation = GameObject.Find("Orientation").transform;
        rb = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        rb.freezeRotation = true;
        readyToJump = true;
        startYScale = transform.localScale.y;
    }
    
    private void getInput()
    {
        xInput = allowMovement ? Input.GetAxisRaw("Horizontal") : 0;
        zInput = allowMovement ? Input.GetAxisRaw("Vertical") : 0;

        // Handle crouch inputs
        if (crouchKeyDown = Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
        }
        if (crouchKeyUp = Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }
        crouchKeyActive = Input.GetKey(crouchKey);
        jumpKeyActive = Input.GetKey(jumpKey);
        sprintKeyActive = Input.GetKey(sprintKey);
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log("State: " + state);
        isGrounded = Physics.Raycast(transform.position, Vector3.down, height * 0.5f + 0.2f, groundLayer);

        getInput();
        StateHandler();

        if (state == MovementState.walking || state == MovementState.sprinting || state == MovementState.crouching)
            rb.drag = groundDrag;
        else
            rb.drag = 0;

        if (xInput != 0 || zInput != 0)
            animator.SetBool("Movement", true);
        else
            animator.SetBool("Movement", false);
    }

    private void FixedUpdate()
    {
        MovePlayer();
        if (!grappling) TopSpeed();
    }

    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;
    private MovementState lastState;
    private bool keepMomentum;

    private void StateHandler()
    {
        if (dashing)
        {
            state = MovementState.dashing;
            desiredMoveSpeed = dashSpeed;
            speedChangeFactor = dashSpeedChangeFactor;
        } 

        if (bouncing)
        {
            state = MovementState.bouncing;
        }
        
        if (wallrunning)
        {
            state = MovementState.wallrunning;
            desiredMoveSpeed = wallRunSpeed;
        }
        else if (grappling) {
            state = MovementState.grappling;
        }
        else if (state != MovementState.air && state != MovementState.wallrunning && crouchKeyActive)
        {
            state = MovementState.crouching;
            desiredMoveSpeed = crouchSpeed;
        }
        else if (isGrounded && sprintKeyActive && !dashing)
        {
            state = MovementState.sprinting;
            desiredMoveSpeed = sprintSpeed;
        }
        else if (isGrounded && !dashing)
        {
            state = MovementState.walking;
            desiredMoveSpeed = walkSpeed;
        }
        else
        {
            state = MovementState.air;

            if (lastState == MovementState.grappling)
                desiredMoveSpeed = grappleLaunchSpeed;
            else if (desiredMoveSpeed < sprintSpeed)
                desiredMoveSpeed = walkSpeed;
            else if (desiredMoveSpeed != grappleLaunchSpeed)
                desiredMoveSpeed = sprintSpeed;

            // Previous Logic:
            // state = MovementState.air;
            // if (desiredMoveSpeed < sprintSpeed)
            //     desiredMoveSpeed = walkSpeed;
            // else
            //     desiredMoveSpeed = sprintSpeed;
        }

        bool desiredSpeedHasChanged = desiredMoveSpeed != lastDesiredMoveSpeed;
        if (lastState == MovementState.dashing) keepMomentum = true;
        // if (lastState == MovementState.air && state == MovementState.walking || state == MovementState.sprinting || state == MovementState.crouching)
        //     SoundManager.Instance.PlaySoundClip(landingSound, transform, 1f);

        if (desiredSpeedHasChanged)
        {
            if (keepMomentum)
            {
                StopAllCoroutines();
                StartCoroutine(SmoothMoveSpeed());
            }
            else
            {
                StopAllCoroutines();
                speed = desiredMoveSpeed;
            }
        }

        lastDesiredMoveSpeed = desiredMoveSpeed;
        lastState = state;
    }

    private void MovePlayer()
    {
        if (jumpKeyActive && readyToJump && isGrounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        // Divebomb
        if (state == MovementState.air && crouchKeyActive) {
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y + (-diveAcceleration*Time.fixedDeltaTime), rb.velocity.z);
        }

        moveDirection = orientation.forward * zInput + orientation.right * xInput;

        if (OnSlope())
        {
            rb.AddForce(getSlopeMoveDirection() * speed * Time.deltaTime, ForceMode.Force);

            if (rb.velocity.y > 0)
                rb.AddForce(Vector3.down * 80f * Time.deltaTime, ForceMode.Force);
        }
        
        if (dashing)
            rb.drag = 0;

        if (isGrounded && !dashing)
            rb.AddForce(moveDirection.normalized * speed * 100f * Time.deltaTime, ForceMode.Force);
        else if(!grappling)
            rb.AddForce(moveDirection.normalized * speed * 100f * airMultiplier * Time.deltaTime, ForceMode.Force);
        
        if (!dashing && !wallrunning)     
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
        else
        {
            Vector3 rawVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            if (rawVelocity.magnitude > speed)
            {
                Debug.Log("Capping speed");
                Vector3 limitedVelocity = rawVelocity.normalized * speed * 0.5f; //TEMP FIX POTENTIALLY
                rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);
            }
        }

        if (state != MovementState.air && maxYSpeed != 0 && rb.velocity.y > maxYSpeed)
        {
            rb.velocity = new Vector3(rb.velocity.x, maxYSpeed, rb.velocity.z);
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

    public void EnableMovement() {
        allowMovement = true;
    }

    public void DisableMovement() {
        allowMovement = false;
    }

    public void FreezePlayer() {
        rb.isKinematic = true;
    }

    public void UnfreezePlayer() {
        rb.isKinematic = false;
    }

    // Grapple Functions //

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

        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity) 
            + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));

        return velocityXZ + velocityY;
    }

    // End Grapple Functions //

    private float speedChangeFactor;
    private IEnumerator SmoothMoveSpeed()
    {
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - speed);
        float startValue = speed;

        float boostFactor = speedChangeFactor;

        while (time < difference)
        {
            speed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);

            time += Time.deltaTime * boostFactor;

            yield return null;
        }

        speed = desiredMoveSpeed;
        speedChangeFactor = 1f;
        keepMomentum = false;
    }
}
