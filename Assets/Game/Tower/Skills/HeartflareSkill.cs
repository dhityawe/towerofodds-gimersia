// HeartflareSkill.cs
// Pink light pulses from small to max radius; pulse speed based on AttackSpeed

using UnityEngine;

[CreateAssetMenu(fileName = "Heartflare Skill", menuName = "Tower/Skills/Heartflare", order = 3)]
public class HeartflareSkill : TowerSkill
{
    [Header("Heartflare Specific")]
    [Tooltip("Maximum pulse radius")]
    public float maxRadius = 5f;
    [Tooltip("Time for pulse to expand from 0 to maxRadius (base, modified by AttackSpeed)")]
    public float basePulseDuration = 1f;
    [Tooltip("Visual effect color")]
    public Color pulseColor = new Color(1f, 0.4f, 0.7f, 0.5f);

    public override void Activate(TowerRuntime tower, int slotIndex, int diceRoll1, int diceRoll2)
    {
        float damage = GetDamageWithDice(diceRoll1, diceRoll2);
        float pulseDuration = basePulseDuration / tower.GetStats().AttackSpeed;

        // TODO: Spawn pulse effect that expands over pulseDuration
        // HeartflarePulse.Spawn(tower.transform.position, maxRadius, pulseDuration, damage, pulseColor);

        Debug.Log($"[Heartflare] Damage: {damage:F1} | Max Radius: {maxRadius} | Duration: {pulseDuration:F2}s");
    }
}
