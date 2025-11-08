using UnityEngine;

namespace TowerOfOdds.Enemies
{
    /// <summary>
    /// Melee Enemy - Fast movement, low HP, moderate damage
    /// Rushes to tower and attacks in close range
    /// </summary>
    public class MeleeEnemy : BaseEnemy
    {
        // Semua stats sekarang di-load dari EnemyData ScriptableObject
        // Tidak perlu hardcode lagi di sini

        protected override void Attack()
        {
            // Melee attack - direct damage to tower
            Tower.Tower towerComponent = tower?.GetComponent<Tower.Tower>();
            if (towerComponent != null)
            {
                towerComponent.TakeDamage(attackDamage);
                Debug.Log($"Melee Enemy attacks tower for {attackDamage} damage!");
            }
        }

        protected override void MovementBehavior()
        {
            // Melee enemies move straight to tower aggressively
            base.MovementBehavior();
        }
    }
}
