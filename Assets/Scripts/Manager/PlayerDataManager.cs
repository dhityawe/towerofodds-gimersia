// PlayerDataManager.cs
// Manages player runtime data including currency (Chips) and game state

using System;
using UnityEngine;

/// <summary>
/// Singleton manager for player runtime data.
/// Manages Chip currency used in the shop.
/// Resets on game over/loss.
/// </summary>
public class PlayerDataManager : MonoBehaviour
{
    private static PlayerDataManager _instance;
    public static PlayerDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<PlayerDataManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("PlayerDataManager");
                    _instance = go.AddComponent<PlayerDataManager>();
                }
            }
            return _instance;
        }
    }

    [Header("Currency")]
    [SerializeField] private int currentChips = 100; // Starting chips
    [SerializeField] private int startingChips = 100; // Default starting amount

    [Header("Runtime State")]
    [SerializeField] private bool isGameOver = false;

    // Events for UI updates
    public event Action<int> OnChipsChanged; // New chip amount
    public event Action<int> OnChipsGained; // Amount gained
    public event Action<int> OnChipsSpent; // Amount spent
    public event Action OnGameReset;

    // Public accessors
    public int Chips => currentChips;
    public bool IsGameOver => isGameOver;

    void Awake()
    {
        // Singleton pattern
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject); // Persist across scenes (optional)
    }

    void Start()
    {
        // Initialize with starting chips
        ResetChips();
    }

    // ====== CURRENCY MANAGEMENT ======

    /// <summary>
    /// Add chips to player's balance.
    /// </summary>
    public void AddChips(int amount)
    {
        if (amount <= 0) return;

        currentChips += amount;
        OnChipsGained?.Invoke(amount);
        OnChipsChanged?.Invoke(currentChips);

        Debug.Log($"[PlayerData] Gained {amount} chips. Total: {currentChips}");
    }

    /// <summary>
    /// Try to spend chips. Returns true if successful, false if not enough.
    /// </summary>
    public bool SpendChips(int amount)
    {
        if (amount <= 0)
        {
            Debug.LogWarning("[PlayerData] Cannot spend negative or zero chips");
            return false;
        }

        if (currentChips < amount)
        {
            Debug.LogWarning($"[PlayerData] Not enough chips! Need {amount}, have {currentChips}");
            return false;
        }

        currentChips -= amount;
        OnChipsSpent?.Invoke(amount);
        OnChipsChanged?.Invoke(currentChips);

        Debug.Log($"[PlayerData] Spent {amount} chips. Remaining: {currentChips}");
        return true;
    }

    /// <summary>
    /// Check if player has enough chips.
    /// </summary>
    public bool HasEnoughChips(int amount)
    {
        return currentChips >= amount;
    }

    /// <summary>
    /// Set chips to a specific amount (for debugging/cheats).
    /// </summary>
    public void SetChips(int amount)
    {
        currentChips = Mathf.Max(0, amount);
        OnChipsChanged?.Invoke(currentChips);
        Debug.Log($"[PlayerData] Chips set to {currentChips}");
    }

    // ====== GAME STATE MANAGEMENT ======

    /// <summary>
    /// Reset chips to starting amount (called on new game or after loss).
    /// </summary>
    public void ResetChips()
    {
        currentChips = startingChips;
        isGameOver = false;
        OnChipsChanged?.Invoke(currentChips);
        OnGameReset?.Invoke();

        Debug.Log($"[PlayerData] Chips reset to {startingChips}");
    }

    /// <summary>
    /// Called when player loses/dies. Resets all runtime data.
    /// </summary>
    public void OnPlayerLose()
    {
        isGameOver = true;
        Debug.Log("[PlayerData] Player lost. Resetting chips on next game...");
        
        // Optional: Reset immediately or wait for "Restart" button
        // ResetChips();
    }

    /// <summary>
    /// Start a new game/run. Resets all data.
    /// </summary>
    public void StartNewGame()
    {
        ResetChips();
        Debug.Log("[PlayerData] New game started");
    }

    // ====== REWARD HELPERS ======

    /// <summary>
    /// Reward chips for completing a wave.
    /// </summary>
    public void RewardWaveComplete(int baseReward, int waveNumber)
    {
        // Example: increase reward based on wave number
        int reward = baseReward + (waveNumber * 5);
        AddChips(reward);
    }

    /// <summary>
    /// Reward chips for killing an enemy.
    /// </summary>
    public void RewardEnemyKilled(int chipReward)
    {
        AddChips(chipReward);
    }

    // ====== DEBUG/EDITOR ======

#if UNITY_EDITOR
    [ContextMenu("Add 100 Chips (Debug)")]
    private void DebugAdd100Chips()
    {
        AddChips(100);
    }

    [ContextMenu("Spend 50 Chips (Debug)")]
    private void DebugSpend50Chips()
    {
        SpendChips(50);
    }

    [ContextMenu("Reset Chips")]
    private void DebugResetChips()
    {
        ResetChips();
    }

    [ContextMenu("Simulate Player Loss")]
    private void DebugPlayerLose()
    {
        OnPlayerLose();
    }
#endif
}
