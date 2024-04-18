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
    GameObject cProjectile = null;
    private bool projectileDeployed = false;
    private float ProjectileStartTime = 0;
    private Rigidbody projectileRb;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = patrolPoints[targetPoint].position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - ProjectileStartTime >= 2)
        {
            if(projectileDeployed)
            {
                GameObject.Destroy(cProjectile);
                cProjectile = null;
                projectileDeployed = false;
                ProjectileStartTime = 0;
            }
        }

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

    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Player")
        {
            if(enemyAnim != null)
                enemyAnim.SetBool("attack", true);

            if(enemyType == enemyType.flying)
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
                    cProjectile = Instantiate(Projectile, transform.position, UnityEngine.Quaternion.identity);
                    projectileRb = cProjectile.GetComponent<Rigidbody>();
                    projectileRb.AddForce(-player.transform.position.normalized * shootForce, ForceMode.Impulse);
                }
                
            }
        }
    }
   
}
