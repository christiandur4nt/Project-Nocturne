using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header("Components")]
    public Transform playerBody;
    
    [Header("Movement Variables")]
    public float sensitivity;
    public float smoothness = 0.5f;

    // Internal
    private float rotationX = 0f;

    void Reset() {
        sensitivity = 800f;
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
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        playerBody.Rotate(Vector3.up * mouseX);

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);

        Quaternion targetRotation = Quaternion.Euler(rotationX, 0f, 0f);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, smoothness);
    }
}
