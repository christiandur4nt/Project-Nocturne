using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private Vector3 checkpointPos;
    private Rigidbody rb;
    public Vector3 CheckpointPos { get => checkpointPos; set => checkpointPos = value; }
    public static PlayerManager Instance;
    public static bool isDead;

    void Awake() {
        checkpointPos = transform.position;
        rb = GetComponent<Rigidbody>();
        if (Instance != null) Instance = this;
        isDead = false;
    }

    public void ResetToCheckpoint() {
        transform.position = checkpointPos;
        rb.velocity = Vector3.zero;
    }
}
