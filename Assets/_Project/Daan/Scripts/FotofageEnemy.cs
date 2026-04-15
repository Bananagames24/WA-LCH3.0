using UnityEngine;

public class FotofageEnemy : EnemyBase
{
    [Header("Fotofage Settings")]
    public float maxGrowSpeed = 5f;
    public float lightGrowthRate = 0.5f;
    public float uvDamageRate = 20f;
    
    private PlayerTools playerTools;

    protected override void Start()
    {
        base.Start();
        if (player != null)
        {
            playerTools = player.GetComponent<PlayerTools>();
        }
    }

    protected override void Update()
    {
        base.Update();
        CheckLightExposure();
    }

    void CheckLightExposure()
    {
        if (playerTools == null || playerTools.flashlight == null || !playerTools.flashlight.enabled) return;

        // Simple check if player is looking at the enemy and light hits it
        Vector3 dirToEnemy = (transform.position - player.position).normalized;
        float angle = Vector3.Angle(player.forward, dirToEnemy);

        if (angle < playerTools.flashlight.spotAngle / 2f)
        {
            // Light is hitting the Fotofage
            RaycastHit hit;
            if (Physics.Raycast(player.position, dirToEnemy, out hit, playerTools.flashlight.range))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    // Check if normal light or UV
                    if (playerTools.flashlight.color == playerTools.uvLightColor)
                    {
                        // Takes continuous damage from UV
                        TakeDamage(uvDamageRate * Time.deltaTime);
                    }
                    else
                    {
                        // Grows and gets faster from normal light
                        agent.speed += lightGrowthRate * Time.deltaTime;
                        if (agent.speed > maxGrowSpeed) agent.speed = maxGrowSpeed;
                        
                        float scaleUp = 1f + (agent.speed - baseSpeed) * 0.1f;
                        transform.localScale = Vector3.one * scaleUp;
                    }
                }
            }
        }
    }
}
