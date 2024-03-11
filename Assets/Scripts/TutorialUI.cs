using System;
using System.Collections;
using UnityEngine;

public class TutorialUI : MonoBehaviour
{
    private PlayerMovement pm;
    private GameObject[] tutorials;
    private int currTutorial;

    void Awake() {
        pm = FindFirstObjectByType<PlayerMovement>();
        tutorials = GameObject.FindGameObjectsWithTag("Tutorial");
        currTutorial = 0;

        // Sort tutorial GameObjects by name
        Array.Sort(tutorials, CompareTutorialNames);

        foreach (GameObject tut in tutorials) {
            tut.SetActive(false);
        }
    }

    // Function for sorting tutorials by name
    int CompareTutorialNames(GameObject x, GameObject y)
    {
        return x.name.CompareTo(y.name);
    }

    // Start is called before the first frame update
    void Update()
    {
        // Disable movement
        pm.disableMovement();

        // Pause game while tutorial is active
        Time.timeScale = 0;
        
        // Display tutorial
        tutorials[currTutorial].SetActive(true);
        // Debug.Log(tutorials[currTutorial].name);

        // Unlock and display cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    [ContextMenu("CloseTutorial")]
    public void CloseTutorial() {

        // Show specific tutorial screen
        tutorials[currTutorial].SetActive(false);
        currTutorial++; // Move index to next tutorial screen

        // Hide and lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Unpause game
        Time.timeScale = 1.0f;
        
        // Re-enable movement
        pm.enableMovement();

        // Close tutorial
        gameObject.SetActive(false);
    }
}