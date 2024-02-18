using UnityEngine;

public class BillboardEffect : MonoBehaviour
{
    [Header("Lock Rotation")]
    [SerializeField] private bool lockX;
    [SerializeField] private bool lockY;
    [SerializeField] private bool lockZ;

    private Vector3 originalRotation;

    void Awake() {
        originalRotation = transform.rotation.eulerAngles;
    }

    void LateUpdate()
    {
        transform.LookAt(Camera.main.transform.position);

        // For locking rotation
        Vector3 rotation = transform.rotation.eulerAngles;
        if (lockX) rotation.x = originalRotation.x;
        if (lockY) rotation.y = originalRotation.y;
        if (lockZ) rotation.z = originalRotation.z;
        transform.rotation = Quaternion.Euler(rotation);
    }
}
