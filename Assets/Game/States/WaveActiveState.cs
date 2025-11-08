// WaveActiveState.cs
// State: Wave is in progress, enemies are spawning/attacking

using UnityEngine;

/// <summary>
/// Wave Active State: Enemies are spawning and attacking the tower.
/// Checks for wave completion or tower death.
/// Transition: To CheckWaveComplete when all enemies defeated, or GameOver if tower dies.
/// </summary>
public class WaveActiveState : GameState
{
    private bool waveComplete;

    public WaveActiveState(GameStateManager manager) : base(manager) { }

    public override void OnEnter()
    {
        waveComplete = false;
        Debug.Log($"[WaveActive] Wave {manager.CurrentWave} in progress...");

        // TODO: Monitor enemy count from wave spawner
    }

    public override void OnUpdate()
    {
        // Check if tower is dead (handled by event in GameStateManager)
        // Check if wave is complete
        if (IsWaveComplete() && !waveComplete)
        {
            waveComplete = true;
            manager.TransitionToCheckWaveComplete();
        }
    }

    public override void OnExit()
    {
        Debug.Log($"[WaveActive] Wave {manager.CurrentWave} activity ended");
    }

    public override string GetStateName() => "Wave Active";

    // ====== HELPERS ======

    private bool IsWaveComplete()
    {
        // TODO: Check with wave spawner if all enemies are defeated
        // return waveSpawner.AllEnemiesDefeated();

        // TEMP: Auto-complete after 5 seconds for testing
        return Time.time > 5f; // Replace with real condition
    }

    /// <summary>
    /// Called externally when wave spawner confirms all enemies defeated.
    /// </summary>
    public void NotifyWaveComplete()
    {
        if (!waveComplete)
        {
            waveComplete = true;
            manager.TransitionToCheckWaveComplete();
        }
    }
}
