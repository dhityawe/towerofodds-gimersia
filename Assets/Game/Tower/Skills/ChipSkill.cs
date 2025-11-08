// ChipSkill.cs
// Shoots a single chip that spreads based on BaseAttackCount when hitting an enemy

using UnityEngine;

[CreateAssetMenu(fileName = "Chip Skill", menuName = "Tower/Skills/Chip", order = 1)]
public class ChipSkill : TowerSkill
{
    [Header("Chip Specific")]
    [Tooltip("Spread angle range in degrees")]
    public float spreadAngle = 120f;

    public override void Activate(TowerRuntime tower, int slotIndex, int diceRoll1, int diceRoll2)
    {
        float damage = GetDamageWithDice(diceRoll1, diceRoll2);
        int spreadCount = Mathf.Max(1, tower.GetStats().BaseAttackCount);

        // TODO: Spawn chip projectile that will spread on hit
        // ChipProjectile proj = ChipProjectile.Spawn(tower.transform.position, damage, targetEnemy);
        // proj.SetSpreadConfig(spreadCount, spreadAngle, damage * 0.8f); // Spread damage reduced

        Debug.Log($"[Chip] Damage: {damage:F1} | Spread Count: {spreadCount} | Angle: {spreadAngle}°");
    }
}
