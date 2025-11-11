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
    [SerializeField] private Button purchaseButton;
    [SerializeField] private TextMeshProUGUI buttonText; // Shows price or "Sold!"

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
        // Subscribe to shop events
        if (shopManager != null)
        {
            shopManager.OnShopRefreshed += OnShopRefreshed;
            shopManager.OnItemPurchased += OnItemPurchased;
        }
        
        // Subscribe to chip changes
        PlayerDataManager.Instance.OnChipsChanged += OnChipsChanged;
    }

    void OnDisable()
    {
        // Unsubscribe from events
        if (shopManager != null)
        {
            shopManager.OnShopRefreshed -= OnShopRefreshed;
            shopManager.OnItemPurchased -= OnItemPurchased;
        }
        
        // Unsubscribe from chip changes
        if (PlayerDataManager.Instance != null)
        {
            PlayerDataManager.Instance.OnChipsChanged -= OnChipsChanged;
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
    /// Called when any item is purchased. Updates this slot if it was the purchased one.
    /// </summary>
    private void OnItemPurchased(int purchasedSlotIndex, IShopItem item)
    {
        // Update this slot if it was purchased
        if (purchasedSlotIndex == slotIndex)
        {
            // Get the latest slot data from shop manager
            var slots = shopManager.GetCurrentSlots();
            if (slotIndex < slots.Length)
            {
                currentSlot = slots[slotIndex];
                UpdateDisplay(currentSlot);
                Debug.Log($"[ShopSlotUI {slotIndex}] Updated display after purchase: {item.GetName()}");
            }
        }
        else
        {
            // Another slot was purchased - update button affordability
            // (chips changed, so we might not be able to afford this anymore)
            UpdatePurchaseButton(currentSlot);
        }
    }

    /// <summary>
    /// Called when player's chips change. Updates button affordability.
    /// </summary>
    private void OnChipsChanged(int newChipAmount)
    {
        if (currentSlot != null)
        {
            UpdatePurchaseButton(currentSlot);
        }
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

        // Cache item reference
        IShopItem item = slot.item;

        // Update icon - ensure proper sprite assignment and visibility
        if (iconImage != null)
        {
            Sprite itemIcon = item.GetIcon();
            iconImage.sprite = itemIcon;
            iconImage.enabled = itemIcon != null;
            
            if (itemIcon != null)
            {
                iconImage.color = Color.white; // Ensure visible
                
                // Force native size if image is too small/large
                if (iconImage.type == Image.Type.Simple && iconImage.preserveAspect)
                {
                    iconImage.SetNativeSize();
                }
            }
            else
            {
                Debug.LogWarning($"[ShopSlotUI {slotIndex}] '{item.GetName()}' missing icon in ScriptableObject!");
            }
        }

        // Update name
        if (nameText != null)
        {
            nameText.text = item.GetName();
        }

        // Update description - compact format with overflow handling
        if (descriptionText != null)
        {
            string desc = item.GetDescription();
            // Remove excessive line breaks and compact whitespace
            desc = System.Text.RegularExpressions.Regex.Replace(desc, @"\n{2,}", "\n");
            desc = desc.Trim();
            descriptionText.text = desc;
            
            // Enable text overflow handling (requires TextMeshProUGUI)
            if (descriptionText is TMPro.TextMeshProUGUI tmpText)
            {
                tmpText.overflowMode = TMPro.TextOverflowModes.Truncate;
            }
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

        // Disable button if already purchased or can't afford
        purchaseButton.interactable = !isPurchased && canAfford;

        // Update button text - show price or "Sold!"
        if (buttonText != null)
        {
            if (isPurchased)
            {
                buttonText.text = "Sold!";
            }
            else
            {
                // Show price number only
                buttonText.text = $"{slot.item.GetCost()}";
            }
        }
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


}
