// HealthPotionItem.cs
// Example consumable item that heals the tower

using UnityEngine;

[CreateAssetMenu(fileName = "Health Potion", menuName = "Tower/Items/Health Potion", order = 0)]
public class HealthPotionItem : TowerItem
{
    [Header("Health Potion Settings")]
    [Tooltip("Amount of HP to restore")]
    public float healAmount = 50f;

    public override void ApplyEffect(TowerRuntime tower)
    {
        tower.Heal(healAmount);
        Debug.Log($"[Health Potion] Healed tower for {healAmount} HP");
    }
}
