// SkillSlotDisplay.cs
// Displays a single skill slot UI
// Updates automatically when tower skills change

using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// UI component for displaying a single skill slot.
/// Automatically updates when skills are equipped/unequipped from the tower.
/// </summary>
public class SkillSlotDisplay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TowerRuntime tower;
    [SerializeField] private int slotIndex = 0; // 0, 1, or 2

    [Header("UI Components")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI levelText;

    private TowerSkill currentSkill;

    void Awake()
    {
        // Find tower if not assigned
        if (tower == null)
        {
            tower = FindFirstObjectByType<TowerRuntime>();
        }
    }

    void Start()
    {
        // Initial display
        RefreshDisplay();
    }

    void OnEnable()
    {
        // Subscribe to tower skill events
        if (tower != null)
        {
            tower.OnSkillEquipped += OnSkillEquipped;
            tower.OnSkillUnequipped += OnSkillUnequipped;
        }
    }

    void OnDisable()
    {
        // Unsubscribe from events
        if (tower != null)
        {
            tower.OnSkillEquipped -= OnSkillEquipped;
            tower.OnSkillUnequipped -= OnSkillUnequipped;
        }
    }

    /// <summary>
    /// Called when a skill is equipped to any slot.
    /// </summary>
    private void OnSkillEquipped(int equippedSlotIndex, TowerSkill skill)
    {
        // Only update if this is our slot
        if (equippedSlotIndex == slotIndex)
        {
            currentSkill = skill;
            RefreshDisplay();
            Debug.Log($"[SkillSlotDisplay {slotIndex}] Equipped: {skill.skillName}");
        }
    }

    /// <summary>
    /// Called when a skill is unequipped from any slot.
    /// </summary>
    private void OnSkillUnequipped(int unequippedSlotIndex)
    {
        // Only update if this is our slot
        if (unequippedSlotIndex == slotIndex)
        {
            currentSkill = null;
            RefreshDisplay();
            Debug.Log($"[SkillSlotDisplay {slotIndex}] Unequipped");
        }
    }

    /// <summary>
    /// Refresh the UI display based on current skill data.
    /// </summary>
    public void RefreshDisplay()
    {
        // Get skill from tower
        if (tower != null)
        {
            currentSkill = tower.GetSkill(slotIndex);
        }

        if (currentSkill != null)
        {
            // Skill is equipped - show skill data
            ShowSkill(currentSkill);
        }
        else
        {
            // Slot is empty - show empty state
            ShowEmptySlot();
        }
    }

    /// <summary>
    /// Display a skill in this slot.
    /// </summary>
    private void ShowSkill(TowerSkill skill)
    {
        // Enable the GameObject (parent that contains skill display components)
        gameObject.SetActive(true);

        // Update icon
        if (iconImage != null)
        {
            iconImage.sprite = skill.icon;
            iconImage.enabled = skill.icon != null;
            iconImage.color = Color.white;
        }

        // Update level
        if (levelText != null)
        {
            levelText.text = $"Lv.{skill.GetLevel()}";
            levelText.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Display empty slot state.
    /// </summary>
    private void ShowEmptySlot()
    {
        // Disable the entire GameObject when no skill is equipped
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Get the current skill in this slot (can be null).
    /// </summary>
    public TowerSkill GetCurrentSkill() => currentSkill;

    /// <summary>
    /// Get the slot index this display represents.
    /// </summary>
    public int GetSlotIndex() => slotIndex;

#if UNITY_EDITOR
    [ContextMenu("Force Refresh")]
    private void ForceRefresh()
    {
        RefreshDisplay();
    }
#endif
}
