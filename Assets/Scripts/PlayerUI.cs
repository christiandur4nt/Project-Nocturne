using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [Header("Ability Icons")]
    public Image[] abilityIcons;
    public TMP_Text[] abilityTimers;
    public static PlayerUI Instance;

    public enum Ability
    {
        GrappleAbility,
        DashAbility,
        SlowTimeAbility
    }

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance == null) Instance = this;
    }
}
