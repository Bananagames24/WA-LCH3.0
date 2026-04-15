using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyBase : MonoBehaviour
{
    [Header("Base Stats")]
    public float maxHealth = 100f;
    protected float currentHealth;
    public float baseSpeed = 3f;
    public float attackDamage = 10f;
    public float attackRange = 1.5f;

    protected NavMeshAgent agent;
    protected Transform player;
    protected PlayerStats playerStats;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = baseSpeed;
        currentHealth = maxHealth;
    }

    protected virtual void Start()
    {
        // Find player via tag or movement script
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
        {
            player = p.transform;
            playerStats = p.GetComponent<PlayerStats>();
        }
    }

    protected virtual void Update()
    {
        if (player != null)
        {
            ChasePlayer();
            CheckAttack();
        }
    }

    protected virtual void ChasePlayer()
    {
        if (agent.isOnNavMesh)
        {
            agent.SetDestination(player.position);
        }
    }

    protected virtual void CheckAttack()
    {
        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            if (playerStats != null && playerStats.currentHealth > 0)
            {
                playerStats.TakeDamage(attackDamage * Time.deltaTime); // Continuous damage
            }
        }
    }

    public virtual void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        // E.g. drop keycard piece
        InventoryManager inv = FindFirstObjectByType<InventoryManager>();
        if (inv != null) inv.AddKeycardPiece(1);

        Destroy(gameObject);
    }
}
