using UnityEngine;

public class FOVController : MonoBehaviour
{
    [Header("FOV Variables")]
    public float minFOV;
    public float maxFOV;
    public float maxSpeed;
    public AnimationCurve easingCurve;

    // Components
    private PlayerMovement playerMovementScript;

    void Start() {
        playerMovementScript = GetComponent<PlayerMovement>();
    }

    void Reset() {
        minFOV = 60f;
        maxFOV = 100f;
        maxSpeed = 100f;
        easingCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    }

    // Update is called once per frame
    void Update()
    {
        // Update Camera FOV according to speed in the XZ direction
        Vector3 vel = playerMovementScript.GetVelocity();
        float speed = (vel-vel.y*Vector3.up).magnitude;
        float parameter = easingCurve.Evaluate(Mathf.InverseLerp(0, maxSpeed, speed));
        float FOV = Mathf.Lerp(minFOV, maxFOV, parameter);
        Camera.main.fieldOfView = FOV;
    }
}
