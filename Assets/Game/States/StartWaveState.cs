// StartWaveState.cs
// State: Initialize and start the wave

using UnityEngine;
using TowerOfOdds.Manager;

/// <summary>
/// Start Wave State: Increment wave counter, activate tower/enemies, and begin wave.
/// Transition: Automatically to WaveActive after initialization.
/// </summary>
public class StartWaveState : GameState
{
    public StartWaveState(GameStateManager manager) : base(manager) { }

    public override void OnEnter()
    {
        // Increment wave counter
        manager.IncrementWave();

        Debug.Log($"[StartWave] Starting Wave {manager.CurrentWave}");

        // Enable tower and enemies (they were inactive before this state)
        EnableGameplay();

        // Start wave via WaveManager
        WaveManager waveManager = Object.FindFirstObjectByType<WaveManager>();
        if (waveManager != null)
        {
            waveManager.StartNextWave();
        }
        else
        {
            Debug.LogWarning("[StartWave] WaveManager not found!");
        }

        // Transition to wave active
        manager.TransitionToWaveActive();
    }

    public override string GetStateName() => "Start Wave";

    private void EnableGameplay()
    {
        // Enable tower combat
        if (manager.PlayerTower != null)
        {
            manager.PlayerTower.enabled = true;
            Debug.Log("[StartWave] Tower combat enabled");
        }

        // Enable enemy spawning/movement
        // (Enemies will be spawned by WaveManager)
    }
}
