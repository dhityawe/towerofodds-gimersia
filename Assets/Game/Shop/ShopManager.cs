// ShopManager.cs
// Manages shop slots, randomization, and purchase flow
// Supports 3 shop types: Skill Shop, Items Shop, Arcane Shop

using System;
using System.Collections.Generic;
using UnityEngine;

public enum ShopType
{
    Skills,     // Skill shop (abilities for tower)
    Items,      // Item shop (consumables, stat boosts)
    Arcanes     // Arcane shop (dice modifiers, powerful passives)
}

public class ShopManager : MonoBehaviour
{
    public const int SHOP_SLOT_COUNT = 3;

    [Header("Shop Type")]
    [SerializeField] private ShopType currentShopType = ShopType.Items;

    [Header("Shop Configuration")]
    [Tooltip("Pool of Skills")]
    [SerializeField] private List<TowerSkill> skillPool = new List<TowerSkill>();
    
    [Tooltip("Pool of Items")]
    [SerializeField] private List<TowerItem> itemPool = new List<TowerItem>();
    
    [Tooltip("Pool of Arcanes (Dice Modifiers)")]
    [SerializeField] private List<TowerArcane> arcanePool = new List<TowerArcane>();

    [Tooltip("Reference to the player's tower")]
    [SerializeField] private TowerRuntime playerTower;

    [Header("Rarity Weights (for random generation)")]
    [SerializeField] private RarityWeights rarityWeights = new RarityWeights
    {
        common = 50f,
        uncommon = 30f,
        rare = 15f,
        epic = 4f,
        legendary = 1f
    };

    [Header("Current Shop Slots (Read-Only)")]
    [SerializeField] private ShopSlot[] currentSlots = new ShopSlot[SHOP_SLOT_COUNT];

    // Events for UI updates
    public event Action<ShopSlot[]> OnShopRefreshed;
    public event Action<int, IShopItem> OnItemPurchased; // slotIndex, item
    public event Action<string> OnPurchaseFailed; // error message

    [System.Serializable]
    public class RarityWeights
    {
        public float common = 50f;
        public float uncommon = 30f;
        public float rare = 15f;
        public float epic = 4f;
        public float legendary = 1f;

        public float GetWeight(ShopItemRarity rarity)
        {
            switch (rarity)
            {
                case ShopItemRarity.Common: return common;
                case ShopItemRarity.Uncommon: return uncommon;
                case ShopItemRarity.Rare: return rare;
                case ShopItemRarity.Epic: return epic;
                case ShopItemRarity.Legendary: return legendary;
                default: return common;
            }
        }
    }

    [System.Serializable]
    public class ShopSlot
    {
        public IShopItem item;
        public bool isPurchased;

        public ShopSlot(IShopItem item)
        {
            this.item = item;
            this.isPurchased = false;
        }
    }

    void Start()
    {
        if (playerTower == null)
        {
            playerTower = FindFirstObjectByType<TowerRuntime>();
        }

        RefreshShop();
    }

    /// <summary>
    /// Regenerate all shop slots with random items based on current shop type.
    /// </summary>
    public void RefreshShop()
    {
        RefreshShop(currentShopType);
    }

    /// <summary>
    /// Regenerate shop with specific shop type.
    /// </summary>
    public void RefreshShop(ShopType shopType)
    {
        currentShopType = shopType;

        // Get appropriate pool based on shop type
        List<IShopItem> validItems = GetItemPoolForShopType(shopType);

        if (validItems.Count == 0)
        {
            Debug.LogWarning($"ShopManager: {shopType} pool is empty! Add items to the pool.");
            return;
        }

        // Generate random slots
        for (int i = 0; i < SHOP_SLOT_COUNT; i++)
        {
            IShopItem randomItem = GetWeightedRandomItem(validItems);
            currentSlots[i] = new ShopSlot(randomItem);
        }

        OnShopRefreshed?.Invoke(currentSlots);
        Debug.Log($"Shop refreshed with {SHOP_SLOT_COUNT} {shopType} items");
    }

    /// <summary>
    /// Get the appropriate item pool based on shop type.
    /// </summary>
    private List<IShopItem> GetItemPoolForShopType(ShopType shopType)
    {
        List<IShopItem> validItems = new List<IShopItem>();

        switch (shopType)
        {
            case ShopType.Skills:
                // If all skill slots are full, only show equipped skills (for upgrades)
                if (playerTower != null && playerTower.GetFirstEmptySlot() == -1)
                {
                    TowerSkill[] equippedSkills = playerTower.GetAllSkills();
                    foreach (var skill in equippedSkills)
                    {
                        if (skill != null && skill.CanLevelUp())
                        {
                            validItems.Add(skill);
                        }
                    }
                    Debug.Log($"Skill slots full - showing {validItems.Count} upgradeable equipped skills");
                }
                else
                {
                    // Normal case: show all skills from pool
                    foreach (var skill in skillPool)
                    {
                        if (skill != null) validItems.Add(skill);
                    }
                }
                break;

            case ShopType.Items:
                foreach (var item in itemPool)
                {
                    if (item != null) validItems.Add(item);
                }
                break;

            case ShopType.Arcanes:
                foreach (var arcane in arcanePool)
                {
                    if (arcane != null) validItems.Add(arcane);
                }
                break;
        }

        return validItems;
    }

    /// <summary>
    /// Purchase an item from a specific slot.
    /// </summary>
    public bool PurchaseItem(int slotIndex)
    {
        // Validate slot index
        if (slotIndex < 0 || slotIndex >= SHOP_SLOT_COUNT)
        {
            Debug.LogError($"Invalid slot index: {slotIndex}");
            return false;
        }

        ShopSlot slot = currentSlots[slotIndex];

        // Check if already purchased
        if (slot.isPurchased)
        {
            string msg = "Item already purchased!";
            OnPurchaseFailed?.Invoke(msg);
            Debug.LogWarning(msg);
            return false;
        }

        // Check if can purchase
        if (!slot.item.CanPurchase(playerTower))
        {
            string msg = $"Cannot purchase {slot.item.GetName()}: Requirements not met (e.g., inventory full)";
            OnPurchaseFailed?.Invoke(msg);
            Debug.LogWarning(msg);
            return false;
        }

        // Check currency (Chips)
        int cost = slot.item.GetCost();
        if (!PlayerDataManager.Instance.HasEnoughChips(cost))
        {
            string msg = $"Not enough Chips! Need {cost}, have {PlayerDataManager.Instance.Chips}";
            OnPurchaseFailed?.Invoke(msg);
            Debug.LogWarning(msg);
            return false;
        }

        // Attempt purchase
        bool success = slot.item.OnPurchase(playerTower);

        if (success)
        {
            // Deduct chips
            PlayerDataManager.Instance.SpendChips(cost);

            slot.isPurchased = true;
            OnItemPurchased?.Invoke(slotIndex, slot.item);
            Debug.Log($"Successfully purchased {slot.item.GetName()} for {cost} chips");
            return true;
        }
        else
        {
            string msg = $"Purchase failed for {slot.item.GetName()}";
            OnPurchaseFailed?.Invoke(msg);
            return false;
        }
    }

    /// <summary>
    /// Get weighted random item based on rarity.
    /// </summary>
    private IShopItem GetWeightedRandomItem(List<IShopItem> items)
    {
        // Build weighted list
        List<float> weights = new List<float>();
        foreach (var item in items)
        {
            float weight = rarityWeights.GetWeight(item.GetRarity());
            weights.Add(weight);
        }

        // Calculate total weight
        float totalWeight = 0f;
        foreach (float w in weights)
        {
            totalWeight += w;
        }

        // Random pick
        float randomValue = UnityEngine.Random.Range(0f, totalWeight);
        float cumulative = 0f;

        for (int i = 0; i < items.Count; i++)
        {
            cumulative += weights[i];
            if (randomValue <= cumulative)
            {
                return items[i];
            }
        }

        // Fallback (shouldn't happen)
        return items[UnityEngine.Random.Range(0, items.Count)];
    }

    /// <summary>
    /// Get current shop slots (for UI display).
    /// </summary>
    public ShopSlot[] GetCurrentSlots() => currentSlots;

    /// <summary>
    /// Get a specific slot.
    /// </summary>
    public ShopSlot GetSlot(int index)
    {
        if (index < 0 || index >= SHOP_SLOT_COUNT) return null;
        return currentSlots[index];
    }

    /// <summary>
    /// Manually add items to the appropriate pool (for runtime population).
    /// </summary>
    public void AddToPool(ScriptableObject item)
    {
        if (item is TowerSkill skill)
        {
            skillPool.Add(skill);
        }
        else if (item is TowerItem towerItem)
        {
            itemPool.Add(towerItem);
        }
        else if (item is TowerArcane arcane)
        {
            arcanePool.Add(arcane);
        }
        else
        {
            Debug.LogWarning($"Cannot add {item.name} to shop pool: not a TowerSkill, TowerItem, or TowerArcane");
        }
    }

    /// <summary>
    /// Load all items from Resources folder (useful for auto-population).
    /// </summary>
    public void LoadItemsFromResources(string resourcePath = "Shop")
    {
        // Load all ScriptableObjects from Resources/Shop
        var skills = Resources.LoadAll<TowerSkill>(resourcePath);
        var items = Resources.LoadAll<TowerItem>(resourcePath);
        var arcanes = Resources.LoadAll<TowerArcane>(resourcePath);

        skillPool.Clear();
        itemPool.Clear();
        arcanePool.Clear();

        skillPool.AddRange(skills);
        itemPool.AddRange(items);
        arcanePool.AddRange(arcanes);

        Debug.Log($"Loaded {skillPool.Count} skills, {itemPool.Count} items, {arcanePool.Count} arcanes from Resources/{resourcePath}");
    }

    /// <summary>
    /// Get current shop type.
    /// </summary>
    public ShopType GetCurrentShopType() => currentShopType;

    /// <summary>
    /// Set shop type (useful for UI switching between shops).
    /// </summary>
    public void SetShopType(ShopType shopType)
    {
        currentShopType = shopType;
    }

#if UNITY_EDITOR
    [ContextMenu("Debug: Print Current Shop")]
    private void DebugPrintShop()
    {
        Debug.Log("=== Current Shop ===");
        for (int i = 0; i < currentSlots.Length; i++)
        {
            var slot = currentSlots[i];
            if (slot?.item != null)
            {
                Debug.Log($"Slot {i}: {slot.item.GetName()} ({slot.item.GetCategory()}) - {slot.item.GetCost()}g - {slot.item.GetRarity()} - Purchased: {slot.isPurchased}");
            }
            else
            {
                Debug.Log($"Slot {i}: Empty");
            }
        }
    }
#endif
}
