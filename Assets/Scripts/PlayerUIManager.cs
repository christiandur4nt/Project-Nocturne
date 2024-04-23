using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{
    [Header("Ability Icons")]
    public Image[] abilityIcons;
    public TMP_Text[] abilityTimers;

    [Header("UI Elements")]
    [SerializeField] private GameObject deathPanel;
    [SerializeField] private GameObject interactText;
    private GameObject abilityPanel;
    private ParticleSystem bloodPS;

    [Header("Dog Transforms")]
    public Transform dog;
    public Transform dogResetPos;
    private bool resetDog;
    public bool ResetDog { get { return resetDog; } set { resetDog = value; } }

    public static PlayerUIManager Instance;

    private PlayerMovement playerMovement;
    private PlayerManager playerManager;

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
        bloodPS = GameObject.FindWithTag("Blood Particle System").GetComponent<ParticleSystem>();
        abilityPanel = GameObject.FindWithTag("Ability Panel");
        playerMovement = FindFirstObjectByType<PlayerMovement>();
        playerManager = FindFirstObjectByType<PlayerManager>();
        resetDog = false;
    }

    public void Die() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        PlayerManager.isDead = true;
        playerMovement.FreezePlayer();
        playerMovement.DisableMovement();
        bloodPS.Play();
        abilityPanel.SetActive(false);
        deathPanel.SetActive(true);
    }

    public void Undie() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        PlayerManager.isDead = false;
        playerMovement.UnfreezePlayer();
        playerMovement.EnableMovement();
        bloodPS.Stop();
        abilityPanel.SetActive(true);
        deathPanel.SetActive(false);
    }

    public void ResetToCheckpoint() {
        Undie();
        if (dog != null && resetDog) dog.position = dogResetPos.position;
        playerManager.ResetToCheckpoint();
    }

    public void RestartLevel() {
        Undie();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ActivateInteractText(bool isActive) {
        interactText.SetActive(isActive);
    }
}
