// ShopSlotUI.cs
// UI component for displaying a single shop slot
// Attach to each shop slot UI element (3 per shop)

using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Displays a single shop slot with icon, name, cost, rarity, and purchase button.
/// Subscribe to ShopManager.OnShopRefreshed to update displays.
/// </summary>
public class ShopSlotUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI levelStackText; // Shows "Lv.3" or "Stack: 2/5"
    [SerializeField] private Image rarityBorder;
    [SerializeField] private Button purchaseButton;
    [SerializeField] private TextMeshProUGUI buttonText; // "Purchase" or "Sold"

    [Header("Rarity Colors")]
    [SerializeField] private Color commonColor = Color.gray;
    [SerializeField] private Color uncommonColor = Color.green;
    [SerializeField] private Color rareColor = Color.blue;
    [SerializeField] private Color epicColor = new Color(0.6f, 0f, 1f); // Purple
    [SerializeField] private Color legendaryColor = new Color(1f, 0.5f, 0f); // Orange

    [Header("Settings")]
    [SerializeField] private int slotIndex;
    [SerializeField] private ShopManager shopManager;

    private ShopManager.ShopSlot currentSlot;

    void Awake()
    {
        // Find ShopManager if not assigned
        if (shopManager == null)
        {
            shopManager = FindFirstObjectByType<ShopManager>();
        }

        // Wire up purchase button
        if (purchaseButton != null)
        {
            purchaseButton.onClick.AddListener(OnPurchaseClicked);
        }
    }

    void OnEnable()
    {
        // Subscribe to shop refresh events
        if (shopManager != null)
        {
            shopManager.OnShopRefreshed += OnShopRefreshed;
        }
    }

    void OnDisable()
    {
        // Unsubscribe from events
        if (shopManager != null)
        {
            shopManager.OnShopRefreshed -= OnShopRefreshed;
        }
    }

    /// <summary>
    /// Initialize this slot with its index and shop manager reference.
    /// Call this once during setup.
    /// </summary>
    public void Initialize(int index, ShopManager manager)
    {
        slotIndex = index;
        shopManager = manager;
    }

    /// <summary>
    /// Called when shop is refreshed. Updates display for this slot.
    /// </summary>
    private void OnShopRefreshed(ShopManager.ShopSlot[] slots)
    {
        if (slotIndex < 0 || slotIndex >= slots.Length)
        {
            Debug.LogWarning($"ShopSlotUI: Invalid slot index {slotIndex}");
            gameObject.SetActive(false);
            return;
        }

        currentSlot = slots[slotIndex];
        UpdateDisplay(currentSlot);
    }

    /// <summary>
    /// Update the visual display of this shop slot.
    /// </summary>
    public void UpdateDisplay(ShopManager.ShopSlot slot)
    {
        if (slot == null || slot.item == null)
        {
            // Hide slot if no item
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);
        currentSlot = slot;

        // Update icon
        if (iconImage != null)
        {
            iconImage.sprite = slot.item.GetIcon();
            iconImage.enabled = slot.item.GetIcon() != null;
        }

        // Update name
        if (nameText != null)
        {
            nameText.text = slot.item.GetName();
        }

        // Update description
        if (descriptionText != null)
        {
            descriptionText.text = slot.item.GetDescription();
        }

        // Update level/stack info
        UpdateLevelStackInfo(slot.item);

        // Update cost
        if (costText != null)
        {
            costText.text = $"{slot.item.GetCost()} <sprite name=\"chip\">"; // Use chip icon if available
            // Fallback without icon:
            // costText.text = $"{slot.item.GetCost()} Chips";
        }

        // Update rarity border color
        if (rarityBorder != null)
        {
            rarityBorder.color = GetRarityColor(slot.item.GetRarity());
        }

        // Update purchase button state
        UpdatePurchaseButton(slot);
    }

    /// <summary>
    /// Update purchase button interactability and text.
    /// </summary>
    private void UpdatePurchaseButton(ShopManager.ShopSlot slot)
    {
        if (purchaseButton == null) return;

        bool canAfford = PlayerDataManager.Instance.Chips >= slot.item.GetCost();
        bool isPurchased = slot.isPurchased;

        // Check if this is an upgrade/stack scenario
        bool isUpgrade = IsUpgradeOrStack(slot.item);

        // Disable button if already purchased or can't afford
        purchaseButton.interactable = !isPurchased && canAfford;

        // Update button text
        if (buttonText != null)
        {
            if (isPurchased)
            {
                buttonText.text = "SOLD";
            }
            else if (!canAfford)
            {
                buttonText.text = "Too Expensive";
            }
            else if (isUpgrade)
            {
                buttonText.text = "UPGRADE";
            }
            else
            {
                buttonText.text = "Purchase";
            }
        }
    }

    /// <summary>
    /// Update level/stack display for skills and items.
    /// </summary>
    private void UpdateLevelStackInfo(IShopItem item)
    {
        if (levelStackText == null) return;

        // Check if it's a skill with levels
        if (item is TowerSkill skill)
        {
            if (skill.GetLevel() > 1 || skill.CanLevelUp())
            {
                levelStackText.text = $"Lv.{skill.GetLevel()}/{skill.GetMaxLevel()}";
                levelStackText.gameObject.SetActive(true);
            }
            else
            {
                levelStackText.gameObject.SetActive(false);
            }
        }
        // Check if it's an item with stacks
        else if (item is TowerItem towerItem)
        {
            if (towerItem.GetMaxStack() > 0 && towerItem.GetStack() > 0)
            {
                levelStackText.text = $"Stack: {towerItem.GetStack()}/{towerItem.GetMaxStack()}";
                levelStackText.gameObject.SetActive(true);
            }
            else
            {
                levelStackText.gameObject.SetActive(false);
            }
        }
        else
        {
            levelStackText.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Check if this item is an upgrade (skill level up) or stack (item stack up).
    /// </summary>
    private bool IsUpgradeOrStack(IShopItem item)
    {
        if (item is TowerSkill skill)
        {
            return skill.GetLevel() > 1 || skill.CanLevelUp();
        }
        else if (item is TowerItem towerItem)
        {
            return towerItem.GetStack() > 0;
        }
        return false;
    }

    /// <summary>
    /// Called when player clicks the purchase button.
    /// </summary>
    private void OnPurchaseClicked()
    {
        if (shopManager != null)
        {
            shopManager.PurchaseItem(slotIndex);
        }
    }

    /// <summary>
    /// Get the color for a given rarity level.
    /// </summary>
    private Color GetRarityColor(ShopItemRarity rarity)
    {
        switch (rarity)
        {
            case ShopItemRarity.Common:
                return commonColor;
            case ShopItemRarity.Uncommon:
                return uncommonColor;
            case ShopItemRarity.Rare:
                return rareColor;
            case ShopItemRarity.Epic:
                return epicColor;
            case ShopItemRarity.Legendary:
                return legendaryColor;
            default:
                return Color.white;
        }
    }

    /// <summary>
    /// Manually refresh this slot's display.
    /// Useful when chips change and affordability needs updating.
    /// </summary>
    public void RefreshDisplay()
    {
        if (currentSlot != null)
        {
            UpdateDisplay(currentSlot);
        }
    }

    // ====== EDITOR HELPERS ======

#if UNITY_EDITOR
    [ContextMenu("Test Display (Common Item)")]
    private void TestDisplay()
    {
        // Test display with dummy data
        if (nameText != null) nameText.text = "Test Item";
        if (descriptionText != null) descriptionText.text = "This is a test item description showing how the UI looks.";
        if (costText != null) costText.text = "25 Chips";
        if (rarityBorder != null) rarityBorder.color = commonColor;
        if (buttonText != null) buttonText.text = "Purchase";
    }
#endif
}
