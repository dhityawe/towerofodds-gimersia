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

        // TODO: Show game over UI
        // gameOverUI.Show();

        // TODO: Display stats (waves survived, chips earned, etc.)
        // gameOverUI.ShowStats(manager.CurrentWave, PlayerDataManager.Instance.Chips);
    }

    public override void OnUpdate()
    {
        // Wait for player to click "Restart" button
    }

    public override void OnExit()
    {
        Debug.Log("[GameOver] Restarting game...");

        // TODO: Hide game over UI
        // gameOverUI.Hide();
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
