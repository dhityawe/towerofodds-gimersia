// IntroState.cs
// State: Game intro animation and setup

using UnityEngine;

/// <summary>
/// Intro State: Play intro animation, then transition to Check Wave (for shop at wave 0).
/// </summary>
public class IntroState : GameState
{
    private IntroScene introScene;
    private bool introCompleted = false;

    public IntroState(GameStateManager manager) : base(manager) { }

    public override void OnEnter()
    {
        Debug.Log("[IntroState] Starting game intro...");

        // Ensure tower combat is disabled at game start
        if (manager.PlayerTower != null)
        {
            manager.PlayerTower.enabled = false;
            Debug.Log("[IntroState] Tower combat disabled at game start");
        }

        // Find IntroScene component
        introScene = Object.FindFirstObjectByType<IntroScene>();

        if (introScene != null)
        {
            // Play intro animation
            introScene.PlayIntro();

            // Wait for intro to complete (we'll check in OnUpdate)
            introCompleted = false;
        }
        else
        {
            Debug.LogWarning("[IntroState] IntroScene not found! Skipping intro.");
            introCompleted = true;
        }
    }

    public override void OnUpdate()
    {
        // Check if intro is completed
        // Since IntroScene doesn't have a completion callback, we'll transition after a delay
        // Alternative: IntroScene can call GameStateManager directly when done
        
        if (introCompleted)
        {
            // Transition to Check Wave (which will open shop for wave 0)
            manager.TransitionToCheckWave();
        }
    }

    public override void OnExit()
    {
        Debug.Log("[IntroState] Intro completed, starting game...");
    }

    public override string GetStateName() => "Intro";

    /// <summary>
    /// Call this method from IntroScene when intro animation completes.
    /// </summary>
    public void OnIntroComplete()
    {
        introCompleted = true;
    }
}
