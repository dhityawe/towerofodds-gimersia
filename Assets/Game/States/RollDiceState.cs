// RollDiceState.cs
// State: Player rolls dice before starting the wave
// Manages panel visibility and dice rolling for all equipped skills

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

/// <summary>
/// Roll Dice State: Player rolls dice for each equipped skill slot.
/// Transition: To StartWave after all dice rolls complete and player clicks Next Wave.
/// </summary>
public class RollDiceState : GameState
{
    private DiceRollUI diceRollUI;
    private GameObject dicePanel;
    private Button nextWaveButton;
    private bool hasCompletedAllRolls;

    public RollDiceState(GameStateManager manager) : base(manager) { }

    public override void OnEnter()
    {
        hasCompletedAllRolls = false;
        Debug.Log("[RollDice] Entering dice roll state...");

        // Find DiceRollUI
        if (diceRollUI == null)
        {
            diceRollUI = Object.FindFirstObjectByType<DiceRollUI>();
        }

        // Find dice panel (adjust path to match your UI hierarchy)
        if (dicePanel == null)
        {
            dicePanel = GameObject.Find("DiceRollPanel"); // or use your panel name
        }

        // Find or get Next Wave button (could be child of dice panel or separate)
        if (nextWaveButton == null && dicePanel != null)
        {
            // Try to find NextWaveButton as child of panel
            Transform buttonTransform = dicePanel.transform.Find("NextWaveButton");
            if (buttonTransform != null)
            {
                nextWaveButton = buttonTransform.GetComponent<Button>();
            }
        }

        if (diceRollUI != null)
        {
            // Show panel
            if (dicePanel != null)
            {
                dicePanel.SetActive(true);
            }

            // Hide Next Wave button initially
            if (nextWaveButton != null)
            {
                nextWaveButton.gameObject.SetActive(false);
                nextWaveButton.onClick.RemoveAllListeners();
                nextWaveButton.onClick.AddListener(OnNextWaveClicked);
            }

            // Subscribe to events
            diceRollUI.OnSkillRollComplete += OnSkillRollComplete;
            diceRollUI.OnAllRollsComplete += OnAllRollsComplete;

            // Prepare dice UI
            diceRollUI.PrepareForRoll();
        }
        else
        {
            Debug.LogWarning("[RollDice] DiceRollUI not found! Auto-transitioning...");
            manager.TransitionToStartWave();
        }
    }

    public override void OnUpdate()
    {
        // UI handles all input and rolling logic
    }

    public override void OnExit()
    {
        Debug.Log("[RollDice] Exiting dice roll state");
        
        // Unsubscribe from events
        if (diceRollUI != null)
        {
            diceRollUI.OnSkillRollComplete -= OnSkillRollComplete;
            diceRollUI.OnAllRollsComplete -= OnAllRollsComplete;
        }

        // Hide panel
        if (dicePanel != null)
        {
            dicePanel.SetActive(false);
        }
    }

    public override string GetStateName() => "Roll Dice";

    // ====== EVENT HANDLERS ======

    /// <summary>
    /// Called when a single skill roll completes.
    /// </summary>
    private void OnSkillRollComplete(int skillIndex, int dice1, int dice2)
    {
        float multiplier = TowerBase.GetDiceMultiplier(dice1, dice2);
        Debug.Log($"[RollDice] Skill {skillIndex} rolled: {dice1} + {dice2} = {dice1 + dice2} (×{multiplier:F1})");
        
        // Store result for this skill
        DiceResult.Instance.SetSkillDice(skillIndex, dice1, dice2);
    }

    /// <summary>
    /// Called when all skill rolls are complete.
    /// </summary>
    private void OnAllRollsComplete()
    {
        if (hasCompletedAllRolls) return;
        
        hasCompletedAllRolls = true;
        Debug.Log("[RollDice] All rolls completed. Showing Next Wave button...");
        
        // Show Next Wave button instead of auto-transitioning
        if (nextWaveButton != null)
        {
            nextWaveButton.gameObject.SetActive(true);
            
            // Optional: Add button animation
            nextWaveButton.transform.localScale = Vector3.zero;
            nextWaveButton.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
            
            // Optional: Update button text
            var buttonText = nextWaveButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = "START WAVE";
            }
        }
        else
        {
            Debug.LogWarning("[RollDice] Next Wave button not found! Auto-transitioning...");
            manager.TransitionToStartWave();
        }
    }
    
    /// <summary>
    /// Called when player clicks Next Wave button.
    /// </summary>
    private void OnNextWaveClicked()
    {
        Debug.Log("[RollDice] Next Wave button clicked. Transitioning to StartWave...");
        manager.TransitionToStartWave();
    }
}

/// <summary>
/// Singleton to store dice roll results for each skill slot.
/// </summary>
public class DiceResult
{
    private static DiceResult _instance;
    public static DiceResult Instance => _instance ?? (_instance = new DiceResult());

    // Store dice results per skill slot
    private int[] dice1Results = new int[3];
    private int[] dice2Results = new int[3];

    /// <summary>
    /// Set dice results for a specific skill slot.
    /// </summary>
    public void SetSkillDice(int skillIndex, int d1, int d2)
    {
        if (skillIndex >= 0 && skillIndex < 3)
        {
            dice1Results[skillIndex] = Mathf.Clamp(d1, 1, 6);
            dice2Results[skillIndex] = Mathf.Clamp(d2, 1, 6);
        }
    }

    /// <summary>
    /// Get dice results for a specific skill slot.
    /// </summary>
    public (int dice1, int dice2) GetSkillDice(int skillIndex)
    {
        if (skillIndex >= 0 && skillIndex < 3)
        {
            return (dice1Results[skillIndex], dice2Results[skillIndex]);
        }
        return (0, 0);
    }

    /// <summary>
    /// Get multiplier for a specific skill slot.
    /// </summary>
    public float GetSkillMultiplier(int skillIndex)
    {
        var (d1, d2) = GetSkillDice(skillIndex);
        return TowerBase.GetDiceMultiplier(d1, d2);
    }

    /// <summary>
    /// Clear all dice results.
    /// </summary>
    public void Clear()
    {
        System.Array.Clear(dice1Results, 0, dice1Results.Length);
        System.Array.Clear(dice2Results, 0, dice2Results.Length);
    }
}
