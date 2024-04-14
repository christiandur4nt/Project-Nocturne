using UnityEngine;

public class SlowTimeAbility : MonoBehaviour
{
    [Header("Keybind")]
    public char key;

    [Header("Slow Time Variables")]
    public float slowTimeScale;
    public float duration = 3f;

    // Internal
    private bool isTimeSlowed = false;
    private float ogFixedDeltaTime;

    void Reset() {
        key = 'r';
        slowTimeScale = 0.5f;
    }

    void Awake() {
        this.ogFixedDeltaTime = Time.fixedDeltaTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(key+"")) {
            if (Time.timeScale == 1.0f)
                Time.timeScale = slowTimeScale;
            else
                Time.timeScale = 1.0f; // reset
            Time.fixedDeltaTime = ogFixedDeltaTime * Time.timeScale;
        }
    }

    public bool IsTimeSlowed() {
        return isTimeSlowed;
    }
}
