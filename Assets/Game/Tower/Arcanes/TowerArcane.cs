// TowerArcane.cs
// Base ScriptableObject for tower arcanes (powerful passive abilities, modifiers)

using UnityEngine;

/// <summary>
/// Base class for all tower arcanes. Arcanes are powerful passive effects or modifiers.
/// Examples: Lifesteal Aura, Critical Strike Chance, Multishot, etc.
/// Unlike items (instant/temporary), arcanes provide persistent effects.
/// </summary>
public abstract class TowerArcane : ScriptableObject, IShopItem
{
    [Header("Arcane Identity")]
    public string arcaneName = "New Arcane";
    [TextArea(2, 4)] public string description = "Arcane description here...";
    public Sprite icon;

    [Header("Shop Properties")]
    [Min(0)] public int cost = 200;
    public ShopItemRarity rarity = ShopItemRarity.Rare;

    /// <summary>
    /// Called when arcane is acquired. Set up passive effects.
    /// </summary>
    public abstract void OnAcquire(TowerRuntime tower);

    /// <summary>
    /// Called every frame to update arcane effects (if needed).
    /// </summary>
    public virtual void OnUpdate(TowerRuntime tower, float deltaTime) { }

    /// <summary>
    /// Called when arcane is removed (for testing/respec).
    /// </summary>
    public virtual void OnRemove(TowerRuntime tower) { }

    // ====== IShopItem Implementation ======
    public virtual string GetName() => arcaneName;
    public virtual string GetDescription() => description;
    public virtual Sprite GetIcon() => icon;
    public virtual int GetCost() => cost;
    public virtual ShopItemRarity GetRarity() => rarity;
    public virtual ShopItemCategory GetCategory() => ShopItemCategory.Arcane;

    public virtual bool CanPurchase(TowerRuntime tower)
    {
        // Arcanes can always be purchased (no limit for now)
        // You could add a max arcane count check here
        return true;
    }

    public virtual bool OnPurchase(TowerRuntime tower)
    {
        OnAcquire(tower);
        Debug.Log($"Purchased arcane: {arcaneName}");
        return true;
    }
}
