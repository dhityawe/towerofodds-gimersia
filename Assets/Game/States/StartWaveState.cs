// StartWaveState.cs
// State: Initialize and start the wave

using UnityEngine;

/// <summary>
/// Start Wave State: Increment wave counter and begin spawning enemies.
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

        // TODO: Initialize wave spawner with current wave number and dice results
        // waveSpawner.StartWave(manager.CurrentWave, DiceResult.Instance.Dice1, DiceResult.Instance.Dice2);

        // Transition to wave active
        manager.TransitionToWaveActive();
    }

    public override string GetStateName() => "Start Wave";
}
