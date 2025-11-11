// DiceRoller.cs
// Core dice rolling logic - manages the roll state and results
// Attach to a persistent GameObject (e.g., GameManager or DiceRollManager)

using System;
using UnityEngine;

/// <summary>
/// Manages dice rolling logic and state.
/// Handles the randomization, duration, and final results of 2d6 rolls.
/// </summary>
public class DiceRoller : MonoBehaviour
{
    [Header("Roll Settings")]
    [Tooltip("How long the dice roll animation lasts (seconds)")]
    [SerializeField] private float rollDuration = 1.5f;

    [Header("Current State (Read-Only)")]
    [SerializeField] private bool isRolling = false;
    [SerializeField] private int currentDice1 = 1;
    [SerializeField] private int currentDice2 = 1;
    [SerializeField] private float rollTimer = 0f;

    // Events
    public event Action OnRollStarted;                          // Fired when roll begins
    public event Action<int, int> OnRollUpdate;                 // Fired each frame during roll (random values)
    public event Action<int, int> OnRollComplete;               // Fired when roll finishes (final values)

    // Final roll results
    private int finalDice1;
    private int finalDice2;

    void Update()
    {
        if (!isRolling) return;

        rollTimer += Time.deltaTime;

        // During roll: randomize dice faces each frame for visual effect
        if (rollTimer < rollDuration)
        {
            currentDice1 = UnityEngine.Random.Range(1, 7);
            currentDice2 = UnityEngine.Random.Range(1, 7);
            OnRollUpdate?.Invoke(currentDice1, currentDice2);
        }
        else
        {
            // Roll complete: set final values
            isRolling = false;
            currentDice1 = finalDice1;
            currentDice2 = finalDice2;
            OnRollComplete?.Invoke(finalDice1, finalDice2);
            Debug.Log($"[DiceRoller] Roll complete: {finalDice1} + {finalDice2} = {finalDice1 + finalDice2}");
        }
    }

    /// <summary>
    /// Start a new dice roll with specified duration.
    /// </summary>
    public void RollDice(float duration = -1f)
    {
        if (isRolling)
        {
            Debug.LogWarning("[DiceRoller] Already rolling! Ignoring new roll request.");
            return;
        }

        // Use custom duration or default
        if (duration > 0f) rollDuration = duration;

        // Determine final results immediately (but reveal after duration)
        finalDice1 = UnityEngine.Random.Range(1, 7);
        finalDice2 = UnityEngine.Random.Range(1, 7);

        // Start rolling
        isRolling = true;
        rollTimer = 0f;
        OnRollStarted?.Invoke();
        
        Debug.Log($"[DiceRoller] Roll started! Final: {finalDice1} + {finalDice2} (revealing in {rollDuration}s)");
    }

    /// <summary>
    /// Instant roll without animation (for testing or fast-forward).
    /// </summary>
    public void RollDiceInstant()
    {
        finalDice1 = UnityEngine.Random.Range(1, 7);
        finalDice2 = UnityEngine.Random.Range(1, 7);
        currentDice1 = finalDice1;
        currentDice2 = finalDice2;
        isRolling = false;
        
        OnRollStarted?.Invoke();
        OnRollComplete?.Invoke(finalDice1, finalDice2);
        
        Debug.Log($"[DiceRoller] Instant roll: {finalDice1} + {finalDice2}");
    }

    /// <summary>
    /// Force roll to finish immediately (reveal final result early).
    /// </summary>
    public void SkipRoll()
    {
        if (!isRolling) return;

        isRolling = false;
        currentDice1 = finalDice1;
        currentDice2 = finalDice2;
        OnRollComplete?.Invoke(finalDice1, finalDice2);
        
        Debug.Log($"[DiceRoller] Roll skipped! Final: {finalDice1} + {finalDice2}");
    }

    // ====== Public Getters ======
    
    public bool IsRolling() => isRolling;
    public int GetDice1() => currentDice1;
    public int GetDice2() => currentDice2;
    public int GetTotal() => currentDice1 + currentDice2;
    public float GetRollProgress() => isRolling ? Mathf.Clamp01(rollTimer / rollDuration) : 1f;

    /// <summary>
    /// Get the dice multiplier using TowerBase logic.
    /// </summary>
    public float GetDiceMultiplier()
    {
        return TowerBase.GetDiceMultiplier(currentDice1, currentDice2);
    }
}
