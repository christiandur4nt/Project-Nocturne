using UnityEngine;

public class SlowTimeAbility : MonoBehaviour
{
    [Header("Keybind")]
    public char key;

    [Header("Slow Time Variables")]
    public float slowTimeScale;
    public float duration = 3f;

    [Header("Audio Clip")]
    public AudioClip slowSound;
    public AudioClip speedSound;

    [Header("Cooldown Duration")]
    public float cooldownDuration;

    // Internal
    private float cooldownTimer = 0;
    private float activeTimer = 0;
    private bool isTimeSlowed = false;
    private float ogFixedDeltaTime;

    void Reset() {
        key = 'r';
        slowTimeScale = 0.5f;
        cooldownDuration = 5f;
    }

    void Awake() {
        this.ogFixedDeltaTime = Time.fixedDeltaTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (cooldownTimer <= 0 && Input.GetKeyDown(key+"")) {
            if (!isTimeSlowed)
                ActivateSlowTime();
            else
                DeactivateSlowTime();
        }

        if (isTimeSlowed) {
            activeTimer += Time.deltaTime;
            if (activeTimer >= duration) DeactivateSlowTime();
        }

        // Update Player UI icons (WIP)
        Color color = PlayerUI.Instance.abilityIcons[(int)PlayerUI.Ability.SlowTimeAbility].color;
        color.a = Mathf.SmoothStep(1f, 0.2f, cooldownTimer/cooldownDuration);
        PlayerUI.Instance.abilityIcons[(int)PlayerUI.Ability.SlowTimeAbility].color = color;
        PlayerUI.Instance.abilityTimers[(int)PlayerUI.Ability.SlowTimeAbility].SetText(cooldownTimer <= 0 ? "<color=blue><b>R" : cooldownTimer.ToString("0.0"));

        if (cooldownTimer > 0)
            cooldownTimer -= Time.deltaTime;
    }

    private void ActivateSlowTime() {
        Time.timeScale = slowTimeScale;
        isTimeSlowed = true;
        activeTimer = 0;
        Time.fixedDeltaTime = ogFixedDeltaTime * Time.timeScale;
        SoundManager.Instance.PlaySoundClip(slowSound, transform, 1f);
    }

    private void DeactivateSlowTime() {
        Time.timeScale = 1.0f; // reset
        isTimeSlowed = false;   
        cooldownTimer = cooldownDuration;
        Time.fixedDeltaTime = ogFixedDeltaTime * Time.timeScale;
        SoundManager.Instance.PlaySoundClip(speedSound, transform, 1f);
    }

    public bool IsTimeSlowed() {
        return isTimeSlowed;
    }
}
