// TowerItem.cs
// Base ScriptableObject for tower items (permanent stat boosts, consumables, etc.)

using UnityEngine;

/// <summary>
/// Base class for all tower items. Items provide permanent or temporary stat modifications.
/// Examples: Health Potion, Armor Plate, Damage Ring, etc.
/// </summary>
public abstract class TowerItem : ScriptableObject, IShopItem
{
    [Header("Item Identity")]
    public string itemName = "New Item";
    [TextArea(2, 4)] public string description = "Item description here...";
    public Sprite icon;

    [Header("Shop Properties")]
    [Min(0)] public int cost = 50;
    public ShopItemRarity rarity = ShopItemRarity.Common;

    [Header("Stacking System")]
    [Tooltip("Max stack count (0 = no stacking, just one-time use)")]
    [Min(0)] public int maxStack = 5;
    [Tooltip("Base stat increase percentage per stack (e.g., 1 = 1% per stack)")]
    [Range(0f, 100f)] public float stackBonusPercent = 1f; // 1% per stack by default

    [Header("Runtime State (Don't Edit)")]
    [SerializeField] private int currentStack = 0;

    /// <summary>
    /// Called when the item is purchased/used.
    /// Apply stat modifications, spawn effects, etc.
    /// </summary>
    public abstract void ApplyEffect(TowerRuntime tower);

    /// <summary>
    /// Optional: Called when item effect expires (for temporary items).
    /// </summary>
    public virtual void RemoveEffect(TowerRuntime tower) { }

    /// <summary>
    /// Get current stack count.
    /// </summary>
    public int GetStack() => currentStack;

    /// <summary>
    /// Get max stack count.
    /// </summary>
    public int GetMaxStack() => maxStack;

    /// <summary>
    /// Check if this item can be stacked further.
    /// </summary>
    public bool CanStack() => maxStack > 0 && currentStack < maxStack;

    /// <summary>
    /// Add a stack to this item. Returns true if successful.
    /// Override this in derived classes to add custom stack effects.
    /// </summary>
    public virtual bool StackUp(TowerRuntime tower)
    {
        if (!CanStack())
        {
            Debug.LogWarning($"{itemName} is already at max stack ({maxStack})!");
            return false;
        }

        currentStack++;
        OnStackUp(tower, currentStack);
        Debug.Log($"{itemName} stacked up to {currentStack}/{maxStack}!");
        return true;
    }

    /// <summary>
    /// Called when item is stacked. Override to add custom stack effects.
    /// Base implementation increases stats by stackBonusPercent per stack.
    /// </summary>
    protected virtual void OnStackUp(TowerRuntime tower, int newStack)
    {
        // Default: re-apply effect with new stack multiplier
        // Override in derived items for custom stacking behavior
        ApplyEffect(tower);
    }

    /// <summary>
    /// Get the stack multiplier for this item's effect.
    /// Example: stack 3 with 1% bonus = 1.03x multiplier
    /// </summary>
    protected float GetStackMultiplier()
    {
        if (maxStack == 0 || currentStack == 0) return 1f;
        return 1f + (currentStack * stackBonusPercent / 100f);
    }

    /// <summary>
    /// Reset stack to 0 (for new game or testing).
    /// </summary>
    public virtual void ResetStack()
    {
        currentStack = 0;
    }

    // ====== IShopItem Implementation ======
    public virtual string GetName() => itemName;
    public virtual string GetDescription() => description;
    public virtual Sprite GetIcon() => icon;
    public virtual int GetCost() => cost;
    public virtual ShopItemRarity GetRarity() => rarity;
    public virtual ShopItemCategory GetCategory() => ShopItemCategory.Item;

    public virtual bool CanPurchase(TowerRuntime tower)
    {
        // Items can always be purchased (will stack if already owned)
        return true;
    }

    public virtual bool OnPurchase(TowerRuntime tower)
    {
        // Check if this item supports stacking and already has stacks
        if (maxStack > 0 && currentStack > 0)
        {
            // Stack it up
            return StackUp(tower);
        }
        else if (maxStack > 0 && currentStack == 0)
        {
            // First purchase of stackable item
            currentStack = 1;
            ApplyEffect(tower);
            Debug.Log($"Purchased {itemName} (Stack: {currentStack}/{maxStack})");
            return true;
        }
        else
        {
            // One-time use item (maxStack = 0)
            ApplyEffect(tower);
            Debug.Log($"Purchased and applied {itemName}");
            return true;
        }
    }
}
