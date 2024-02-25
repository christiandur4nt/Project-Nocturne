using UnityEngine;
using DG.Tweening;

public class CameraMovement : MonoBehaviour
{
    [Header("Components")]
    public Transform orientation;
    
    [Header("Mouse Variables")]
    public float sensitivityX;
    public float sensitivityY;
    public float smoothness;

    // Internal
    private float rotationX;
    private float rotationY;

    void Reset() {
        sensitivityX = sensitivityY = 800f;
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
        

        float mouseX = Input.GetAxis("Mouse X") * sensitivityX * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivityY * Time.deltaTime;

        orientation.Rotate(Vector3.up * mouseX);

        rotationY += mouseX;
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);

        transform.rotation = Quaternion.Euler(rotationX, rotationY, 0f);
        orientation.rotation = Quaternion.Euler(0, rotationY, 0);
        // transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, smoothness);
    }

    public void doFOV(float endValue)
    {
        GetComponent<Camera>().DOFieldOfView(endValue, 0.3f);
    }
}
