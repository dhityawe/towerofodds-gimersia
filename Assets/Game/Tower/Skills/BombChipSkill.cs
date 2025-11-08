// BombChipSkill.cs
// Shoots a chip that explodes on enemy hit, damaging enemies in radius

using UnityEngine;

[CreateAssetMenu(fileName = "BombChip Skill", menuName = "Tower/Skills/BombChip", order = 4)]
public class BombChipSkill : TowerSkill
{
    [Header("BombChip Specific")]
    [Tooltip("Explosion radius from hit point")]
    public float explosionRadius = 2.5f;
    [Tooltip("Damage multiplier for explosion (relative to base)")]
    public float explosionDamageMultiplier = 0.8f;

    public override void Activate(TowerRuntime tower, int slotIndex, int diceRoll1, int diceRoll2)
    {
        float directDamage = GetDamageWithDice(diceRoll1, diceRoll2);
        float explosionDamage = directDamage * explosionDamageMultiplier;

        // TODO: Spawn bomb chip projectile
        // BombChipProjectile proj = BombChipProjectile.Spawn(tower.transform.position, directDamage, targetEnemy);
        // proj.SetExplosionConfig(explosionRadius, explosionDamage);

        Debug.Log($"[BombChip] Direct Damage: {directDamage:F1} | Explosion Damage: {explosionDamage:F1} | Radius: {explosionRadius}");
    }
}
