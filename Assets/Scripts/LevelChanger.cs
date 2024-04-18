using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChanger : MonoBehaviour
{
    public static LevelChanger instance;
    public Animator animator;

    void Start()
    {
        if (instance == null)
            instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FadeOut()
    {
        animator.SetTrigger("Fade Out");
    }

    public void ChangeScene()
    {
        SceneManager.LoadScene(0);
    }
}
