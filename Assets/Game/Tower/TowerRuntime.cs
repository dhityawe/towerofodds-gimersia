// TowerRuntime.cs
// Attach this to your Tower prefab or Tower GameObject in the scene.

using System;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-50)]
public class TowerRuntime : MonoBehaviour
{
    public const int MAX_SKILL_SLOTS = 3;

    [Header("Base Stats (Editable)")]
    [SerializeField, Min(1f)] private float baseHp = 100f;
    [SerializeField, Min(0f)] private float baseArmor = 5f;
    [SerializeField, Min(0f)] private float baseHpRegen = 1f;
    [SerializeField, Min(1)] private int baseAttackCount = 1;
    [SerializeField, Min(0.01f)] private float baseAttackSpeed = 1f;

    [Header("Skill Slots (Max 3)")]
    [SerializeField] private TowerSkill[] skillSlots = new TowerSkill[MAX_SKILL_SLOTS];

    [Header("Runtime State (Read-Only)")]
    [SerializeField] private float currentHp;
    [SerializeField] private bool isDead;

    // Expose death event so other systems can subscribe without direct access to fields
    public event Action OnDeath;

    private float attackTimer;
    private TowerBase.Stats currentStats;

    void Awake()
    {
        currentStats = new TowerBase.Stats(
            baseHp,
            baseArmor,
            baseHpRegen,
            baseAttackCount,
            baseAttackSpeed
        );

        // Initialize runtime state
        currentHp = Mathf.Clamp(currentStats.MaxHp, 1f, float.MaxValue);
        isDead = false;
        attackTimer = 0f;

        // Equip skills
        for (int i = 0; i < skillSlots.Length; i++)
        {
            if (skillSlots[i] != null)
            {
                skillSlots[i].OnEquip(this, i);
            }
        }
    }

    void OnDestroy()
    {
        // Cleanup skills
        for (int i = 0; i < skillSlots.Length; i++)
        {
            if (skillSlots[i] != null)
            {
                skillSlots[i].OnUnequip(this, i);
            }
        }
    }

    void Update()
    {
        // Apply passive regen
        TickRegen(Time.deltaTime);

        // Keep inspector fields in sync
        currentHp = Mathf.Clamp(currentHp, 0f, currentStats.MaxHp);
        isDead = currentHp <= 0f;

        if (isDead) return;

        // Update skills (for continuous effects like orbiting cards)
        for (int i = 0; i < skillSlots.Length; i++)
        {
            if (skillSlots[i] != null)
            {
                skillSlots[i].OnUpdate(this, i, Time.deltaTime);
            }
        }

        // Attack cycle
        attackTimer += Time.deltaTime;
        float attackInterval = 1f / currentStats.AttackSpeed; // Convert speed to interval
        if (attackTimer >= attackInterval)
        {
            attackTimer = 0f;
            DoAttack();
        }
    }

    private void TickRegen(float deltaTime)
    {
        if (isDead || currentStats.HpRegenPerSec <= 0f) return;
        currentHp = Mathf.Min(currentStats.MaxHp, currentHp + currentStats.HpRegenPerSec * Mathf.Max(0f, deltaTime));
    }

    private void DoAttack()
    {
        // Example roll (replace with your Preparation Phase dice result)
        int d1 = UnityEngine.Random.Range(1, 7);
        int d2 = UnityEngine.Random.Range(1, 7);

        // Activate all equipped skills
        for (int i = 0; i < skillSlots.Length; i++)
        {
            if (skillSlots[i] != null)
            {
                skillSlots[i].Activate(this, i, d1, d2);
            }
        }
    }

    // ====== Instance API ======
    public float ApplyDamage(float rawDamage)
    {
        if (isDead) return 0f;
        float effective = Mathf.Max(0f, rawDamage - currentStats.Armor);
        // Always allow at least chip damage if raw > 0
        if (rawDamage > 0f) effective = Mathf.Max(1f, effective);
        currentHp = Mathf.Max(0f, currentHp - effective);
        if (currentHp <= 0f && !isDead)
        {
            isDead = true;
            OnDeath?.Invoke();
        }
        return effective;
    }

    public void Heal(float amount)
    {
        if (amount <= 0f || isDead) return;
        currentHp = Mathf.Min(currentStats.MaxHp, currentHp + amount);
    }

    public float GetCurrentHp() => currentHp;
    public bool IsDead() => isDead;
    public TowerBase.Stats GetStats() => currentStats;

    // ====== Helpers to mutate stats safely (items, blessings)
    public void SetStats(TowerBase.Stats stats)
    {
        currentStats = stats;
        currentHp = Mathf.Clamp(currentHp, 0f, currentStats.MaxHp);
    }

    public void ResetToDefault()
    {
        SetStats(TowerBase.Default);
        currentHp = currentStats.MaxHp;
        isDead = false;
        attackTimer = 0f;
    }

    public void AddMaxHp(float flat)
    {
        currentStats.MaxHp = Mathf.Max(1f, currentStats.MaxHp + flat);
        currentHp = Mathf.Min(currentHp, currentStats.MaxHp);
    }

    public void AddArmor(float flat) => currentStats.Armor = Mathf.Max(0f, currentStats.Armor + flat);
    public void AddRegen(float flat) => currentStats.HpRegenPerSec = Mathf.Max(0f, currentStats.HpRegenPerSec + flat);
    public void AddAttackCount(int add) => currentStats.BaseAttackCount = Mathf.Max(1, currentStats.BaseAttackCount + add);
    public void MultiplyAttackSpeed(float mult)
    {
        mult = Mathf.Max(0.01f, mult);
        currentStats.AttackSpeed = Mathf.Max(0.01f, currentStats.AttackSpeed * mult);
    }

    // ====== Skill Slot Management ======
    /// <summary>Equip a skill to a specific slot (0-2). Automatically unequips previous skill.</summary>
    public bool EquipSkill(TowerSkill skill, int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= MAX_SKILL_SLOTS)
        {
            Debug.LogError($"Invalid slot index {slotIndex}. Must be 0-{MAX_SKILL_SLOTS - 1}");
            return false;
        }

        // Unequip current skill in slot
        if (skillSlots[slotIndex] != null)
        {
            skillSlots[slotIndex].OnUnequip(this, slotIndex);
        }

        // Equip new skill
        skillSlots[slotIndex] = skill;
        if (skill != null)
        {
            skill.OnEquip(this, slotIndex);
        }

        return true;
    }

    /// <summary>Unequip a skill from a specific slot.</summary>
    public void UnequipSkill(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= MAX_SKILL_SLOTS) return;

        if (skillSlots[slotIndex] != null)
        {
            skillSlots[slotIndex].OnUnequip(this, slotIndex);
            skillSlots[slotIndex] = null;
        }
    }

    /// <summary>Get the skill in a specific slot (can be null).</summary>
    public TowerSkill GetSkill(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= MAX_SKILL_SLOTS) return null;
        return skillSlots[slotIndex];
    }

    /// <summary>Get all equipped skills.</summary>
    public TowerSkill[] GetAllSkills() => skillSlots;

    /// <summary>Check if a slot is empty.</summary>
    public bool IsSlotEmpty(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= MAX_SKILL_SLOTS) return true;
        return skillSlots[slotIndex] == null;
    }

    /// <summary>Get the first empty slot index, or -1 if all full.</summary>
    public int GetFirstEmptySlot()
    {
        for (int i = 0; i < MAX_SKILL_SLOTS; i++)
        {
            if (skillSlots[i] == null) return i;
        }
        return -1;
    }

    // Optional gizmo for editor visualization
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position + Vector3.up * 0.5f, new Vector3(1f, 1f, 1f));
    }
}
