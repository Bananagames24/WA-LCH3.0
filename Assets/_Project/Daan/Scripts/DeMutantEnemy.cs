using UnityEngine;
using UnityEngine.AI;

public class DeMutantEnemy : EnemyBase
{
    private Vector3 lastKnownNoiseLocation;
    private bool hearsNoise = false;
    public float wanderRadius = 10f;

    protected override void Start()
    {
        base.Start();
        SetRandomDestination();
    }

    protected override void ChasePlayer()
    {
        if (hearsNoise)
        {
            agent.SetDestination(lastKnownNoiseLocation);
            if (Vector3.Distance(transform.position, lastKnownNoiseLocation) < 2f)
            {
                hearsNoise = false; // Reached noise origin
                SetRandomDestination();
            }
        }
        else
        {
            // Wandering blindly
            if (agent.remainingDistance < 1f)
            {
                SetRandomDestination();
            }
        }
    }

    void SetRandomDestination()
    {
        Vector3 randomDir = Random.insideUnitSphere * wanderRadius;
        randomDir += transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDir, out hit, wanderRadius, 1))
        {
            agent.SetDestination(hit.position);
        }
    }

    // Call this from PlayerMovement or other scripts when making sound (Sprinting, cleaning)
    public void HearNoise(Vector3 origin)
    {
        lastKnownNoiseLocation = origin;
        hearsNoise = true;
    }
}
