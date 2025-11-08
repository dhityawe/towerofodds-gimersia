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

    /// <summary>
    /// Called when the item is purchased/used.
    /// Apply stat modifications, spawn effects, etc.
    /// </summary>
    public abstract void ApplyEffect(TowerRuntime tower);

    /// <summary>
    /// Optional: Called when item effect expires (for temporary items).
    /// </summary>
    public virtual void RemoveEffect(TowerRuntime tower) { }

    // ====== IShopItem Implementation ======
    public virtual string GetName() => itemName;
    public virtual string GetDescription() => description;
    public virtual Sprite GetIcon() => icon;
    public virtual int GetCost() => cost;
    public virtual ShopItemRarity GetRarity() => rarity;
    public virtual ShopItemCategory GetCategory() => ShopItemCategory.Item;

    public virtual bool CanPurchase(TowerRuntime tower)
    {
        // Items can always be purchased (no inventory limit for now)
        return true;
    }

    public virtual bool OnPurchase(TowerRuntime tower)
    {
        ApplyEffect(tower);
        Debug.Log($"Purchased and applied {itemName}");
        return true;
    }
}
