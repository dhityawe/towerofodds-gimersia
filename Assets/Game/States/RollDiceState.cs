// RollDiceState.cs
// State: Player rolls dice before starting the wave

using UnityEngine;

/// <summary>
/// Roll Dice State: Player rolls two dice (1-6) to determine wave modifiers.
/// Transition: Automatically to StartWave after dice are rolled.
/// </summary>
public class RollDiceState : GameState
{
    private int dice1;
    private int dice2;
    private bool hasRolled;

    public RollDiceState(GameStateManager manager) : base(manager) { }

    public override void OnEnter()
    {
        hasRolled = false;
        Debug.Log("[RollDice] Waiting for player to roll dice...");
        
        // TODO: Show dice UI, wait for player input
        // For now, auto-roll after a delay for testing
        AutoRollDice();
    }

    public override void OnUpdate()
    {
        // Wait for dice roll
        // In real implementation, wait for UI button click
    }

    public override void OnExit()
    {
        Debug.Log($"[RollDice] Rolled: {dice1}, {dice2} (Multiplier: {TowerBase.GetDiceMultiplier(dice1, dice2):F2})");
    }

    public override string GetStateName() => "Roll Dice";

    // ====== PUBLIC METHODS ======

    /// <summary>
    /// Called by UI when player clicks "Roll Dice" button.
    /// </summary>
    public void RollDice()
    {
        if (hasRolled) return;

        dice1 = Random.Range(1, 7);
        dice2 = Random.Range(1, 7);
        hasRolled = true;

        Debug.Log($"[RollDice] Player rolled {dice1} and {dice2}");

        // Store dice results for wave usage
        DiceResult.Instance.SetDice(dice1, dice2);

        // Transition to start wave
        manager.TransitionToStartWave();
    }

    // ====== TEMP: AUTO ROLL FOR TESTING ======

    private void AutoRollDice()
    {
        // Remove this in production - wait for player input instead
        RollDice();
    }
}

/// <summary>
/// Singleton to store current dice roll results for the wave.
/// </summary>
public class DiceResult
{
    private static DiceResult _instance;
    public static DiceResult Instance => _instance ?? (_instance = new DiceResult());

    public int Dice1 { get; private set; }
    public int Dice2 { get; private set; }

    public void SetDice(int d1, int d2)
    {
        Dice1 = Mathf.Clamp(d1, 1, 6);
        Dice2 = Mathf.Clamp(d2, 1, 6);
    }

    public float GetMultiplier()
    {
        return TowerBase.GetDiceMultiplier(Dice1, Dice2);
    }
}
