// DiceRollUI.cs
// Manages dice visuals and roll button
// Panel activation/deactivation handled by GameState
// Roll actions based on tower skill slots

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System;

/// <summary>
/// Dice UI controller - handles visual display and roll button.
/// Does NOT manage panel activation (that's GameState's job).
/// Rolls dice for each skill slot in sequence.
/// </summary>
public class DiceRollUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DiceRoller diceRoller;
    [SerializeField] private DiceVisualController dice1Visual;
    [SerializeField] private DiceVisualController dice2Visual;
    [SerializeField] private TowerRuntime towerRuntime;

    [Header("UI Components")]
    [SerializeField] private Button rollButton;
    [SerializeField] private TextMeshProUGUI rollButtonText;
    
    [Header("Skill Slot UI (for popup positions)")]
    [Tooltip("UI transforms for skill slots 0-2. Popup spawns at active slot.")]
    [SerializeField] private Transform[] skillSlotTransforms = new Transform[TowerRuntime.MAX_SKILL_SLOTS];

    [Header("Multiplier Popup (Balatro-style)")]
    [SerializeField] private GameObject multiplierPopupPrefab;
    [SerializeField] private Transform showResultDiceTransform; // First popup position (at dice result area)
    [SerializeField] private Transform fallbackPopupSpawnPoint;
    [SerializeField] private float popupDelay = 0.3f;
    [SerializeField] private float popupDurationAtDiceResult = 1f; // How long popup stays at dice result before moving to skill slot

    [Header("Settings")]
    [SerializeField] private float delayBetweenRolls = 0.5f; // Delay between skill rolls
    [SerializeField] private float delayAfterPopup = 0.8f; // Delay after popup shown before enabling next roll button
    [SerializeField] private bool autoRollNextSkill = false; // If true, auto-rolls next skill. If false, requires button click.

    [Header("Button Pulse")]
    [SerializeField] private bool enableButtonPulse = true;
    [SerializeField] private float buttonPulseScale = 1.1f;
    [SerializeField] private float buttonPulseDuration = 0.8f;

    // Events
    public event Action<int, int, int> OnSkillRollComplete; // skillIndex, dice1, dice2
    public event Action OnAllRollsComplete;

    private bool isRolling = false;
    private Tween buttonPulseTween;
    private int currentSkillIndex = 0;
    private int totalSkillsToRoll = 0;

    void Awake()
    {
        // Auto-find components if not assigned
        if (diceRoller == null)
        {
            diceRoller = FindFirstObjectByType<DiceRoller>();
        }

        if (towerRuntime == null)
        {
            towerRuntime = FindFirstObjectByType<TowerRuntime>();
        }

        // Wire button
        if (rollButton != null)
        {
            rollButton.onClick.AddListener(OnRollButtonClicked);
        }
    }

    void OnDestroy()
    {
        // Kill all tweens
        DOTween.Kill(rollButton);
        buttonPulseTween?.Kill();
    }

    void OnEnable()
    {
        if (diceRoller != null)
        {
            diceRoller.OnRollStarted += OnRollStarted;
            diceRoller.OnRollComplete += OnSingleRollComplete;
        }
    }

    void OnDisable()
    {
        if (diceRoller != null)
        {
            diceRoller.OnRollStarted -= OnRollStarted;
            diceRoller.OnRollComplete -= OnSingleRollComplete;
        }
    }

    /// <summary>
    /// Initialize for rolling. Called by GameState when panel is shown.
    /// </summary>
    public void PrepareForRoll()
    {
        // Count active skills
        if (towerRuntime != null)
        {
            var skills = towerRuntime.GetAllSkills();
            totalSkillsToRoll = 0;
            for (int i = 0; i < skills.Length; i++)
            {
                if (skills[i] != null)
                {
                    totalSkillsToRoll++;
                }
            }
        }

        currentSkillIndex = 0;
        isRolling = false;

        // Enable roll button with pulse animation
        if (rollButton != null)
        {
            rollButton.interactable = true;
            rollButton.transform.localScale = Vector3.zero;
            rollButton.transform.DOScale(Vector3.one, 0.3f)
                .SetEase(Ease.OutBack)
                .OnComplete(() => {
                    // Start button pulse loop
                    if (enableButtonPulse)
                    {
                        buttonPulseTween = rollButton.transform.DOScale(Vector3.one * buttonPulseScale, buttonPulseDuration)
                            .SetEase(Ease.InOutSine)
                            .SetLoops(-1, LoopType.Yoyo);
                    }
                });
        }

        if (rollButtonText != null)
        {
            rollButtonText.text = totalSkillsToRoll > 0 ? $"ROLL ({totalSkillsToRoll}x)" : "ROLL DICE";
        }

        Debug.Log($"[DiceRollUI] Prepared to roll for {totalSkillsToRoll} skills");
    }

    private void OnRollButtonClicked()
    {
        if (isRolling) return;

        // Stop button pulse
        buttonPulseTween?.Kill();
        if (rollButton != null)
        {
            rollButton.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutQuad);
            rollButton.interactable = false;
        }

        // Check if this is the first roll or a continuation
        if (currentSkillIndex == 0)
        {
            // Start rolling sequence for all skills
            isRolling = true;
            
            if (totalSkillsToRoll > 0)
            {
                StartNextSkillRoll();
            }
            else
            {
                // No skills, just roll once
                RollForSkillIndex(-1);
            }
        }
        else
        {
            // Continue with next skill roll
            StartNextSkillRoll();
        }
    }

    private void StartNextSkillRoll()
    {
        // Find next skill slot with a skill
        while (currentSkillIndex < TowerRuntime.MAX_SKILL_SLOTS)
        {
            if (towerRuntime != null)
            {
                var skills = towerRuntime.GetAllSkills();
                if (currentSkillIndex < skills.Length && skills[currentSkillIndex] != null)
                {
                    RollForSkillIndex(currentSkillIndex);
                    return;
                }
            }
            currentSkillIndex++;
        }

        // No more skills, complete all rolls
        CompleteAllRolls();
    }

    private void RollForSkillIndex(int skillIndex)
    {
        if (rollButtonText != null)
        {
            string skillName = skillIndex >= 0 && towerRuntime != null 
                ? towerRuntime.GetSkill(skillIndex).GetName()
                : "Dice";
            rollButtonText.text = $"Rolling {skillName}...";
        }

        // Start dice roll
        if (diceRoller != null)
        {
            diceRoller.RollDice();
        }
    }

    private void OnRollStarted()
    {
        Debug.Log("[DiceRollUI] Roll animation started");
    }

    private void OnSingleRollComplete(int dice1, int dice2)
    {
        // Show multiplier popup for this skill
        ShowMultiplierPopup(currentSkillIndex, dice1, dice2);

        // Fire event for this skill roll
        OnSkillRollComplete?.Invoke(currentSkillIndex, dice1, dice2);

        // Move to next skill index
        currentSkillIndex++;
        
        // Check if there are more skills to roll by looking ahead
        bool hasMoreSkills = false;
        if (towerRuntime != null && currentSkillIndex < TowerRuntime.MAX_SKILL_SLOTS)
        {
            var skills = towerRuntime.GetAllSkills();
            for (int i = currentSkillIndex; i < skills.Length; i++)
            {
                if (skills[i] != null)
                {
                    hasMoreSkills = true;
                    break;
                }
            }
        }
        
        if (hasMoreSkills)
        {
            if (autoRollNextSkill)
            {
                // Auto-roll mode: Wait and automatically start next roll
                Invoke(nameof(StartNextSkillRoll), delayBetweenRolls);
            }
            else
            {
                // Manual mode: Wait, then enable button for player to click
                DOVirtual.DelayedCall(delayAfterPopup, () =>
                {
                    if (rollButton != null)
                    {
                        rollButton.interactable = true;
                        
                        // Pulse animation to indicate button is ready
                        if (enableButtonPulse)
                        {
                            buttonPulseTween = rollButton.transform.DOScale(Vector3.one * buttonPulseScale, buttonPulseDuration)
                                .SetEase(Ease.InOutSine)
                                .SetLoops(-1, LoopType.Yoyo);
                        }
                    }
                    
                    if (rollButtonText != null)
                    {
                        // Count remaining rolls
                        int remainingRolls = 0;
                        if (towerRuntime != null)
                        {
                            var skills = towerRuntime.GetAllSkills();
                            for (int i = currentSkillIndex; i < skills.Length; i++)
                            {
                                if (skills[i] != null) remainingRolls++;
                            }
                        }
                        rollButtonText.text = $"NEXT ROLL ({remainingRolls} left)";
                    }
                });
            }
        }
        else
        {
            // No more skills, complete all rolls
            Invoke(nameof(CompleteAllRolls), delayBetweenRolls);
        }
    }

    private void CompleteAllRolls()
    {
        isRolling = false;
        
        // Ensure button stays disabled
        if (rollButton != null)
        {
            rollButton.interactable = false;
        }
        
        // Stop any pulse animation
        buttonPulseTween?.Kill();
        if (rollButton != null)
        {
            rollButton.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutQuad);
        }
        
        if (rollButtonText != null)
        {
            rollButtonText.text = "All Rolls Complete!";
        }

        Debug.Log("[DiceRollUI] All rolls complete - button disabled");

        // Fire completion event
        OnAllRollsComplete?.Invoke();
    }

    private void ShowMultiplierPopup(int skillIndex, int dice1, int dice2)
    {
        float multiplier = TowerBase.GetDiceMultiplier(dice1, dice2);

        if (multiplierPopupPrefab == null)
        {
            Debug.LogError("[DiceRollUI] multiplierPopupPrefab is NULL! Assign the prefab in Inspector.");
            return;
        }

        // Stage 1: Spawn popup at ShowResultDice position (dice result area)
        Transform diceResultPos = showResultDiceTransform != null ? showResultDiceTransform : transform;
        
        Debug.Log($"[DiceRollUI] Instantiating popup at DiceResultText position");
        GameObject popup = Instantiate(multiplierPopupPrefab, transform);
        popup.SetActive(true); // Ensure it's active
        
        // Set the position to match DiceResultText exactly (use RectTransform for UI)
        RectTransform popupRect = popup.GetComponent<RectTransform>();
        RectTransform diceResultRect = diceResultPos.GetComponent<RectTransform>();
        
        if (popupRect != null && diceResultRect != null)
        {
            // Copy all transform properties to match exactly
            popupRect.anchorMin = diceResultRect.anchorMin;
            popupRect.anchorMax = diceResultRect.anchorMax;
            popupRect.anchoredPosition = diceResultRect.anchoredPosition;
            popupRect.sizeDelta = diceResultRect.sizeDelta;
            popupRect.pivot = diceResultRect.pivot;
            popupRect.localScale = Vector3.one; // Will be animated by MultTextEffect
            
            Debug.Log($"[DiceRollUI] Popup RectTransform copied from DiceResultText - anchoredPosition: {popupRect.anchoredPosition}, scale: {popupRect.localScale}");
        }
        else if (popupRect != null)
        {
            // Fallback: use world position
            popupRect.position = diceResultPos.position;
            Debug.Log($"[DiceRollUI] Popup RectTransform position set to: {popupRect.position}");
        }
        else
        {
            popup.transform.position = diceResultPos.position;
            Debug.Log($"[DiceRollUI] Popup transform position set to: {popup.transform.position}");
        }
        
        Debug.Log($"[DiceRollUI] Popup instantiated: {popup.name}, Active: {popup.activeSelf}");
        
        // Set text using MultTextEffect component
        var multEffect = popup.GetComponent<MultTextEffect>();
        if (multEffect != null)
        {
            string skillName = skillIndex >= 0 && towerRuntime != null 
                ? towerRuntime.GetSkill(skillIndex).GetName()
                : "";
                
            Debug.Log($"[DiceRollUI] Found MultTextEffect, setting text: {skillName} x{multiplier:F1}");
            multEffect.SetMultiplierText(multiplier, skillName);
        }
        else
        {
            Debug.LogWarning("[DiceRollUI] No MultTextEffect found, using fallback text setting");
            
            // Fallback: directly set text if no MultTextEffect component
            var popupText = popup.GetComponent<TextMeshProUGUI>();
            if (popupText == null)
                popupText = popup.GetComponentInChildren<TextMeshProUGUI>();
                
            if (popupText != null)
            {
                string skillName = skillIndex >= 0 && towerRuntime != null 
                    ? towerRuntime.GetSkill(skillIndex).GetName()
                    : "";
                    
                popupText.text = skillName != "" 
                    ? $"{skillName}\n×{multiplier:F1}"
                    : $"×{multiplier:F1}";
                    
                Debug.Log($"[DiceRollUI] Text set directly: {popupText.text}");
            }
            else
            {
                Debug.LogError("[DiceRollUI] No TextMeshProUGUI found on popup!");
            }
        }
        
        Debug.Log($"[DiceRollUI] Stage 1: Popup spawned at dice result area - Skill {skillIndex}: ×{multiplier:F1} (Dice: {dice1} + {dice2})");
        
        // Stage 2: After delay, move popup to skill slot position with Y+40 offset
        DOVirtual.DelayedCall(popupDurationAtDiceResult, () =>
        {
            if (popup != null)
            {
                Transform skillSlotPos = GetPopupSpawnPosition(skillIndex);
                Vector3 targetPosition = skillSlotPos.position;
                targetPosition.y += 40f;
                
                // Move popup to skill slot position
                popup.transform.DOMove(targetPosition, 0.5f).SetEase(Ease.OutQuad);
                
                Debug.Log($"[DiceRollUI] Stage 2: Popup moving to skill slot {skillIndex} position");
            }
        });
    }
    
    /// <summary>
    /// Get the spawn position for the multiplier popup based on skill slot index.
    /// </summary>
    private Transform GetPopupSpawnPosition(int skillIndex)
    {
        // If valid skill index and slot transform is assigned, use it
        if (skillIndex >= 0 && skillIndex < skillSlotTransforms.Length 
            && skillSlotTransforms[skillIndex] != null)
        {
            return skillSlotTransforms[skillIndex];
        }
        
        // Fallback to designated spawn point or this transform
        return fallbackPopupSpawnPoint != null ? fallbackPopupSpawnPoint : transform;
    }

    // ====== Manual Control (for testing) ======

    [ContextMenu("Test: Prepare Roll")]
    public void TestPrepare()
    {
        PrepareForRoll();
    }

    [ContextMenu("Test: Auto Roll")]
    public void TestAutoRoll()
    {
        PrepareForRoll();
        OnRollButtonClicked();
    }
}
