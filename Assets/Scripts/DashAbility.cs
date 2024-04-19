using System;
using System.Collections;
using UnityEngine;

public class DashAbility : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Animator armAnimation;
    [SerializeField] private AudioClip dashSound;
    [HideInInspector] public Transform orientation;
    [HideInInspector] public Transform playerCamera;
    private PlayerMovement pm;
    private Rigidbody rb;

    [Header("Dash Variables")]
    public float dashForce;
    public float dashUpwardForce;
    public float dashDuration;
    public float maxDashYSpeed;

    [Header("Camera Effects")]
    [HideInInspector] public CameraMovement cam;
    public float dashFOV;

    [Header("Cooldown")]
    public float cooldownDuration;

    [Header("Keybinds")]
    public int dashButton = 0;

    [Header("Settings")]
    public bool useCameraForward = false;
    public bool allowAllDirections = true;
    public bool resetVelocity = true;
    public bool disableGravity = true;

    // Internal
    public bool onCooldown = false;
    private float cooldownTimer = 0;
    private bool grounded = true;

    void Awake() {
        orientation = GameObject.Find("Orientation").transform;
        playerCamera = GameObject.Find("Player Camera").transform;
        cam = FindObjectOfType<CameraMovement>();
    }

    void Start()
    {
        pm = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!pm.AllowMovement) return;

        grounded = pm.IsGrounded();
        if (Input.GetMouseButtonDown(dashButton) && !onCooldown)
        {
            Dash();
            onCooldown = true;
        }

        if (pm.IsGrounded())
            onCooldown = false;

        // Update Player UI icons (WIP)
        Color color = PlayerUI.Instance.abilityIcons[(int)PlayerUI.Ability.SlowTimeAbility].color;
        color.a = (cooldownTimer > 0) ? Mathf.SmoothStep(1f, 0.2f, cooldownTimer/cooldownDuration) : (onCooldown ? 0.2f : 1f);
        PlayerUI.Instance.abilityIcons[(int)PlayerUI.Ability.DashAbility].color = color;
        PlayerUI.Instance.abilityTimers[(int)PlayerUI.Ability.DashAbility].SetText(cooldownTimer.ToString("0.0"));

        if (cooldownTimer > 0)
            cooldownTimer -= Time.deltaTime;
    }

    void Dash()
    {
        if (cooldownTimer > 0) return;
        else if (pm.IsGrounded()) cooldownTimer = cooldownDuration;

        pm.dashing = true;
        pm.maxYSpeed = maxDashYSpeed;

        cam.doFOV(dashFOV);
        SoundManager.Instance.PlaySoundClip(dashSound, transform, 1f);

        Transform forwardT;

        if (useCameraForward)
            forwardT = playerCamera;
        else
            forwardT = orientation;

        Vector3 direction = getDirection(forwardT);

        Vector3 forceToApply = direction * dashForce + orientation.up * dashUpwardForce;
        
        rb.useGravity = false;

        rb.AddForce(forceToApply * Time.deltaTime, ForceMode.Impulse);
        delayedForceToApply = forceToApply;

        Invoke(nameof(DelayedDashForce), 0.025f);
        Invoke(nameof(ResetDash), dashDuration);


        armAnimation.SetBool("Is Dashing", true);

    }

    private Vector3 delayedForceToApply;
    private void DelayedDashForce()
    {
        if(resetVelocity)
            rb.velocity = Vector3.zero;

        rb.AddForce(delayedForceToApply, ForceMode.Impulse);
    }

    void ResetDash()
    {
        pm.dashing = false;
        pm.maxYSpeed = 0;
        armAnimation.SetBool("Is Dashing", false);

        cam.doFOV(60f);

        rb.useGravity = true;
    }

    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(cooldownDuration);
        onCooldown = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy" && pm.dashing)
            onCooldown = false;  
    }

    private Vector3 getDirection(Transform forwardT)
    {
        float xInput = Input.GetAxisRaw("Horizontal");
        float zInput = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3();

        if (allowAllDirections)
            direction = forwardT.forward * zInput + forwardT.right * xInput;
        else
            direction = forwardT.forward;
        
        if (zInput == 0 && xInput == 0)
            direction = forwardT.forward;

        return direction.normalized;
    }

    public bool IsDashing() {
        return pm.dashing;
    }
}
