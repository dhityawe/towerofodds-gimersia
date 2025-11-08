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
        TowerRuntime towerComponent = tower?.GetComponent<TowerRuntime>();
        if (towerComponent != null)
        {
            towerComponent.ApplyDamage(attackDamage);
            Debug.Log($"Melee Enemy attacks tower for {attackDamage} damage!");
        }
    }        protected override void MovementBehavior()
        {
            // Melee enemies move straight to tower aggressively
            base.MovementBehavior();
        }
    }
}
