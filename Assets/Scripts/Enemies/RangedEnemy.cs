using UnityEngine;

namespace TowerOfOdds.Enemies
{
    /// <summary>
    /// Ranged Enemy - Medium speed, medium HP, lower damage but attacks from distance
    /// Maintains distance from tower while attacking
    /// </summary>
    public class RangedEnemy : BaseEnemy
    {
        // Stats sekarang di-load dari EnemyData ScriptableObject
        private float preferredDistance = 8f; // Will be loaded from enemyData
        private GameObject projectilePrefab;
        private float projectileSpeed = 15f;

        protected override void LoadStatsFromData()
        {
            base.LoadStatsFromData();

            if (enemyData != null)
            {
                preferredDistance = enemyData.preferredDistance;
                projectilePrefab = enemyData.projectilePrefab;
                projectileSpeed = enemyData.projectileSpeed;
            }
        }

        protected override void MovementBehavior()
        {
            if (tower == null) return;

            Vector2 currentPos = new Vector2(transform.position.x, transform.position.y);
            Vector2 towerPos = new Vector2(tower.position.x, tower.position.y);
            float distanceToTower = Vector2.Distance(currentPos, towerPos);

            // Maintain preferred distance (kiting behavior)
            Vector2 movement = Vector2.zero;
            
            if (distanceToTower < preferredDistance)
            {
                // Too close - move away (kite backward)
                Vector2 direction = (currentPos - towerPos).normalized;
                movement = direction * moveSpeed * Time.deltaTime;
            }
            else if (distanceToTower > attackRange)
            {
                // Too far - move closer to get in range
                Vector2 direction = (towerPos - currentPos).normalized;
                movement = direction * moveSpeed * Time.deltaTime;
            }
            // Else: in optimal range - stay still and attack
            
            // Apply movement (2D - keep Z at 0)
            if (movement != Vector2.zero)
            {
                transform.position = new Vector3(
                    transform.position.x + movement.x,
                    transform.position.y + movement.y,
                    0f
                );
            }
        }

        protected override void Attack()
        {
            // Ranged attack - projectile or instant damage from distance
            if (tower == null) return;

            if (projectilePrefab != null)
            {
                GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
                var projectile = proj.GetComponent<Combat.Projectile>();
                if (projectile != null)
                {
                    projectile.Init(tower, attackDamage, projectileSpeed, true);
                }
            }
            else
            {
                TowerRuntime towerComponent = tower?.GetComponent<TowerRuntime>();
                if (towerComponent != null)
                {
                    towerComponent.ApplyDamage(attackDamage);
                }
            }

            Debug.Log($"Ranged Enemy fires at tower for {attackDamage} damage (projectile: {projectilePrefab != null})!");
        }
    }
}
