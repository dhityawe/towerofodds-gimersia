// SkillSlotDisplay.cs
// Displays all skill slots UI (3 slots)
// Updates automatically when tower skills change

using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// UI component for displaying all skill slots (3 slots).
/// Automatically updates when skills are equipped/unequipped from the tower.
/// </summary>
public class SkillSlotDisplay : MonoBehaviour
{
    [System.Serializable]
    public class SkillSlot
    {
        public GameObject slotObject;
        public Image iconImage;
        public TextMeshProUGUI levelText;
        [HideInInspector] public TowerSkill currentSkill;
    }

    [Header("References")]
    [SerializeField] private TowerRuntime tower;

    [Header("Skill Slots (0-2)")]
    [SerializeField] private SkillSlot slot0;
    [SerializeField] private SkillSlot slot1;
    [SerializeField] private SkillSlot slot2;

    private SkillSlot[] allSlots;

    void Awake()
    {
        // Find tower if not assigned
        if (tower == null)
        {
            tower = FindFirstObjectByType<TowerRuntime>();
        }

        // Build array for easy iteration
        allSlots = new SkillSlot[] { slot0, slot1, slot2 };
    }

    void Start()
    {
        // Initial display for all slots
        RefreshAllSlots();
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
        if (equippedSlotIndex >= 0 && equippedSlotIndex < allSlots.Length)
        {
            allSlots[equippedSlotIndex].currentSkill = skill;
            RefreshSlot(equippedSlotIndex);
            Debug.Log($"[SkillSlotDisplay] Slot {equippedSlotIndex} Equipped: {skill.skillName}");
        }
    }

    /// <summary>
    /// Called when a skill is unequipped from any slot.
    /// </summary>
    private void OnSkillUnequipped(int unequippedSlotIndex)
    {
        if (unequippedSlotIndex >= 0 && unequippedSlotIndex < allSlots.Length)
        {
            allSlots[unequippedSlotIndex].currentSkill = null;
            RefreshSlot(unequippedSlotIndex);
            Debug.Log($"[SkillSlotDisplay] Slot {unequippedSlotIndex} Unequipped");
        }
    }

    /// <summary>
    /// Refresh all skill slots.
    /// </summary>
    public void RefreshAllSlots()
    {
        for (int i = 0; i < allSlots.Length; i++)
        {
            RefreshSlot(i);
        }
    }

    /// <summary>
    /// Refresh a specific skill slot.
    /// </summary>
    private void RefreshSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= allSlots.Length)
            return;

        SkillSlot slot = allSlots[slotIndex];
        if (slot == null || slot.slotObject == null)
            return;

        // Get skill from tower
        TowerSkill skill = null;
        if (tower != null)
        {
            skill = tower.GetSkill(slotIndex);
            slot.currentSkill = skill;
        }

        if (skill != null)
        {
            // Skill is equipped - show skill data
            ShowSkill(slot, skill);
        }
        else
        {
            // Slot is empty - show empty state
            ShowEmptySlot(slot);
        }
    }

    /// <summary>
    /// Display a skill in the specified slot.
    /// </summary>
    private void ShowSkill(SkillSlot slot, TowerSkill skill)
    {
        // Enable the slot GameObject
        slot.slotObject.SetActive(true);

        // Update icon
        if (slot.iconImage != null)
        {
            slot.iconImage.sprite = skill.icon;
            slot.iconImage.enabled = skill.icon != null;
            slot.iconImage.color = Color.white;
        }

        // Update level
        if (slot.levelText != null)
        {
            slot.levelText.text = $"Lv.{skill.GetLevel()}";
            slot.levelText.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Display empty slot state.
    /// </summary>
    private void ShowEmptySlot(SkillSlot slot)
    {
        // Disable the slot GameObject when no skill is equipped
        slot.slotObject.SetActive(false);
    }

    /// <summary>
    /// Get the current skill in a specific slot (can be null).
    /// </summary>
    public TowerSkill GetSkill(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < allSlots.Length)
        {
            return allSlots[slotIndex]?.currentSkill;
        }
        return null;
    }

#if UNITY_EDITOR
    [ContextMenu("Force Refresh All Slots")]
    private void ForceRefresh()
    {
        if (allSlots == null || allSlots.Length == 0)
        {
            allSlots = new SkillSlot[] { slot0, slot1, slot2 };
        }
        RefreshAllSlots();
    }
#endif
}
