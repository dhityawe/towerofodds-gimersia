// GameOverState.cs
// State: Tower HP = 0, game over

using UnityEngine;

/// <summary>
/// Game Over State: Tower has been destroyed (HP = 0).
/// Shows game over UI and waits for player to restart.
/// </summary>
public class GameOverState : GameState
{
    public GameOverState(GameStateManager manager) : base(manager) { }

    public override void OnEnter()
    {
        Debug.Log("[GameOver] Tower destroyed. Game Over!");

        // Trigger game over event
        manager.NotifyGameOver();

        // Mark player as lost
        PlayerDataManager.Instance.OnPlayerLose();

        // Show game over panel with animation
        if (manager.GameOverPanel != null)
        {
            manager.GameOverPanel.SetActive(true);
            Debug.Log("[GameOver] GameOverPanel activated");
            
            // Find and trigger PanelTransitionHandle animation
            var panelTransition = manager.GameOverPanel.GetComponentInChildren<PanelTransitionHandle>();
            if (panelTransition == null)
            {
                // Try finding it as a sibling or parent
                panelTransition = Object.FindFirstObjectByType<PanelTransitionHandle>();
            }
            
            if (panelTransition != null)
            {
                panelTransition.Show();
                Debug.Log("[GameOver] Panel show animation triggered");
            }
            else
            {
                Debug.LogWarning("[GameOver] PanelTransitionHandle not found for GameOver panel!");
            }
        }
        else
        {
            Debug.LogWarning("[GameOver] GameOverPanel reference is null in GameStateManager!");
        }
    }

    public override void OnUpdate()
    {
        // Wait for player to click "Restart" button
    }

    public override void OnExit()
    {
        Debug.Log("[GameOver] Restarting game...");

        // Hide game over panel with animation
        if (manager.GameOverPanel != null)
        {
            var panelTransition = manager.GameOverPanel.GetComponentInChildren<PanelTransitionHandle>();
            if (panelTransition == null)
            {
                panelTransition = Object.FindFirstObjectByType<PanelTransitionHandle>();
            }
            
            if (panelTransition != null)
            {
                panelTransition.Hide();
                Debug.Log("[GameOver] Panel hide animation triggered");
            }
            
            // Note: Panel will be deactivated after hide animation completes (if disableOnHide is true)
        }
    }

    public override string GetStateName() => "Game Over";

    // ====== PUBLIC METHODS ======

    /// <summary>
    /// Called by UI when player clicks "Restart" button.
    /// </summary>
    public void RestartGame()
    {
        manager.ResetGame();
    }

    /// <summary>
    /// Called by UI when player clicks "Quit to Menu" button.
    /// </summary>
    public void QuitToMenu()
    {
        // TODO: Load main menu scene
        // SceneManager.LoadScene("MainMenu");
        Debug.Log("[GameOver] Quit to menu (not implemented)");
    }
}
