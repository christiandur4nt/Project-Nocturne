using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndDoor : MonoBehaviour
{
    [Header("References")]
    public GameObject player;
    public Camera playerCamera;
    public LayerMask doorLayer;

    [Header("Settings")]
    public KeyCode interactKey = KeyCode.E;
    public float interactDist = 3f;
    public string newScene;

    private RaycastHit hit;

    // Update is called once per frame
    void Update()
    {   
        if (Input.GetKeyDown(interactKey))
        {
            if (Physics.Raycast(player.transform.position, playerCamera.transform.forward, out hit, interactDist, doorLayer) && newScene != null)
                SceneManager.LoadScene(newScene);
        }
    }
}
