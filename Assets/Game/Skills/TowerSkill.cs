// TowerSkill.cs
// Base ScriptableObject for all tower skills

using UnityEngine;

/// <summary>
/// Base class for all tower skills. Create skill assets via right-click > Create > Tower > Skills.
/// Each skill defines its own damage, behavior, and how it uses tower stats (BaseAttackCount, AttackSpeed).
/// </summary>
public abstract class TowerSkill : ScriptableObject, IShopItem
{
    [Header("Skill Identity")]
    public string skillName = "New Skill";
    [TextArea(2, 4)] public string description = "Skill description here...";
    public Sprite icon;

    [Header("Shop Properties")]
    [Min(0)] public int cost = 100;
    public ShopItemRarity rarity = ShopItemRarity.Common;

    [Header("Base Stats")]
    [Min(0f)] public float baseDamage = 10f;

    [Header("Audio")]
    [Tooltip("Sound effect when skill hits enemy")]
    [SerializeField] protected AudioClip hitSound;
    
    [Tooltip("Volume for hit sound (0-1)")]
    [SerializeField, Range(0f, 1f)] protected float hitSoundVolume = 1f;

    [Header("Upgrade System")]
    [Tooltip("Max level this skill can reach")]
    [Min(1)] public int maxLevel = 5;
    [Tooltip("Damage increase per level (%)")]
    [Range(0f, 100f)] public float damageIncreasePerLevel = 10f; // 10% per level

    [Header("Runtime State (Don't Edit)")]
    [SerializeField] private int currentLevel = 1;

    /// <summary>
    /// Called when the skill is activated (usually on attack cycle).
    /// </summary>
    /// <param name="tower">The tower runtime instance owning this skill</param>
    /// <param name="slotIndex">Which skill slot (0-2) this skill is in</param>
    /// <param name="diceRoll1">First dice roll (1-6), from Preparation Phase</param>
    /// <param name="diceRoll2">Second dice roll (1-6), from Preparation Phase</param>
    public abstract void Activate(TowerRuntime tower, int slotIndex, int diceRoll1, int diceRoll2);

    /// <summary>
    /// Called every frame for skills that need continuous updates (e.g., orbiting cards).
    /// </summary>
    public virtual void OnUpdate(TowerRuntime tower, int slotIndex, float deltaTime) { }

    /// <summary>
    /// Called when the skill is equipped to a slot (for initialization).
    /// </summary>
    public virtual void OnEquip(TowerRuntime tower, int slotIndex) { }

    /// <summary>
    /// Called when the skill is removed from a slot (for cleanup).
    /// </summary>
    public virtual void OnUnequip(TowerRuntime tower, int slotIndex) { }

    /// <summary>
    /// Helper: Get final damage after dice multiplier.
    /// </summary>
    protected float GetDamageWithDice(int d1, int d2, float extraMultiplier = 1f)
    {
        float diceMultiplier = TowerBase.GetDiceMultiplier(d1, d2);
        float levelMultiplier = 1f + (currentLevel - 1) * (damageIncreasePerLevel / 100f);
        return baseDamage * diceMultiplier * levelMultiplier * Mathf.Max(0f, extraMultiplier);
    }

    /// <summary>
    /// Play hit sound effect at given position.
    /// </summary>
    protected void PlayHitSound(Vector3 position)
    {
        if (hitSound != null)
        {
            AudioSource.PlayClipAtPoint(hitSound, position, hitSoundVolume);
        }
    }

    /// <summary>
    /// Get the current level of this skill.
    /// </summary>
    public int GetLevel() => currentLevel;

    /// <summary>
    /// Get the max level of this skill.
    /// </summary>
    public int GetMaxLevel() => maxLevel;

    /// <summary>
    /// Check if this skill can be upgraded.
    /// </summary>
    public bool CanLevelUp() => currentLevel < maxLevel;

    /// <summary>
    /// Upgrade this skill to the next level.
    /// Override this in derived classes to add custom upgrade effects.
    /// </summary>
    public virtual bool LevelUp()
    {
        if (!CanLevelUp())
        {
            Debug.LogWarning($"{skillName} is already at max level ({maxLevel})!");
            return false;
        }

        currentLevel++;
        OnLevelUp(currentLevel);
        Debug.Log($"{skillName} upgraded to level {currentLevel}!");
        return true;
    }

    /// <summary>
    /// Called when skill levels up. Override to add custom upgrade effects.
    /// </summary>
    protected virtual void OnLevelUp(int newLevel)
    {
        // Default: just increase damage via GetDamageWithDice()
        // Override in derived skills for custom behavior
    }

    /// <summary>
    /// Reset skill to level 1 (for new game or testing).
    /// </summary>
    public virtual void ResetLevel()
    {
        currentLevel = 1;
    }

    // ====== IShopItem Implementation ======
    public virtual string GetName() => skillName;
    public virtual string GetDescription() => description;
    public virtual Sprite GetIcon() => icon;
    public virtual int GetCost() => cost;
    public virtual ShopItemRarity GetRarity() => rarity;
    public virtual ShopItemCategory GetCategory() => ShopItemCategory.Skill;

    public virtual bool CanPurchase(TowerRuntime tower)
    {
        // Can purchase if: have empty slot OR already have this skill equipped (for upgrade)
        return tower.GetFirstEmptySlot() != -1 || HasSkillEquipped(tower);
    }

    public virtual bool OnPurchase(TowerRuntime tower)
    {
        // Check if skill is already equipped (upgrade path)
        int equippedSlot = GetEquippedSlotIndex(tower);
        
        if (equippedSlot != -1)
        {
            // Skill already equipped - upgrade it instead
            if (CanLevelUp())
            {
                LevelUp();
                Debug.Log($"Upgraded {skillName} to level {currentLevel}!");
                return true;
            }
            else
            {
                Debug.LogWarning($"{skillName} is already at max level!");
                return false;
            }
        }
        
        // New skill - equip to empty slot
        int emptySlot = tower.GetFirstEmptySlot();
        if (emptySlot == -1)
        {
            Debug.LogWarning($"Cannot purchase {skillName}: All skill slots are full!");
            return false;
        }

        tower.EquipSkill(this, emptySlot);
        Debug.Log($"Purchased and equipped {skillName} to slot {emptySlot}");
        return true;
    }

    /// <summary>
    /// Check if this skill is equipped on the tower.
    /// </summary>
    private bool HasSkillEquipped(TowerRuntime tower)
    {
        return GetEquippedSlotIndex(tower) != -1;
    }

    /// <summary>
    /// Get the slot index where this skill is equipped, or -1 if not equipped.
    /// </summary>
    private int GetEquippedSlotIndex(TowerRuntime tower)
    {
        TowerSkill[] skills = tower.GetAllSkills();
        for (int i = 0; i < skills.Length; i++)
        {
            if (skills[i] == this) return i;
        }
        return -1;
    }
}
