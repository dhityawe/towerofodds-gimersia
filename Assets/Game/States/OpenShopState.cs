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
    private ShopType shopType;

    public OpenShopState(GameStateManager manager) : base(manager) { }

    public override void OnEnter()
    {
        // Default to Items shop if not specified
        shopType = ShopType.Items;
        OpenShop(shopType);
    }

    /// <summary>
    /// Open a specific shop type.
    /// </summary>
    public void OpenShop(ShopType type)
    {
        shopType = type;
        Debug.Log($"[OpenShop] {shopType} Shop is now open!");

        // Refresh shop with specific type
        if (manager.ShopManager != null)
        {
            manager.ShopManager.RefreshShop(shopType);
        }

        // TODO: Show shop UI with shop type selector
        // shopUI.OpenShop(shopType);

        // TODO: Pause game (optional)
        // Time.timeScale = 0f;
    }

    public override void OnUpdate()
    {
        // Wait for player to click "Continue" or close shop
        // In real implementation, this is triggered by UI button
    }

    public override void OnExit()
    {
        Debug.Log($"[OpenShop] {shopType} Shop closed. Continuing to next wave.");

        // TODO: Hide shop UI
        // shopUI.CloseShop();

        // TODO: Unpause game
        // Time.timeScale = 1f;
    }

    public override string GetStateName() => $"Open Shop ({shopType})";

    // ====== PUBLIC METHODS ======

    /// <summary>
    /// Called by UI when player clicks "Continue" or "Close Shop" button.
    /// </summary>
    public void CloseShop()
    {
        manager.TransitionToRollDice();
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
