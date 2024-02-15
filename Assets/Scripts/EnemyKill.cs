using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class EnemyKill : MonoBehaviour
{

    private bool playerDashing = false;
    // Start is called before the first frame update
    // void Start()
    // {
        
    // }

    // Update is called once per frame
    void Update()
    {
        playerDashing = DashAbility.instance.isDashing;
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
