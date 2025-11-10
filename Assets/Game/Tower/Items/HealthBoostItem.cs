// HealthBoostItem.cs
// Example stackable item: Increases max HP by a percentage per stack

using UnityEngine;

[CreateAssetMenu(fileName = "HealthBoost", menuName = "Tower/Items/Health Boost")]
public class HealthBoostItem : TowerItem
{
    [Header("Health Boost Settings")]
    [Tooltip("Base HP increase (flat)")]
    [Min(0f)] public float baseHpIncrease = 20f;

    public override void ApplyEffect(TowerRuntime tower)
    {
        if (tower == null) return;

        // Get stack multiplier (1% per stack by default)
        float stackMult = GetStackMultiplier();
        
        // Calculate final HP increase with stacking
        float finalHpIncrease = baseHpIncrease * stackMult;
        
        tower.AddMaxHp(finalHpIncrease);
        
        Debug.Log($"Applied {itemName}: +{finalHpIncrease} Max HP (Base: {baseHpIncrease}, Stack Mult: {stackMult}x)");
    }

    protected override void OnStackUp(TowerRuntime tower, int newStack)
    {
        // Reapply effect doesn't work for flat increases
        // Instead, calculate the additional HP from this new stack
        float additionalHp = baseHpIncrease * (stackBonusPercent / 100f);
        tower.AddMaxHp(additionalHp);
        
        Debug.Log($"{itemName} stacked! Added {additionalHp} HP from stack bonus");
    }

    public override string GetDescription()
    {
        if (GetStack() > 0)
        {
            float stackMult = GetStackMultiplier();
            return $"{description}\n\n<color=green>Current: +{baseHpIncrease * stackMult} Max HP</color>";
        }
        return description;
    }
}
