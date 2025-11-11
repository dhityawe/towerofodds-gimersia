// StatModifierItem.cs
// Generic stackable item that modifies a single tower stat
// GameJam Best Practice: One flexible class instead of many specific ones

using UnityEngine;

public enum StatModifierType
{
    MaxHpFlat,          // +20 MaxHp
    MaxHpPercent,       // +20% MaxHp
    ArmorFlat,          // +1 Armor
    RegenFlat,          // +0.5 HP/sec
    AttackCountFlat,    // +1 BaseAttackCount
    AttackSpeedMultiply,// AttackSpeed × 0.95 (5% faster)
    AttackSpeedFlat     // AttackSpeed - 0.15s (flat reduction)
}

[CreateAssetMenu(fileName = "New Stat Item", menuName = "Tower/Items/Stat Modifier")]
public class StatModifierItem : TowerItem
{
    [Header("Stat Modification")]
    [Tooltip("Which stat to modify")]
    public StatModifierType statType;
    
    [Tooltip("Value to add/multiply")]
    public float value = 1f;

    [Header("Display")]
    [Tooltip("Show in description (e.g., '+1 Armor')")]
    public string statDisplayName = "Stat";

    // Track total applied for percentage-based items
    private float totalApplied = 0f;

    public override void ApplyEffect(TowerRuntime tower)
    {
        if (tower == null) return;

        float stackMult = GetStackMultiplier();
        float finalValue = value;

        switch (statType)
        {
            case StatModifierType.MaxHpFlat:
                finalValue = value * stackMult;
                tower.AddMaxHp(finalValue);
                totalApplied = finalValue;
                break;

            case StatModifierType.MaxHpPercent:
                // Calculate percentage of current MaxHp
                float currentMaxHp = tower.GetStats().MaxHp;
                finalValue = currentMaxHp * (value / 100f) * stackMult;
                tower.AddMaxHp(finalValue);
                totalApplied = finalValue;
                break;

            case StatModifierType.ArmorFlat:
                finalValue = value * stackMult;
                tower.AddArmor(finalValue);
                totalApplied = finalValue;
                break;

            case StatModifierType.RegenFlat:
                finalValue = value * stackMult;
                tower.AddRegen(finalValue);
                totalApplied = finalValue;
                break;

            case StatModifierType.AttackCountFlat:
                int attackCountIncrease = Mathf.RoundToInt(value * stackMult);
                tower.AddAttackCount(attackCountIncrease);
                totalApplied = attackCountIncrease;
                break;

            case StatModifierType.AttackSpeedMultiply:
                // value = 0.95 means 5% faster (multiply by 0.95 = interval reduced)
                // Stack multiplier doesn't apply to multipliers
                tower.MultiplyAttackSpeed(value);
                totalApplied = value;
                break;

            case StatModifierType.AttackSpeedFlat:
                // value = -0.15 means subtract 0.15s from interval
                tower.AddAttackSpeedFlat(value);
                totalApplied = value;
                break;
        }

        Debug.Log($"[{itemName}] Applied {statType}: {finalValue} (Stack: {GetStack()})");
    }

    protected override void OnStackUp(TowerRuntime tower, int newStack)
    {
        // For flat modifiers, apply the incremental stack bonus
        float incrementalValue = value * (stackBonusPercent / 100f);

        switch (statType)
        {
            case StatModifierType.MaxHpFlat:
                tower.AddMaxHp(incrementalValue);
                totalApplied += incrementalValue;
                break;

            case StatModifierType.MaxHpPercent:
                float hpIncrease = tower.GetStats().MaxHp * (value / 100f) * (stackBonusPercent / 100f);
                tower.AddMaxHp(hpIncrease);
                totalApplied += hpIncrease;
                break;

            case StatModifierType.ArmorFlat:
                tower.AddArmor(incrementalValue);
                totalApplied += incrementalValue;
                break;

            case StatModifierType.RegenFlat:
                tower.AddRegen(incrementalValue);
                totalApplied += incrementalValue;
                break;

            case StatModifierType.AttackCountFlat:
                if (newStack % Mathf.RoundToInt(100f / stackBonusPercent) == 0)
                {
                    tower.AddAttackCount(1);
                    totalApplied += 1;
                }
                break;

            case StatModifierType.AttackSpeedMultiply:
                tower.MultiplyAttackSpeed(value);
                break;

            case StatModifierType.AttackSpeedFlat:
                // Apply incremental flat reduction
                float incrementalFlat = value * (stackBonusPercent / 100f);
                tower.AddAttackSpeedFlat(incrementalFlat);
                break;
        }

        Debug.Log($"[{itemName}] Stack {newStack}: +{incrementalValue}");
    }

    public override string GetDescription()
    {
        if (GetStack() > 0)
        {
            string bonus = GetBonusText();
            return $"{description}\n\n<color=green>Current: {bonus}</color>";
        }
        return description;
    }

    private string GetBonusText()
    {
        switch (statType)
        {
            case StatModifierType.MaxHpFlat:
            case StatModifierType.MaxHpPercent:
                return $"+{totalApplied:F0} Max HP";
            
            case StatModifierType.ArmorFlat:
                return $"+{totalApplied:F1} Armor";
            
            case StatModifierType.RegenFlat:
                return $"+{totalApplied:F1} HP/sec";
            
            case StatModifierType.AttackCountFlat:
                return $"+{totalApplied:F0} Attack Count";
            
            case StatModifierType.AttackSpeedMultiply:
                float percent = (1f - value) * 100f;
                return $"+{percent:F0}% Attack Speed";
            
            case StatModifierType.AttackSpeedFlat:
                return $"+{Mathf.Abs(totalApplied):F2} Attack Speed";
            
            default:
                return $"+{totalApplied}";
        }
    }

    public override void ResetStack()
    {
        base.ResetStack();
        totalApplied = 0f;
    }
}
