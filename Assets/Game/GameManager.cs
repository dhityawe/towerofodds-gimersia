// GameManager.cs
// Simplified game manager that integrates with GameStateManager

using UnityEngine;

/// <summary>
/// Simplified game manager. Most logic now handled by GameStateManager.
/// This script handles integration between systems.
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TowerRuntime playerTower;
    [SerializeField] private ShopManager shopManager;
    [SerializeField] private GameStateManager stateManager;

    [Header("Settings")]
    [SerializeField] private int enemyKillReward = 5; // Chips per enemy kill

    void Start()
    {
        // Find references if not assigned
        if (stateManager == null)
        {
            stateManager = GameStateManager.Instance;
        }
        if (playerTower == null)
        {
            playerTower = stateManager.PlayerTower;
        }
        if (shopManager == null)
        {
            shopManager = stateManager.ShopManager;
        }

        // Subscribe to state manager events
        stateManager.OnWaveStarted += OnWaveStarted;
        stateManager.OnWaveCompleted += OnWaveCompleted;
        stateManager.OnGameOver += OnGameOver;

        // Subscribe to player data events for UI
        PlayerDataManager.Instance.OnChipsChanged += OnChipsChanged;
        PlayerDataManager.Instance.OnChipsGained += OnChipsGained;
        PlayerDataManager.Instance.OnChipsSpent += OnChipsSpent;
    }

    void OnDestroy()
    {
        // Unsubscribe
        if (stateManager != null)
        {
            stateManager.OnWaveStarted -= OnWaveStarted;
            stateManager.OnWaveCompleted -= OnWaveCompleted;
            stateManager.OnGameOver -= OnGameOver;
        }

        if (PlayerDataManager.Instance != null)
        {
            PlayerDataManager.Instance.OnChipsChanged -= OnChipsChanged;
            PlayerDataManager.Instance.OnChipsGained -= OnChipsGained;
            PlayerDataManager.Instance.OnChipsSpent -= OnChipsSpent;
        }
    }

    // ====== EVENT HANDLERS ======

    private void OnWaveStarted(int waveNumber)
    {
        Debug.Log($"[GameManager] Wave {waveNumber} started");
        // TODO: Update wave UI
        // waveText.text = $"Wave {waveNumber}";
    }

    private void OnWaveCompleted(int waveNumber)
    {
        Debug.Log($"[GameManager] Wave {waveNumber} completed");
        // TODO: Show wave complete banner
    }

    private void OnGameOver()
    {
        Debug.Log("[GameManager] Game Over!");
        // TODO: Show game over UI
    }

    private void OnChipsChanged(int newAmount)
    {
        // TODO: Update chip UI
        // chipText.text = newAmount.ToString();
    }

    private void OnChipsGained(int amount)
    {
        // TODO: Show floating +X text
        Debug.Log($"[GameManager] +{amount} chips!");
    }

    private void OnChipsSpent(int amount)
    {
        // TODO: Play purchase sound
        Debug.Log($"[GameManager] -{amount} chips");
    }

    // ====== PUBLIC API (called by other systems) ======

    /// <summary>
    /// Called when an enemy is killed.
    /// </summary>
    public void OnEnemyKilled()
    {
        PlayerDataManager.Instance.RewardEnemyKilled(enemyKillReward);
    }

    /// <summary>
    /// Called by wave spawner when all enemies are defeated.
    /// </summary>
    public void OnAllEnemiesDefeated()
    {
        // Notify state manager to transition from WaveActive to CheckWaveComplete
        // This would be called by your wave spawner
        Debug.Log("[GameManager] All enemies defeated");
    }

    // ====== DEBUG ======

#if UNITY_EDITOR
    [ContextMenu("Test: Kill 5 Enemies")]
    private void TestKillEnemies()
    {
        for (int i = 0; i < 5; i++)
        {
            OnEnemyKilled();
        }
    }
#endif
}
