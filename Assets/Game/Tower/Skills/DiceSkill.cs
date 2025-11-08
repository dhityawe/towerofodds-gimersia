// DiceSkill.cs
// Fires one dice projectile at the nearest enemy.
// On impact: deal randomized damage 1-6 (visual pip), and briefly hasten the tower's next attack.
// Scales with: AttackSpeed (tempo)

using UnityEngine;

[CreateAssetMenu(fileName = "Dice Skill", menuName = "Tower/Skills/Dice", order = 0)]
public class DiceSkill : TowerSkill
{
    [Header("Dice Specific")]
    [Tooltip("Attack speed reduction per roll point (default: 0.03s reduction per pip)")]
    [SerializeField] private float speedUpPerRoll = 0.03f;
    
    [Tooltip("Duration of the speed-up buff in seconds")]
    [SerializeField] private float buffDuration = 0.75f;

    [Header("Projectile")]
    [SerializeField] private float attackRange = 15f;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 25f;

    public override void Activate(TowerRuntime tower, int slotIndex, int diceRoll1, int diceRoll2)
    {
        Debug.Log($"[Dice] Activate called - Slot: {slotIndex} | Tower: {tower.name} | Time: {Time.time:F2}");
        
        // Find nearest enemy
        var target = tower.FindClosestEnemy(attackRange);
        if (target == null)
        {
            Debug.Log("[Dice] No target in range");
            return;
        }

        // Roll a single die (1-6) for damage variance - R ∈ {1..6}
        int R = Random.Range(1, 7);
        
        // Get dice multiplier from tower's dice system - M
        float M = TowerBase.GetDiceMultiplier(diceRoll1, diceRoll2);
        
        // Calculate damage: BaseDamage * M * R
        float damage = baseDamage * M * R;

        // Spawn dice projectile with calculated damage
        if (projectilePrefab != null)
        {
            Debug.Log($"[Dice] SPAWNING projectile at {Time.time:F2}");
            GameObject projectileObj = Instantiate(projectilePrefab, tower.transform.position, Quaternion.identity);
            var projectile = projectileObj.GetComponent<TowerOfOdds.Combat.Projectile>();
            
            if (projectile != null)
            {
                projectile.Init(target.transform, damage, projectileSpeed, false);
                
                // Apply speed-up buff after hit
                // Reduce AttackSpeed by (0.03 * R) for 0.75s, clamped to minimum 0.15s
                ApplySpeedUpBuff(tower, R);
                
                Debug.Log($"[Dice] PROJECTILE SPAWNED | Roll: {R} | M: {M:F2} | Damage: {damage:F1} | Speed buff: -{speedUpPerRoll * R:F2}s for {buffDuration}s | Target: {target.name}");
            }
            else
            {
                Debug.LogWarning("[Dice] Projectile prefab missing Projectile component!");
                Destroy(projectileObj);
            }
        }
        else
        {
            // Direct damage fallback
            target.TakeDamage(damage);
            ApplySpeedUpBuff(tower, R);
            Debug.Log($"[Dice] Direct hit: {damage:F1} | Speed buff applied (no projectile)");
        }
    }

    private void ApplySpeedUpBuff(TowerRuntime tower, int rollValue)
    {
        // Calculate speed reduction: 0.03 * R (this is cooldown reduction in seconds)
        float cooldownReduction = speedUpPerRoll * rollValue;
        
        // Apply temporary speed buff
        // Reduce AttackSpeed cooldown by 'cooldownReduction' for 'buffDuration' seconds
        // Clamped to minimum 0.15s cooldown
        tower.ReduceAttackSpeedTemporarily(cooldownReduction, buffDuration, minAttackSpeed: 0.15f);
        
        Debug.Log($"[Dice] Speed-up buff: Reduce cooldown by {cooldownReduction:F2}s for {buffDuration}s (min cooldown 0.15s)");
    }
}
