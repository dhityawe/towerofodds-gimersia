using UnityEngine;

namespace TowerOfOdds.Enemies
{
    /// <summary>
    /// Tank Enemy - Slow movement, very high HP, high damage
    /// Absorbs tower damage while slowly advancing
    /// </summary>
    public class TankEnemy : BaseEnemy
    {
        // Stats sekarang di-load dari EnemyData ScriptableObject
        private float damageReduction = 0.2f; // Will be loaded from enemyData

        protected override void LoadStatsFromData()
        {
            base.LoadStatsFromData();

            if (enemyData != null)
            {
                damageReduction = enemyData.damageReduction;
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
            // Tank attack - heavy damage to tower
            Tower.Tower towerComponent = tower?.GetComponent<Tower.Tower>();
            if (towerComponent != null)
            {
                towerComponent.TakeDamage(attackDamage);
                Debug.Log($"Tank Enemy smashes tower for {attackDamage} damage!");
            }
        }

        protected override void MovementBehavior()
        {
            // Tank moves slowly but steadily
            base.MovementBehavior();
        }
    }
}
