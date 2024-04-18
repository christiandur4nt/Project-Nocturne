using UnityEngine;
using DG.Tweening;

public class CameraMovement : MonoBehaviour
{
    [Header("Components")]
    [HideInInspector] public Transform orientation;
    [HideInInspector] public Transform cameraHolder;
    [HideInInspector] public PlayerMovement pm;
    
    [Header("Mouse Variables")]
    public float sensitivityX;
    public float sensitivityY;
    public float smoothness;

    // Internal
    private float rotationX;
    private float rotationY;
    public const float DEFAULT_SENSITIVITY = 500f;
    public const float DEFAULT_FIELD_OF_VIEW = 90f;

    void Reset() {
        sensitivityX = sensitivityY = DEFAULT_SENSITIVITY;
    }

    void Awake() {
        pm = FindFirstObjectByType<PlayerMovement>();
        orientation = GameObject.Find("Orientation").transform;
        cameraHolder = transform.parent;

        // Set default values
        if (!PlayerPrefs.HasKey("sensitivity")) PlayerPrefs.SetFloat("sensitivity", DEFAULT_SENSITIVITY);
        if (!PlayerPrefs.HasKey("fieldOfView")) PlayerPrefs.SetFloat("fieldOfView", DEFAULT_FIELD_OF_VIEW);

        sensitivityX = sensitivityY = PlayerPrefs.GetFloat("sensitivity");
        GetComponent<Camera>().fieldOfView = PlayerPrefs.GetFloat("fieldOfView");
    }

    void Start()
    {
        // Lock and hide the mouse cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (pm.AllowMovement)
        {
            float mouseX = Input.GetAxis("Mouse X") * sensitivityX * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * sensitivityY * Time.deltaTime;

            orientation.Rotate(Vector3.up * mouseX);

            rotationY += mouseX;
            rotationX -= mouseY;
            rotationX = Mathf.Clamp(rotationX, -90f, 90f);

            cameraHolder.rotation = Quaternion.Euler(rotationX, rotationY, 0f);
            orientation.rotation = Quaternion.Euler(0, rotationY, 0);
        }
    }

    public void AdjustSensitivity(float newSensitivity)
    {
        sensitivityX = sensitivityY = newSensitivity;
        PlayerPrefs.SetFloat("sensitivity", newSensitivity);
    }

    public void AdjustFOV(float fov)
    {
        GetComponent<Camera>().fieldOfView = fov;
        PlayerPrefs.SetFloat("fieldOfView", fov);
    }

    public void doFOV(float endValue)
    {
        GetComponent<Camera>().DOFieldOfView(endValue, 0.3f);
    }

    public void DoTilt(float zTilt)
    {
        transform.DOLocalRotate(new Vector3(0, 0, zTilt), 0.25f);
    }
}
