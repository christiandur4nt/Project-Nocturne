using UnityEngine;

public class EnemyKill : MonoBehaviour
{
    [Header("Components")]
    public GameObject playerCharacter;
    private DashAbility dashAbilityScript;

    // Internal
    private bool playerDashing = false;

    void Awake() {
        dashAbilityScript = playerCharacter.GetComponent<DashAbility>();
    }

    // Update is called once per frame
    void Update()
    {
        playerDashing = dashAbilityScript.IsDashing();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player" && playerDashing)
        {
            Destroy(gameObject);
            collision.rigidbody.velocity = UnityEngine.Vector3.zero;
        }
    }
}
