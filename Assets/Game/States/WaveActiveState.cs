// WaveActiveState.cs
// State: Wave is in progress, enemies are spawning/attacking

using UnityEngine;
using TowerOfOdds.Manager;

/// <summary>
/// Wave Active State: Enemies are spawning and attacking the tower.
/// Monitors WaveManager for wave completion (duration-based).
/// Transition: To CheckWaveComplete when wave duration ends, or GameOver if tower dies.
/// </summary>
public class WaveActiveState : GameState
{
    private bool waveComplete;
    private WaveManager waveManager;

    public WaveActiveState(GameStateManager manager) : base(manager) { }

    public override void OnEnter()
    {
        waveComplete = false;
        Debug.Log($"[WaveActive] Wave {manager.CurrentWave} in progress...");

        // Find WaveManager if not cached
        if (waveManager == null)
        {
            waveManager = Object.FindFirstObjectByType<WaveManager>();
            if (waveManager == null)
            {
                Debug.LogError("[WaveActive] WaveManager not found in scene!");
                return;
            }
        }

        // Subscribe to wave completion event
        waveManager.OnWaveComplete -= OnWaveCompleted;
        waveManager.OnWaveComplete += OnWaveCompleted;
    }

    public override void OnUpdate()
    {
        // Tower death is handled by event in GameStateManager
        // Wave completion is handled by WaveManager.OnWaveComplete event
    }

    public override void OnExit()
    {
        Debug.Log($"[WaveActive] Wave {manager.CurrentWave} activity ended");
        
        // Unsubscribe from events
        if (waveManager != null)
        {
            waveManager.OnWaveComplete -= OnWaveCompleted;
        }
    }

    public override string GetStateName() => "Wave Active";

    // ====== EVENT HANDLERS ======

    /// <summary>
    /// Called by WaveManager when wave duration ends.
    /// </summary>
    private void OnWaveCompleted(int waveNumber)
    {
        if (!waveComplete)
        {
            waveComplete = true;
            Debug.Log($"[WaveActive] Wave {waveNumber} completed by WaveManager");
            manager.TransitionToCheckWaveComplete();
        }
    }
}
