// TowerRuntime.cs
// Attach this to your Tower prefab or Tower GameObject in the scene.

using System;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-50)]
public class TowerRuntime : MonoBehaviour
{
    public const int MAX_SKILL_SLOTS = 3;
    public const int MAX_ITEM_SLOTS = 10; // Adjust as needed

    [Header("Base Stats (Editable)")]
    [SerializeField, Min(1f)] private float baseHp = 100f;
    [SerializeField, Min(0f)] private float baseArmor = 5f;
    [SerializeField, Min(0f)] private float baseHpRegen = 1f;
    [SerializeField, Min(1)] private int baseAttackCount = 1;
    [SerializeField, Min(0.01f)] private float baseAttackSpeed = 1f;

    [Header("Skill Slots (Max 3)")]
    [SerializeField] private TowerSkill[] skillSlots = new TowerSkill[MAX_SKILL_SLOTS];

    [Header("Item Slots")]
    [SerializeField] private TowerItem[] itemSlots = new TowerItem[MAX_ITEM_SLOTS];

    [Header("Runtime State (Read-Only)")]
    [SerializeField] private float currentHp;
    [SerializeField] private bool isDead;

    // Events
    public event Action OnDeath;
    public event Action<int, TowerSkill> OnSkillEquipped; // slotIndex, skill
    public event Action<int> OnSkillUnequipped; // slotIndex
    public event Action<int, TowerItem> OnItemAdded; // slotIndex, item
    public event Action<int> OnItemRemoved; // slotIndex

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
        Debug.Log($"[TowerRuntime] DoAttack() called at {Time.time:F2} | Tower: {gameObject.name}");
        
        // Roll dice once per attack cycle
        int d1 = UnityEngine.Random.Range(1, 7);
        int d2 = UnityEngine.Random.Range(1, 7);

        // Count equipped skills
        int equippedCount = 0;
        for (int i = 0; i < skillSlots.Length; i++)
        {
            if (skillSlots[i] != null)
            {
                equippedCount++;
                Debug.Log($"[TowerRuntime] Slot {i} has skill: {skillSlots[i].skillName}");
            }
        }
        Debug.Log($"[TowerRuntime] Total equipped skills: {equippedCount} | BaseAttackCount: {currentStats.BaseAttackCount}");

        // Activate all equipped skills (BaseAttackCount not used in this version)
        // For multi-attack, wrap this in a loop: for(int n = 0; n < BaseAttackCount; n++)
        for (int i = 0; i < skillSlots.Length; i++)
        {
            if (skillSlots[i] != null)
            {
                Debug.Log($"[TowerRuntime] Activating slot {i}: {skillSlots[i].skillName}");
                skillSlots[i].Activate(this, i, d1, d2);
            }
        }
        Debug.Log($"[TowerRuntime] DoAttack() COMPLETE");
    }

    // ====== Enemy Targeting Helpers (Integration with old enemy system) ======
    
    /// <summary>
    /// Find the closest alive enemy within range.
    /// Returns null if no enemy in range.
    /// </summary>
    public TowerOfOdds.Enemies.BaseEnemy FindClosestEnemy(float range = 15f)
    {
        var allEnemies = UnityEngine.Object.FindObjectsByType<TowerOfOdds.Enemies.BaseEnemy>(FindObjectsSortMode.None);
        
        TowerOfOdds.Enemies.BaseEnemy closest = null;
        float closestDistance = float.MaxValue;

        foreach (var enemy in allEnemies)
        {
            if (!enemy.IsAlive) continue;

            float distance = Vector3.Distance(transform.position, enemy.GetPosition());
            if (distance <= range && distance < closestDistance)
            {
                closest = enemy;
                closestDistance = distance;
            }
        }

        return closest;
    }

    /// <summary>
    /// Find multiple closest enemies within range, sorted by distance.
    /// </summary>
    public TowerOfOdds.Enemies.BaseEnemy[] FindClosestEnemies(int count, float range = 15f)
    {
        var allEnemies = UnityEngine.Object.FindObjectsByType<TowerOfOdds.Enemies.BaseEnemy>(FindObjectsSortMode.None);
        
        var enemiesInRange = new System.Collections.Generic.List<(TowerOfOdds.Enemies.BaseEnemy enemy, float distance)>();

        foreach (var enemy in allEnemies)
        {
            if (!enemy.IsAlive) continue;

            float distance = Vector3.Distance(transform.position, enemy.GetPosition());
            if (distance <= range)
            {
                enemiesInRange.Add((enemy, distance));
            }
        }

        // Sort by distance
        enemiesInRange.Sort((a, b) => a.distance.CompareTo(b.distance));

        // Take top N
        int resultCount = Mathf.Min(count, enemiesInRange.Count);
        var result = new TowerOfOdds.Enemies.BaseEnemy[resultCount];
        for (int i = 0; i < resultCount; i++)
        {
            result[i] = enemiesInRange[i].enemy;
        }

        return result;
    }

    /// <summary>
    /// Get all alive enemies within range.
    /// </summary>
    public TowerOfOdds.Enemies.BaseEnemy[] GetAllEnemiesInRange(float range = 15f)
    {
        var allEnemies = UnityEngine.Object.FindObjectsByType<TowerOfOdds.Enemies.BaseEnemy>(FindObjectsSortMode.None);
        var result = new System.Collections.Generic.List<TowerOfOdds.Enemies.BaseEnemy>();

        foreach (var enemy in allEnemies)
        {
            if (!enemy.IsAlive) continue;

            float distance = Vector3.Distance(transform.position, enemy.GetPosition());
            if (distance <= range)
            {
                result.Add(enemy);
            }
        }

        return result.ToArray();
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

    /// <summary>
    /// Reduce attack interval by a flat amount (makes attacks faster).
    /// Example: intervalReduction = -0.15 means subtract 0.15s from interval.
    /// Clamps to minInterval (default 0.15s) to prevent infinite speed.
    /// </summary>
    public void AddAttackSpeedFlat(float intervalReduction, float minInterval = 0.15f)
    {
        float currentInterval = 1f / currentStats.AttackSpeed;
        float newInterval = currentInterval + intervalReduction; // intervalReduction is negative to speed up
        newInterval = Mathf.Max(minInterval, newInterval); // Clamp minimum
        currentStats.AttackSpeed = 1f / newInterval;
        Debug.Log($"[TowerRuntime] Flat interval change: {intervalReduction}s | New interval: {newInterval}s | New speed: {currentStats.AttackSpeed}");
    }

    // ====== Temporary Buff System ======
    
    private Coroutine activeSpeedBuffCoroutine;
    
    /// <summary>
    /// Temporarily reduce AttackSpeed cooldown by the specified amount.
    /// Clamps to minAttackSpeed, then restores after duration.
    /// Used by DiceSkill to hasten next attack.
    /// </summary>
    public void ReduceAttackSpeedTemporarily(float reduction, float duration, float minAttackSpeed = 0.15f)
    {
        // Cancel previous buff if still active
        if (activeSpeedBuffCoroutine != null)
        {
            StopCoroutine(activeSpeedBuffCoroutine);
        }
        
        activeSpeedBuffCoroutine = StartCoroutine(ApplyTemporarySpeedBuff(reduction, duration, minAttackSpeed));
    }
    
    private System.Collections.IEnumerator ApplyTemporarySpeedBuff(float reduction, float duration, float minAttackSpeed)
    {
        // Store original attack speed
        float originalSpeed = currentStats.AttackSpeed;
        
        // Calculate current cooldown (time between attacks)
        float currentCooldown = 1f / originalSpeed;
        
        // Reduce cooldown by specified amount
        float newCooldown = Mathf.Max(minAttackSpeed, currentCooldown - reduction);
        
        // Convert back to attack speed (attacks per second)
        float newSpeed = 1f / newCooldown;
        
        currentStats.AttackSpeed = newSpeed;
        
        Debug.Log($"[TowerRuntime] Speed buff active: {currentCooldown:F3}s → {newCooldown:F3}s cooldown ({originalSpeed:F2} → {newSpeed:F2} attacks/sec) for {duration}s");
        
        // Wait for duration
        yield return new WaitForSeconds(duration);
        
        // Restore original speed
        currentStats.AttackSpeed = originalSpeed;
        
        Debug.Log($"[TowerRuntime] Speed buff expired: restored to {currentCooldown:F3}s cooldown ({originalSpeed:F2} attacks/sec)");
        
        activeSpeedBuffCoroutine = null;
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
            OnSkillEquipped?.Invoke(slotIndex, skill);
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
            OnSkillUnequipped?.Invoke(slotIndex);
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

    // ====== Item Management ======

    /// <summary>Add an item to the tower's inventory.</summary>
    public bool AddItem(TowerItem item)
    {
        if (item == null) return false;

        // Find first empty slot
        for (int i = 0; i < MAX_ITEM_SLOTS; i++)
        {
            if (itemSlots[i] == null)
            {
                itemSlots[i] = item;
                item.ApplyEffect(this);
                OnItemAdded?.Invoke(i, item);
                Debug.Log($"[TowerRuntime] Item added to slot {i}: {item.GetName()}");
                return true;
            }
        }

        Debug.LogWarning("[TowerRuntime] No empty item slots available!");
        return false;
    }

    /// <summary>Remove an item from a specific slot.</summary>
    public bool RemoveItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= MAX_ITEM_SLOTS) return false;
        if (itemSlots[slotIndex] == null) return false;

        var item = itemSlots[slotIndex];
        item.RemoveEffect(this);
        itemSlots[slotIndex] = null;
        OnItemRemoved?.Invoke(slotIndex);
        Debug.Log($"[TowerRuntime] Item removed from slot {slotIndex}: {item.GetName()}");
        return true;
    }

    /// <summary>Get the item in a specific slot (can be null).</summary>
    public TowerItem GetItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= MAX_ITEM_SLOTS) return null;
        return itemSlots[slotIndex];
    }

    /// <summary>Get all equipped items.</summary>
    public TowerItem[] GetAllItems() => itemSlots;

    /// <summary>Get count of equipped items (non-null).</summary>
    public int GetItemCount()
    {
        int count = 0;
        for (int i = 0; i < MAX_ITEM_SLOTS; i++)
        {
            if (itemSlots[i] != null) count++;
        }
        return count;
    }

    // Optional gizmo for editor visualization
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position + Vector3.up * 0.5f, new Vector3(1f, 1f, 1f));
    }
}
