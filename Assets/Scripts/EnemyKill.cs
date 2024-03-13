using System;
using UnityEngine;

public class EnemyKill : MonoBehaviour
{
    [Header("References")]
    public GameObject Player;
    public GameObject RespawnObj; 

    [SerializeField] private AudioClip killSound;

    // Internal
    private PlayerMovement pm;
    private DemoRespawn dr;
    private Rigidbody rb;
    private bool dashing = false;
    private Vector3 originalPos;
    private Quaternion originalRot;

    void Awake()
    {
        pm = Player.GetComponent<PlayerMovement>();
        dr = RespawnObj.GetComponent<DemoRespawn>();
        rb = gameObject.GetComponent<Rigidbody>();
        originalPos = gameObject.transform.position;
        originalRot = gameObject.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        dashing = pm.dashing;
        if (dr.respawn)
            Respawn();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player" && dashing)
        {
            SoundManager.instance.PlaySoundClip(killSound, transform, 1f);
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - 100, gameObject.transform.position.z);
            rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
        }
    }

    private void Respawn()
    {
        gameObject.transform.position = originalPos;
        gameObject.transform.rotation = originalRot;
        dr.respawn = false;
    }
}
