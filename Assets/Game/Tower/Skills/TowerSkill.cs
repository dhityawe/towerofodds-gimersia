// TowerSkill.cs
// Base ScriptableObject for all tower skills

using UnityEngine;

/// <summary>
/// Base class for all tower skills. Create skill assets via right-click > Create > Tower > Skills.
/// Each skill defines its own damage, behavior, and how it uses tower stats (BaseAttackCount, AttackSpeed).
/// </summary>
public abstract class TowerSkill : ScriptableObject, IShopItem
{
    [Header("Skill Identity")]
    public string skillName = "New Skill";
    [TextArea(2, 4)] public string description = "Skill description here...";
    public Sprite icon;

    [Header("Shop Properties")]
    [Min(0)] public int cost = 100;
    public ShopItemRarity rarity = ShopItemRarity.Common;

    [Header("Base Stats")]
    [Min(0f)] public float baseDamage = 10f;

    /// <summary>
    /// Called when the skill is activated (usually on attack cycle).
    /// </summary>
    /// <param name="tower">The tower runtime instance owning this skill</param>
    /// <param name="slotIndex">Which skill slot (0-2) this skill is in</param>
    /// <param name="diceRoll1">First dice roll (1-6), from Preparation Phase</param>
    /// <param name="diceRoll2">Second dice roll (1-6), from Preparation Phase</param>
    public abstract void Activate(TowerRuntime tower, int slotIndex, int diceRoll1, int diceRoll2);

    /// <summary>
    /// Called every frame for skills that need continuous updates (e.g., orbiting cards).
    /// </summary>
    public virtual void OnUpdate(TowerRuntime tower, int slotIndex, float deltaTime) { }

    /// <summary>
    /// Called when the skill is equipped to a slot (for initialization).
    /// </summary>
    public virtual void OnEquip(TowerRuntime tower, int slotIndex) { }

    /// <summary>
    /// Called when the skill is removed from a slot (for cleanup).
    /// </summary>
    public virtual void OnUnequip(TowerRuntime tower, int slotIndex) { }

    /// <summary>
    /// Helper: Get final damage after dice multiplier.
    /// </summary>
    protected float GetDamageWithDice(int d1, int d2, float extraMultiplier = 1f)
    {
        float diceMultiplier = TowerBase.GetDiceMultiplier(d1, d2);
        return baseDamage * diceMultiplier * Mathf.Max(0f, extraMultiplier);
    }

    // ====== IShopItem Implementation ======
    public virtual string GetName() => skillName;
    public virtual string GetDescription() => description;
    public virtual Sprite GetIcon() => icon;
    public virtual int GetCost() => cost;
    public virtual ShopItemRarity GetRarity() => rarity;
    public virtual ShopItemCategory GetCategory() => ShopItemCategory.Skill;

    public virtual bool CanPurchase(TowerRuntime tower)
    {
        // Check if tower has an empty skill slot
        return tower.GetFirstEmptySlot() != -1;
    }

    public virtual bool OnPurchase(TowerRuntime tower)
    {
        int emptySlot = tower.GetFirstEmptySlot();
        if (emptySlot == -1)
        {
            Debug.LogWarning($"Cannot purchase {skillName}: All skill slots are full!");
            return false;
        }

        tower.EquipSkill(this, emptySlot);
        Debug.Log($"Purchased and equipped {skillName} to slot {emptySlot}");
        return true;
    }
}
