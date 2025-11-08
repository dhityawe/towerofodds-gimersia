// GameState.cs
// Base class for all game states using the State Pattern

using UnityEngine;

/// <summary>
/// Base class for all game states.
/// Each state represents a phase in the game flow.
/// </summary>
public abstract class GameState
{
    protected GameStateManager manager;

    public GameState(GameStateManager manager)
    {
        this.manager = manager;
    }

    /// <summary>Called when entering this state.</summary>
    public virtual void OnEnter() { }

    /// <summary>Called every frame while in this state.</summary>
    public virtual void OnUpdate() { }

    /// <summary>Called when exiting this state.</summary>
    public virtual void OnExit() { }

    /// <summary>State name for debugging.</summary>
    public abstract string GetStateName();
}
