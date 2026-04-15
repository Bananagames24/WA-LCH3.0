using UnityEngine;

public class DeVergetenEnemy : EnemyBase
{
    private Camera mainCam;

    protected override void Start()
    {
        base.Start();
        mainCam = Camera.main;
    }

    protected override void ChasePlayer()
    {
        if (IsLookedAt())
        {
            // Stop moving if looked at
            agent.isStopped = true;
        }
        else
        {
            // Move towards player if not looked at
            agent.isStopped = false;
            base.ChasePlayer();
        }
    }

    bool IsLookedAt()
    {
        if (mainCam == null) return false;
        
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(mainCam);
        if (GeometryUtility.TestPlanesAABB(planes, GetComponent<Collider>().bounds))
        {
            // In camera view. Now test occlusion
            Vector3 dirToEnemy = (transform.position - mainCam.transform.position).normalized;
            RaycastHit hit;
            if (Physics.Raycast(mainCam.transform.position, dirToEnemy, out hit))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    return true; // We are directly looking at it without walls in between
                }
            }
        }
        return false;
    }
}
