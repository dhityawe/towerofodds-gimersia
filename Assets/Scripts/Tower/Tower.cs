using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace TowerOfOdds.Tower
{
    public class Tower : MonoBehaviour
    {
        [Header("Tower Stats")]
        [SerializeField] private float maxHealth = 1000f;
        [SerializeField] private float currentHealth;
        [SerializeField] private float attackDamage = 25f;
        [SerializeField] private float attackRange = 15f;
        [SerializeField] private float attackCooldown = 0.5f;

        [Header("Targeting")]
        [SerializeField] private LayerMask enemyLayer;
        private Enemies.BaseEnemy currentTarget;
        private float lastAttackTime;

    [Header("Projectile")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 20f;

        [Header("Status")]
        [SerializeField] private bool isAlive = true;

        public float CurrentHealth => currentHealth;
        public float MaxHealth => maxHealth;
        public bool IsAlive => isAlive;

        private void Start()
        {
            currentHealth = maxHealth;
        }

        private void Update()
        {
            if (!isAlive) return;

            // Find and attack enemies
            FindTarget();

            if (currentTarget != null && Time.time >= lastAttackTime + attackCooldown)
            {
                AttackTarget();
                lastAttackTime = Time.time;
            }
        }

        private void FindTarget()
        {
            // Always find closest enemy (dynamic retargeting)
            // This ensures tower always attacks the nearest threat
            currentTarget = FindClosestEnemy();
        }

        private Enemies.BaseEnemy FindClosestEnemy()
        {
            // Find all enemies in the scene
            Enemies.BaseEnemy[] allEnemies = Object.FindObjectsByType<Enemies.BaseEnemy>(FindObjectsSortMode.None);
            
            if (allEnemies.Length == 0)
                return null;

            // Filter alive enemies within range and sort by distance
            var enemiesInRange = allEnemies
                .Where(e => e.IsAlive && Vector3.Distance(transform.position, e.GetPosition()) <= attackRange)
                .OrderBy(e => Vector3.Distance(transform.position, e.GetPosition()))
                .ToList();

            // Return closest enemy (prioritas jarak terdekat)
            return enemiesInRange.FirstOrDefault();
        }

        private void AttackTarget()
        {
            if (currentTarget == null || !currentTarget.IsAlive)
                return;

            // Fire projectile if prefab provided, otherwise apply instant damage
            if (projectilePrefab != null)
            {
                GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
                var projectile = proj.GetComponent<Combat.Projectile>();
                if (projectile != null)
                {
                    projectile.Init(currentTarget.transform, attackDamage, projectileSpeed, false);
                }
            }
            else
            {
                currentTarget.TakeDamage(attackDamage);
            }

            Debug.Log($"Tower fires at {currentTarget.Type} enemy for {attackDamage} damage (projectile: {projectilePrefab != null})!");
        }

        public void TakeDamage(float damage)
        {
            if (!isAlive) return;

            currentHealth -= damage;
            currentHealth = Mathf.Max(0, currentHealth);

            Debug.Log($"Tower takes {damage} damage! HP: {currentHealth}/{maxHealth}");

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            isAlive = false;
            Debug.Log("Tower destroyed! Game Over!");
            
            // Trigger game over
            Manager.GameManager gameManager = Object.FindFirstObjectByType<Manager.GameManager>();
            if (gameManager != null)
            {
                gameManager.OnTowerDestroyed();
            }
        }

        // Upgrade methods for idle game progression
        public void UpgradeMaxHealth(float amount)
        {
            maxHealth += amount;
            currentHealth += amount; // Also heal
            Debug.Log($"Tower max health upgraded to {maxHealth}");
        }

        public void UpgradeAttackDamage(float amount)
        {
            attackDamage += amount;
            Debug.Log($"Tower attack damage upgraded to {attackDamage}");
        }

        public void UpgradeAttackSpeed(float reductionPercent)
        {
            attackCooldown *= (1f - reductionPercent);
            attackCooldown = Mathf.Max(0.1f, attackCooldown); // Min 0.1s cooldown
            Debug.Log($"Tower attack speed upgraded. Cooldown: {attackCooldown}s");
        }

        public void UpgradeRange(float amount)
        {
            attackRange += amount;
            Debug.Log($"Tower range upgraded to {attackRange}");
        }

        public void Heal(float amount)
        {
            currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
            Debug.Log($"Tower healed for {amount}. HP: {currentHealth}/{maxHealth}");
        }

        private void OnDrawGizmosSelected()
        {
            // Visualize attack range in editor
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}
