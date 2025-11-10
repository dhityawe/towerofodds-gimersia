// BombChipProjectile.cs
// Projectile that explodes on enemy hit, dealing AoE damage

using System.Collections.Generic;
using UnityEngine;
using TowerOfOdds.Enemies;

public class BombChipProjectile : MonoBehaviour
{
    private Transform target;
    private float speed;
    private float directDamage;
    private float aoeDamage;
    private float explosionRadius;
    private GameObject explosionEffectPrefab;
    private float explosionDuration;
    private TowerRuntime towerRuntime;
    private bool hasHit = false;

    public void Init(float projectileSpeed, float hitDamage, Transform targetTransform, 
                     TowerRuntime tower, float explRadius, float aoeHitDamage, 
                     GameObject explosionPrefab, float explDuration)
    {
        speed = projectileSpeed;
        directDamage = hitDamage;
        target = targetTransform;
        towerRuntime = tower;
        explosionRadius = explRadius;
        aoeDamage = aoeHitDamage;
        explosionEffectPrefab = explosionPrefab;
        explosionDuration = explDuration;
    }

    private void Update()
    {
        if (hasHit) return;

        // Check if target is still valid
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        // Move towards target
        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        // Check if reached target
        if (Vector3.Distance(transform.position, target.position) < 0.2f)
        {
            OnHitTarget();
        }
    }

    private void OnHitTarget()
    {
        if (hasHit) return;
        hasHit = true;

        Vector3 hitPosition = transform.position;

        // Apply direct damage to the hit enemy
        var hitEnemy = target.GetComponent<BaseEnemy>();
        if (hitEnemy != null && hitEnemy.IsAlive)
        {
            hitEnemy.TakeDamage(directDamage);
            Debug.Log($"[BombChip] Direct hit on {hitEnemy.name} for {directDamage:F1} damage");
        }

        // Get all enemies in explosion radius and apply AoE damage
        var allEnemies = FindObjectsByType<BaseEnemy>(FindObjectsSortMode.None);
        int hitCount = 0;

        foreach (var enemy in allEnemies)
        {
            if (enemy == null || !enemy.IsAlive) continue;

            float distance = Vector3.Distance(hitPosition, enemy.GetPosition());
            if (distance <= explosionRadius)
            {
                enemy.TakeDamage(aoeDamage);
                hitCount++;
            }
        }

        Debug.Log($"[BombChip] Explosion hit {hitCount} enemies for {aoeDamage:F1} damage each (radius: {explosionRadius})");

        // Spawn explosion visual effect
        if (explosionEffectPrefab != null)
        {
            GameObject explosion = Instantiate(explosionEffectPrefab, hitPosition, Quaternion.identity);
            explosion.name = "BombChip_Explosion";
            Destroy(explosion, explosionDuration);
        }
        else
        {
            // Fallback: Create simple visual explosion
            GameObject explosion = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            explosion.transform.position = hitPosition;
            explosion.transform.localScale = Vector3.one * explosionRadius * 2f;
            
            var renderer = explosion.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = new Color(1f, 0.3f, 0f, 0.5f); // Orange with transparency
            }
            
            // Remove collider
            var collider = explosion.GetComponent<Collider>();
            if (collider != null) Destroy(collider);
            
            explosion.name = "BombChip_Explosion_Fallback";
            Destroy(explosion, explosionDuration);
        }

        // Destroy projectile
        Destroy(gameObject);
    }
}
