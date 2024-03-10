using System;
using UnityEngine;

public class EnemyKill : MonoBehaviour
{
    [Header("References")]
    public GameObject Player;

    // Internal
    private PlayerMovement pm;
    private bool dashing = false;

    void Awake()
    {
        pm = Player.GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        dashing = pm.dashing;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player" && dashing)
        {
            Destroy(gameObject);
        }
    }
}
