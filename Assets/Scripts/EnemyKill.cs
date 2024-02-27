using System;
using UnityEngine;

public class EnemyKill : MonoBehaviour
{
    [Header("References")]
    public GameObject player;

    // Internal
    private PlayerMovement pm;
    private bool dashing = false;

    void Start()
    {
        pm = player.GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(dashing);
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
