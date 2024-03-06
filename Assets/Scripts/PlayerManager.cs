using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private Vector3 checkpointPos;
    public Vector3 CheckpointPos { get => checkpointPos; set => checkpointPos = value; }

    void Awake() {
        checkpointPos = transform.position;
    }

    public void ResetToCheckpoint() {
        transform.position = checkpointPos;
    }
}
