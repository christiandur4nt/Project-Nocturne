using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DemoMenu : MonoBehaviour
{
    public void PlayFirst()
    {
        SceneManager.LoadScene("Scenes/Tutorial-Dylan");
    }

    public void PlayGrapple()
    {
        SceneManager.LoadScene("Scenes/Grapple Hook Tutorial");
    }

    public void PlayDash()
    {
        SceneManager.LoadScene(3);
    }

    public void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
