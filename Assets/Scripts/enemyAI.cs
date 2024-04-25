using System;
using System.Runtime.CompilerServices;
using UnityEngine;

enum enemyType{
    basic = 0,
    flying = 1,
    ranged = 2
}

public class enemyAI : MonoBehaviour
{
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private int targetPoint;
    [SerializeField] private int speed;
    [SerializeField] Animator enemyAnim = null;
    [SerializeField] private enemyType enemyType;
    [SerializeField] private GameObject player;
    [SerializeField] private SphereCollider trigger;
    [SerializeField] private GameObject ProjectileSpawner;
    [SerializeField] private GameObject Projectile;
    [SerializeField] float shootForce;
    [SerializeField] private AudioClip shootSound;
    GameObject cProjectile = null;
    private bool projectileDeployed = false;
    private float ProjectileStartTime = 0;
    private float ProjectileSpawnInterval = 0;
    private Rigidbody projectileRb;

    public Boolean useDivebomb = true;
    // Start is called before the first frame update
    void Start()
    {
        if (patrolPoints != null && patrolPoints[0] != null)
            transform.position = patrolPoints[targetPoint].position;
        player = GameObject.Find("PlayerObj");
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - ProjectileStartTime >= 1.5f)
        {
            if(projectileDeployed)
            {
                GameObject.Destroy(cProjectile);
                cProjectile = null;
                //ProjectileSpawnInterval = Time.time + 1 + UnityEngine.Random.Range(0f, 1f);
                projectileDeployed = false;
                ProjectileStartTime = 0;
            }
            if (Time.time - ProjectileSpawnInterval >= 0)
            {
                
            }
        }

        
        if (patrolPoints != null && patrolPoints.Length != 0 && patrolPoints[0] != null)
        {
            if (transform.position == patrolPoints[targetPoint].position)
            {
                targetPoint++;
            }
            if (targetPoint >= patrolPoints.Length)
            {
                targetPoint = 0;
            }
            Quaternion targetRotation = Quaternion.LookRotation(patrolPoints[targetPoint].position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * speed);
            transform.position = UnityEngine.Vector3.MoveTowards(transform.position, patrolPoints[targetPoint].position, speed * Time.deltaTime); 
        }      
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Player")
        {
            if(enemyAnim != null)
                enemyAnim.SetBool("attack", true);

            if(enemyType == enemyType.flying && useDivebomb)
            {
                Quaternion targetRotation = Quaternion.LookRotation(other.transform.position - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * speed);
            }
            if(enemyType == enemyType.ranged)
            {
                if(projectileDeployed == false)
                {
                    ProjectileStartTime = Time.time;
                    projectileDeployed = true;
                    SoundManager.Instance.PlaySoundClip(shootSound, transform, 1f);
                    cProjectile = Instantiate(Projectile, ProjectileSpawner.transform.position, UnityEngine.Quaternion.identity);
                    projectileRb = cProjectile.GetComponent<Rigidbody>();
                    projectileRb.velocity = (player.transform.position - transform.position).normalized * shootForce;
                    Debug.Log(projectileRb.velocity);
                }
                
            }
        }
    }
   
}
