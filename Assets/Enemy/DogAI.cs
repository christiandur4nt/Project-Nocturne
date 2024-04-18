using System;
using UnityEngine;
using UnityEngine.AI;

public class DogAI : MonoBehaviour
{
    [Range(0, 100), SerializeField] private float speed;
    [Range(0, 500), SerializeField] private float walkRadius;
    private NavMeshAgent agent;
    private float walkTimeStart;
    private float walkTimeElapsed;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if(agent != null)
        {
            agent.enabled = true;
            agent.speed = speed;
            agent.SetDestination(RandomNavMeshLocation());
        }
    }

    public Vector3 RandomNavMeshLocation()
    {
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * walkRadius;
        Vector3 randomPosition = transform.position + randomDirection;
        walkTimeStart = Time.time;

        if (NavMesh.SamplePosition(randomPosition, out NavMeshHit hit, walkRadius, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return Vector3.zero;
    }


    // Update is called once per frame
    void Update()
    {
        walkTimeElapsed = Time.time - walkTimeStart;
        if(agent != null && (agent.remainingDistance <= agent.stoppingDistance) || walkTimeElapsed > 2)
        {
            agent.SetDestination(RandomNavMeshLocation());
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Player")
        {
            Quaternion targetRotation = Quaternion.LookRotation(other.transform.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * speed);
            transform.position = Vector3.MoveTowards(transform.position ,other.transform.position, speed * Time.deltaTime*.15f);
        }
    }
}