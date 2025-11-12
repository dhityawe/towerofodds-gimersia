// ChipSkill.cs
// Shoots a single chip that splits on first hit based on BaseAttackCount

using UnityEngine;

[CreateAssetMenu(fileName = "Chip Skill", menuName = "Tower/Skills/Chip", order = 1)]
public class ChipSkill : TowerSkill
{
    [Header("Chip Specific")]
    [Tooltip("Attack range to find enemies")]
    [SerializeField] private float attackRange = 15f;
    
    [Tooltip("Main chip projectile prefab (needs ChipProjectile component)")]
    [SerializeField] private GameObject chipProjectilePrefab;
    
    [Tooltip("Mini-chip projectile prefab (needs MiniChipProjectile component)")]
    [SerializeField] private GameObject miniChipPrefab;
    
    [Tooltip("Projectile speed")]
    [SerializeField] private float projectileSpeed = 20f;
    
    [Tooltip("Mini-chip damage multiplier (0.5 = 50% of primary damage)")]
    [SerializeField, Range(0.1f, 1f)] private float miniChipDamageMultiplier = 0.5f;

    public override void Activate(TowerRuntime tower, int slotIndex, int diceRoll1, int diceRoll2)
    {
        // Find nearest enemy
        var target = tower.FindClosestEnemy(attackRange);
        if (target == null)
        {
            Debug.Log("[Chip] No target in range");
            return;
        }

        // Get dice multiplier from tower's 2d6 roll
        float towerDiceMultiplier = TowerBase.GetDiceMultiplier(diceRoll1, diceRoll2);
        
        // Calculate damages
        // Primary hit: Damage = BaseDamage * towerDiceMultiplier
        float primaryDamage = baseDamage * towerDiceMultiplier;
        
        // Mini-chip damage: MiniDamage = BaseDamage * towerDiceMultiplier * 0.5f
        float miniDamage = baseDamage * towerDiceMultiplier * miniChipDamageMultiplier;
        
        // Split count from tower stats
        int splitCount = Mathf.Max(1, tower.GetStats().BaseAttackCount);

        // Spawn chip projectile
        if (chipProjectilePrefab != null && miniChipPrefab != null)
        {
            GameObject chipObj = Instantiate(chipProjectilePrefab, tower.transform.position, Quaternion.identity);
            var chipProjectile = chipObj.GetComponent<TowerOfOdds.Combat.ChipProjectile>();
            
            if (chipProjectile != null)
            {
                chipProjectile.Init(
                    target.transform,
                    primaryDamage,
                    miniDamage,
                    splitCount,
                    projectileSpeed,
                    miniChipPrefab,
                    tower,
                    hitSound,
                    hitSoundVolume
                );
                
                Debug.Log($"[Chip] Fired! Primary: {primaryDamage:F1} dmg | Mini: {miniDamage:F1} dmg | Splits: {splitCount} | Tower Multiplier: {towerDiceMultiplier:F2} | Target: {target.name}");
            }
            else
            {
                Debug.LogWarning("[Chip] Chip prefab missing ChipProjectile component!");
                Destroy(chipObj);
            }
        }
        else
        {
            if (chipProjectilePrefab == null)
                Debug.LogWarning("[Chip] Chip projectile prefab not assigned!");
            if (miniChipPrefab == null)
                Debug.LogWarning("[Chip] Mini-chip projectile prefab not assigned!");
        }
    }
}
