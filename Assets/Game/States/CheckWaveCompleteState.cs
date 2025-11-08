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

        // Notify wave complete event
        manager.NotifyWaveComplete();

        // Reward chips for wave completion
        PlayerDataManager.Instance.RewardWaveComplete(baseReward: 50, manager.CurrentWave);

        // Check tower HP
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
        if (manager.PlayerTower.IsDead())
        {
            Debug.Log("[CheckWaveComplete] Tower HP = 0. Game Over!");
            manager.TransitionToGameOver();
            return;
        }

        // Tower is alive, check if should open shop
        if (manager.ShouldOpenShop())
        {
            Debug.Log($"[CheckWaveComplete] Wave {manager.CurrentWave} (kelipatan {manager.MaxWaveKelipatan}). Opening shop!");
            manager.TransitionToOpenShop();
        }
        else
        {
            Debug.Log($"[CheckWaveComplete] Wave {manager.CurrentWave} complete. Continue to next wave.");
            manager.TransitionToRollDice();
        }
    }
}
