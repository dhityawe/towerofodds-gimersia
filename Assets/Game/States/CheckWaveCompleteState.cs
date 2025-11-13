// CheckWaveCompleteState.cs
// State: Wave completed, check tower HP and decide next action

using UnityEngine;

/// <summary>
/// Check Wave Complete State: Wave is complete, check if tower HP > 0.
/// If HP = 0: Transition to GameOver.
/// If HP > 0 AND wave kelipatan 5: Transition to OpenShop.
/// If HP > 0 AND NOT kelipatan 5: Transition back to RollDice.
/// </summary>
public class CheckWaveCompleteState : GameState
{
    public CheckWaveCompleteState(GameStateManager manager) : base(manager) { }

    public override void OnEnter()
    {
        Debug.Log($"[CheckWaveComplete] Checking wave {manager.CurrentWave} completion...");

        // Disable tower combat after wave ends
        if (manager.PlayerTower != null)
        {
            manager.PlayerTower.enabled = false;
            Debug.Log("[CheckWaveComplete] Tower combat disabled");
        }

        // Only reward chips if wave > 0 (not after intro)
        if (manager.CurrentWave > 0)
        {
            // Notify wave complete event
            manager.NotifyWaveComplete();

            // Reward chips for wave completion
            PlayerDataManager.Instance.RewardWaveComplete(baseReward: 50, manager.CurrentWave);
        }

        // Check tower HP and decide next action
        CheckTowerStatus();
    }

    public override string GetStateName() => "Check Wave Complete";

    // ====== LOGIC ======

    private void CheckTowerStatus()
    {
        if (manager.PlayerTower == null)
        {
            Debug.LogError("[CheckWaveComplete] PlayerTower reference is null!");
            manager.TransitionToGameOver();
            return;
        }

        // Check if tower is dead
        if (manager.PlayerTower != null && manager.PlayerTower.IsDead())
        {
            Debug.Log("[CheckWaveComplete] Tower HP = 0. Game Over!");
            manager.TransitionToGameOver();
            return;
        }

        // Tower is alive, check if should open shop
        // Shop opens at:
        // - Wave 0 (after intro) - Skills shop
        // - Every 5 waves (5, 10, 15, etc.) - Skills shop
        // - Every 3 waves (3, 6, 9, 12, etc.) - Items shop
        
        int currentWave = manager.CurrentWave;
        
        // Check multiples of 5 first (Skills shop has priority)
        if (manager.ShouldOpenShop())
        {
            Debug.Log($"[CheckWaveComplete] Wave {currentWave} - Opening Skills shop (multiple of 5)!");
            manager.TransitionToOpenShop(ShopType.Skills);
        }
        // Check multiples of 3 (Items shop)
        else if (currentWave > 0 && currentWave % 3 == 0)
        {
            Debug.Log($"[CheckWaveComplete] Wave {currentWave} - Opening Items shop (multiple of 3)!");
            manager.TransitionToOpenShop(ShopType.Items);
        }
        else
        {
            Debug.Log($"[CheckWaveComplete] Wave {currentWave} complete. Continue to next wave.");
            manager.TransitionToRollDice();
        }
    }
}
