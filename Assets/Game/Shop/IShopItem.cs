// IShopItem.cs
// Interface for all items that can appear in the shop (Skills, Items, Arcanes)

using UnityEngine;

/// <summary>
/// Interface for anything that can be sold in the shop.
/// Implement this on ScriptableObjects for Skills, Items, Arcanes, etc.
/// </summary>
public interface IShopItem
{
    /// <summary>Display name shown in shop UI.</summary>
    string GetName();

    /// <summary>Description text shown in shop UI.</summary>
    string GetDescription();

    /// <summary>Icon sprite for shop UI.</summary>
    Sprite GetIcon();

    /// <summary>Purchase cost (gold, currency, etc.).</summary>
    int GetCost();

    /// <summary>Item rarity/tier for visuals and sorting.</summary>
    ShopItemRarity GetRarity();

    /// <summary>Category for filtering (Skill, Item, Arcane).</summary>
    ShopItemCategory GetCategory();

    /// <summary>
    /// Called when player purchases this item.
    /// Return true if purchase succeeds, false if it fails (e.g., inventory full).
    /// </summary>
    bool OnPurchase(TowerRuntime tower);

    /// <summary>Can this item be purchased right now? (Check inventory space, requirements, etc.)</summary>
    bool CanPurchase(TowerRuntime tower);
}

public enum ShopItemRarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}

public enum ShopItemCategory
{
    Skill,
    Item,
    Arcane
}
