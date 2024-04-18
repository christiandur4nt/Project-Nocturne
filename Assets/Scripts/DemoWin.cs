using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DemoWin : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        LevelChanger.instance.FadeOut();
    }
}
