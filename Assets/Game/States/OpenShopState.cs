// OpenShopState.cs
// State: Shop is open for player to purchase items

using UnityEngine;

/// <summary>
/// Open Shop State: Display shop UI and allow purchases.
/// Supports 3 shop types: Skills, Items, Arcanes.
/// Transition: Back to RollDice when player closes shop or clicks "Continue".
/// </summary>
public class OpenShopState : GameState
{
    private ShopType shopType = ShopType.Skills; // Default to Skills
    private bool isClosing = false;
    private bool shopTypeSetExternally = false; // Track if shopType was set before OnEnter

    public OpenShopState(GameStateManager manager) : base(manager) { }

    public override void OnEnter()
    {
        // Only set default if shopType wasn't already set externally
        if (!shopTypeSetExternally)
        {
            shopType = ShopType.Skills;
        }
        
        isClosing = false;
        
        // Subscribe to purchase events
        if (manager.ShopManager != null)
        {
            manager.ShopManager.OnItemPurchased -= OnItemPurchased; // Unsubscribe first to prevent duplicates
            manager.ShopManager.OnItemPurchased += OnItemPurchased;
            Debug.Log("[OpenShop] Subscribed to OnItemPurchased event");
        }
        else
        {
            Debug.LogError("[OpenShop] ShopManager is null! Cannot subscribe to purchase events.");
        }
        
        OpenShop(shopType);
        
        // Reset flag after opening
        shopTypeSetExternally = false;
    }

    /// <summary>
    /// Open a specific shop type.
    /// </summary>
    public void OpenShop(ShopType type)
    {
        shopType = type;
        shopTypeSetExternally = true; // Mark that shop type was set externally
        Debug.Log($"[OpenShop] {shopType} Shop is now open!");

        // Refresh shop with specific type
        if (manager.ShopManager != null)
        {
            manager.ShopManager.RefreshShop(shopType);
            
            // Show shop UI via ShopManager event
            manager.ShopManager.ShowShop();
            
            // Also directly call Show() on PanelTransitionHandle to ensure animation plays
            var panelTransition = Object.FindFirstObjectByType<PanelTransitionHandle>();
            if (panelTransition != null)
            {
                panelTransition.Show();
                Debug.Log("[OpenShop] Called Show() on PanelTransitionHandle");
            }
            else
            {
                Debug.LogWarning("[OpenShop] PanelTransitionHandle not found in scene!");
            }
        }
        else
        {
            Debug.LogError("[OpenShop] ShopManager reference is null!");
        }
    }

    public override void OnUpdate()
    {
        // Wait for player to click "Continue" or close shop
        // In real implementation, this is triggered by UI button
    }

    public override void OnExit()
    {
        Debug.Log($"[OpenShop] {shopType} Shop closed. Continuing to next wave.");

        // Unsubscribe from purchase events
        if (manager.ShopManager != null)
        {
            manager.ShopManager.OnItemPurchased -= OnItemPurchased;
            
            // Only hide if not already closing (to avoid double hide animation)
            if (!isClosing && manager.ShopManager != null)
            {
                manager.ShopManager.HideShop();
            }
        }
        
        // Clean up event subscriptions
        var panelTransition = Object.FindFirstObjectByType<PanelTransitionHandle>();
        if (panelTransition != null)
        {
            panelTransition.OnHideComplete -= OnPanelHideComplete;
        }
    }

    public override string GetStateName() => $"Open Shop ({shopType})";

    // ====== EVENT HANDLERS ======

    /// <summary>
    /// Called when player purchases an item from the shop.
    /// Triggers hide animation and transitions to RollDice after animation completes.
    /// </summary>
    private void OnItemPurchased(int slotIndex, IShopItem item)
    {
        Debug.Log($"[OpenShop] OnItemPurchased called! SlotIndex: {slotIndex}, Item: {item.GetName()}, isClosing: {isClosing}");
        
        if (isClosing)
        {
            Debug.Log("[OpenShop] Already closing, ignoring duplicate purchase event");
            return;
        }
        
        isClosing = true;
        Debug.Log($"[OpenShop] Item purchased: {item.GetName()}. Closing shop after animation...");
        
        // Find PanelTransitionHandle and call Hide directly
        var panelTransition = Object.FindFirstObjectByType<PanelTransitionHandle>();
        if (panelTransition != null)
        {
            Debug.Log("[OpenShop] Found PanelTransitionHandle, calling Hide() and subscribing to OnHideComplete");
            
            // Subscribe to hide complete event first
            panelTransition.OnHideComplete += OnPanelHideComplete;
            
            // Then trigger the hide animation
            panelTransition.Hide();
        }
        else
        {
            // Fallback: transition immediately if no panel found
            Debug.LogWarning("[OpenShop] PanelTransitionHandle not found! Transitioning immediately.");
            manager.TransitionToRollDice();
        }
    }

    /// <summary>
    /// Called when panel hide animation completes.
    /// </summary>
    private void OnPanelHideComplete()
    {
        Debug.Log("[OpenShop] Panel hide animation complete. Transitioning to RollDice...");
        
        // Unsubscribe from event
        var panelTransition = Object.FindFirstObjectByType<PanelTransitionHandle>();
        if (panelTransition != null)
        {
            panelTransition.OnHideComplete -= OnPanelHideComplete;
        }
        
        // Transition to RollDice state
        manager.TransitionToRollDice();
    }

    // ====== PUBLIC METHODS ======

    /// <summary>
    /// Called by UI when player clicks "Continue" or "Close Shop" button.
    /// </summary>
    public void CloseShop()
    {
        if (isClosing) return;
        
        isClosing = true;
        Debug.Log("[OpenShop] Close button clicked. Closing shop with animation...");
        
        // Find PanelTransitionHandle and call Hide directly
        var panelTransition = Object.FindFirstObjectByType<PanelTransitionHandle>();
        if (panelTransition != null)
        {
            Debug.Log("[OpenShop] Found PanelTransitionHandle, calling Hide() and subscribing to OnHideComplete");
            
            // Subscribe to hide complete event first
            panelTransition.OnHideComplete += OnPanelHideComplete;
            
            // Then trigger the hide animation
            panelTransition.Hide();
        }
        else
        {
            // Fallback: transition immediately
            Debug.LogWarning("[OpenShop] PanelTransitionHandle not found! Transitioning immediately.");
            manager.TransitionToRollDice();
        }
    }

    /// <summary>
    /// Switch to a different shop type without leaving the shop state.
    /// </summary>
    public void SwitchShopType(ShopType newType)
    {
        if (newType != shopType)
        {
            OpenShop(newType);
        }
    }
}
