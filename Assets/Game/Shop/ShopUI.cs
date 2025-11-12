// ShopUI.cs
// Example UI controller for the shop (attach to your shop UI canvas)

using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Simple UI controller for displaying and interacting with the shop.
/// Attach this to your Shop UI Canvas/Panel.
/// </summary>
public class ShopUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ShopManager shopManager;
    [SerializeField] private ShopSlotUI[] slotUIs = new ShopSlotUI[ShopManager.SHOP_SLOT_COUNT];

    [Header("UI Panels")]
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private Button continueButton;
    [SerializeField] private TextMeshProUGUI errorMessageText;

    [System.Serializable]
    public class ShopSlotUI
    {
        public GameObject slotObject;
        public Image iconImage;
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI descriptionText;
        public TextMeshProUGUI costText;
        public TextMeshProUGUI categoryText;
        public Button purchaseButton;
        public Image rarityBorder;
        public GameObject purchasedOverlay;
    }

    void OnEnable()
    {
        if (shopManager != null)
        {
            shopManager.OnShowShop += OnShowShop;
            shopManager.OnHideShop += OnHideShop;
            shopManager.OnShopRefreshed += OnShopRefreshed;
            shopManager.OnItemPurchased += OnItemPurchased;
            shopManager.OnPurchaseFailed += OnPurchaseFailed;
        }
    }

    void OnDisable()
    {
        if (shopManager != null)
        {
            shopManager.OnShowShop -= OnShowShop;
            shopManager.OnHideShop -= OnHideShop;
            shopManager.OnShopRefreshed -= OnShopRefreshed;
            shopManager.OnItemPurchased -= OnItemPurchased;
            shopManager.OnPurchaseFailed -= OnPurchaseFailed;
        }
    }

    void Start()
    {
        // Wire up purchase buttons
        for (int i = 0; i < slotUIs.Length; i++)
        {
            int slotIndex = i; // Capture for lambda
            slotUIs[i].purchaseButton?.onClick.AddListener(() => OnPurchaseButtonClicked(slotIndex));
        }

        // Wire up continue button
        if (continueButton != null)
        {
            continueButton.onClick.AddListener(OnContinueButtonClicked);
        }

        // Hide shop initially
        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
        }
    }

    public void OpenShop()
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(true);
        }
        RefreshUI();
    }

    public void CloseShop()
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
        }
    }

    public void RefreshShop()
    {
        shopManager?.RefreshShop();
    }

    private void OnShopRefreshed(ShopManager.ShopSlot[] slots)
    {
        RefreshUI();
    }

    private void OnShowShop()
    {
        OpenShop();
    }

    private void OnHideShop()
    {
        CloseShop();
    }

    private void RefreshUI()
    {
        var slots = shopManager.GetCurrentSlots();

        for (int i = 0; i < slotUIs.Length; i++)
        {
            if (i >= slots.Length || slots[i]?.item == null)
            {
                // Hide empty slot
                slotUIs[i].slotObject?.SetActive(false);
                continue;
            }

            var slot = slots[i];
            var ui = slotUIs[i];

            ui.slotObject?.SetActive(true);

            // Update icon
            if (ui.iconImage != null)
            {
                ui.iconImage.sprite = slot.item.GetIcon();
            }

            // Update text
            if (ui.nameText != null)
            {
                ui.nameText.text = slot.item.GetName();
            }

            if (ui.descriptionText != null)
            {
                ui.descriptionText.text = slot.item.GetDescription();
            }

            if (ui.costText != null)
            {
                ui.costText.text = $"{slot.item.GetCost()}g";
            }

            if (ui.categoryText != null)
            {
                ui.categoryText.text = slot.item.GetCategory().ToString();
            }

            // Rarity border color
            if (ui.rarityBorder != null)
            {
                ui.rarityBorder.color = GetRarityColor(slot.item.GetRarity());
            }

            // Purchase state
            if (ui.purchasedOverlay != null)
            {
                ui.purchasedOverlay.SetActive(slot.isPurchased);
            }

            if (ui.purchaseButton != null)
            {
                ui.purchaseButton.interactable = !slot.isPurchased;
            }
        }
    }

    private void OnPurchaseButtonClicked(int slotIndex)
    {
        shopManager?.PurchaseItem(slotIndex);
    }

    private void OnContinueButtonClicked()
    {
        Debug.Log("[ShopUI] Continue button clicked. Closing shop...");
        
        // Find OpenShopState and tell it to close
        var gameStateManager = GameStateManager.Instance;
        if (gameStateManager != null && gameStateManager.CurrentState is OpenShopState openShopState)
        {
            openShopState.CloseShop();
        }
        else
        {
            Debug.LogWarning("[ShopUI] Could not find OpenShopState to close shop!");
            // Fallback: just hide the panel
            CloseShop();
        }
    }

    private void OnItemPurchased(int slotIndex, IShopItem item)
    {
        Debug.Log($"UI: Item purchased from slot {slotIndex}: {item.GetName()}");
        RefreshUI();
        
        // Optional: Show purchase success feedback
        // ShowMessage($"Purchased {item.GetName()}!", Color.green);
    }

    private void OnPurchaseFailed(string errorMessage)
    {
        Debug.LogWarning($"UI: Purchase failed - {errorMessage}");
        ShowErrorMessage(errorMessage);
    }

    private void ShowErrorMessage(string message)
    {
        if (errorMessageText != null)
        {
            errorMessageText.text = message;
            errorMessageText.gameObject.SetActive(true);
            // Optional: Hide after delay
            Invoke(nameof(HideErrorMessage), 3f);
        }
    }

    private void HideErrorMessage()
    {
        if (errorMessageText != null)
        {
            errorMessageText.gameObject.SetActive(false);
        }
    }

    private Color GetRarityColor(ShopItemRarity rarity)
    {
        switch (rarity)
        {
            case ShopItemRarity.Common: return new Color(0.7f, 0.7f, 0.7f); // Gray
            case ShopItemRarity.Uncommon: return new Color(0.2f, 0.8f, 0.2f); // Green
            case ShopItemRarity.Rare: return new Color(0.2f, 0.5f, 1f); // Blue
            case ShopItemRarity.Epic: return new Color(0.7f, 0.2f, 0.9f); // Purple
            case ShopItemRarity.Legendary: return new Color(1f, 0.6f, 0.1f); // Orange
            default: return Color.white;
        }
    }
}
