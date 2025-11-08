using UnityEngine;

namespace TowerOfOdds.Enemies
{
    /// <summary>
    /// Tank Enemy - Slow movement, very high HP, high damage
    /// Absorbs tower damage while slowly advancing
    /// Can shoot projectiles at range
    /// </summary>
    public class TankEnemy : BaseEnemy
    {
        // Stats sekarang di-load dari EnemyData ScriptableObject
        private float damageReduction = 0.2f; // Will be loaded from enemyData
        private GameObject projectilePrefab;
        private float projectileSpeed = 10f;

        protected override void LoadStatsFromData()
        {
            base.LoadStatsFromData();

            if (enemyData != null)
            {
                damageReduction = enemyData.damageReduction;
                projectilePrefab = enemyData.projectilePrefab;
                projectileSpeed = enemyData.projectileSpeed;
            }
        }

        public override void TakeDamage(float damage)
        {
            // Tank has damage reduction
            float reducedDamage = damage * (1f - damageReduction);
            base.TakeDamage(reducedDamage);
            Debug.Log($"Tank Enemy takes {reducedDamage} damage (reduced from {damage})");
        }

    protected override void Attack()
    {
        // Tank attack - projectile or direct damage
        if (tower == null) return;

        if (projectilePrefab != null)
        {
            // Shoot heavy projectile
            GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            var projectile = proj.GetComponent<Combat.Projectile>();
            if (projectile != null)
            {
                projectile.Init(tower, attackDamage, projectileSpeed, true);
            }
            Debug.Log($"Tank Enemy fires heavy projectile for {attackDamage} damage!");
        }
        else
        {
            // Fallback to direct damage (melee smash)
            TowerRuntime towerComponent = tower?.GetComponent<TowerRuntime>();
            if (towerComponent != null)
            {
                towerComponent.ApplyDamage(attackDamage);
                Debug.Log($"Tank Enemy smashes tower for {attackDamage} damage!");
            }
        }
    }        protected override void MovementBehavior()
        {
            // Tank moves slowly but steadily
            base.MovementBehavior();
        }
    }
}
