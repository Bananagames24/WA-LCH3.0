using UnityEngine;

public class RoombaBoombaEnemy : EnemyBase
{
    public float explosionRadius = 3f;
    public float explosionDamage = 50f;
    public ParticleSystem explosionEffect;

    protected override void ChasePlayer()
    {
        base.ChasePlayer();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Explode();
        }
    }

    protected override void CheckAttack()
    {
        // Attack logic handled by collision
    }

    void Explode()
    {
        if (Vector3.Distance(transform.position, player.position) <= explosionRadius)
        {
             if (playerStats != null) playerStats.TakeDamage(explosionDamage);
        }

        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        Die();
    }
}
