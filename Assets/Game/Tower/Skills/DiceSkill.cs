// DiceSkill.cs
// Shoots a single dice projectile; damage randomized 1-6, increases AttackSpeed based on roll

using UnityEngine;

[CreateAssetMenu(fileName = "Dice Skill", menuName = "Tower/Skills/Dice", order = 0)]
public class DiceSkill : TowerSkill
{
    [Header("Dice Specific")]
    [Tooltip("Attack speed multiplier per damage point (e.g., 0.05 = 5% faster per point)")]
    public float attackSpeedGainPerDamage = 0.05f;

    public override void Activate(TowerRuntime tower, int slotIndex, int diceRoll1, int diceRoll2)
    {
        // Roll a single die (1-6) for damage variance
        int singleRoll = Random.Range(1, 7);
        float damage = baseDamage * singleRoll;

        // Temporarily boost attack speed based on roll
        float speedBoost = 1f + (singleRoll * attackSpeedGainPerDamage);
        tower.MultiplyAttackSpeed(speedBoost);

        // TODO: Spawn dice projectile with calculated damage
        // DiceProjectile.Spawn(tower.transform.position, damage, targetEnemy);

        Debug.Log($"[Dice] Roll: {singleRoll} | Damage: {damage:F1} | Speed Boost: {speedBoost:F2}x");
    }
}
