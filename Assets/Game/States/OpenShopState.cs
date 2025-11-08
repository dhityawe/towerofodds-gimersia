// OpenShopState.cs
// State: Shop is open for player to purchase items

using UnityEngine;

/// <summary>
/// Open Shop State: Display shop UI and allow purchases.
/// Transition: Back to RollDice when player closes shop or clicks "Continue".
/// </summary>
public class OpenShopState : GameState
{
    public OpenShopState(GameStateManager manager) : base(manager) { }

    public override void OnEnter()
    {
        Debug.Log("[OpenShop] Shop is now open!");

        // Refresh shop with new random items
        if (manager.ShopManager != null)
        {
            manager.ShopManager.RefreshShop();
        }

        // TODO: Show shop UI
        // shopUI.OpenShop();

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
        Debug.Log("[OpenShop] Shop closed. Continuing to next wave.");

        // TODO: Hide shop UI
        // shopUI.CloseShop();

        // TODO: Unpause game
        // Time.timeScale = 1f;
    }

    public override string GetStateName() => "Open Shop";

    // ====== PUBLIC METHODS ======

    /// <summary>
    /// Called by UI when player clicks "Continue" or "Close Shop" button.
    /// </summary>
    public void CloseShop()
    {
        manager.TransitionToRollDice();
    }
}
