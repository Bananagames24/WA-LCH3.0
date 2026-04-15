using UnityEngine;

public class DeKijkerEnemy : EnemyBase
{
    [Header("Kijker Settings")]
    public float maxStoredSpeed = 10f;
    public float chargeUpRate = 2f;
    
    private float storedSpeedMultiplier = 1f;
    private Camera mainCam;

    protected override void Start()
    {
        base.Start();
        mainCam = Camera.main;
    }

    protected override void ChasePlayer()
    {
        bool lookedAt = IsLookedAt();

        // Must not move when looked at (like De Vergeten? De Kijker logic says "if you look away, it moves faster. Om te overleven moet je niet kijken")
        // Wait, "Als jij hem ziet -> ziet hij jou. Hoe langer oogcontact, hoe sneller hij beweegt als je wegkijkt. Om te overleven moet je juist niet kijken".
        
        if (lookedAt)
        {
            agent.isStopped = true;
            storedSpeedMultiplier += chargeUpRate * Time.deltaTime;
            if (storedSpeedMultiplier > maxStoredSpeed) storedSpeedMultiplier = maxStoredSpeed;
        }
        else
        {
            agent.isStopped = false;
            // Drain multiplier over time so it slows down eventually if you don't look
            storedSpeedMultiplier -= Time.deltaTime * 0.5f; 
            if (storedSpeedMultiplier < 1f) storedSpeedMultiplier = 1f;

            agent.speed = baseSpeed * storedSpeedMultiplier;
            base.ChasePlayer();
        }
    }

    bool IsLookedAt()
    {
        if (mainCam == null) return false;
        
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(mainCam);
        if (GeometryUtility.TestPlanesAABB(planes, GetComponent<Collider>().bounds))
        {
            Vector3 dirToEnemy = (transform.position - mainCam.transform.position).normalized;
            RaycastHit hit;
            if (Physics.Raycast(mainCam.transform.position, dirToEnemy, out hit))
            {
                if (hit.collider.gameObject == gameObject) return true;
            }
        }
        return false;
    }
}
