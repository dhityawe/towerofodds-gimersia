// TowerBase.cs
// Drop this anywhere in your Unity project (e.g., Scripts/Tower/)

using UnityEngine;

/// <summary>
/// Lightweight, stateless helpers and shared types for tower stats and dice/damage rules.
/// This class intentionally does NOT hold mutable runtime state. Runtime state should be
/// owned by a MonoBehaviour (e.g. `TowerRuntime`) so each tower instance is independent
/// and safe from accidental global modification.
/// </summary>
public static class TowerBase
{
    // ====== PUBLIC DATA ======
    public struct Stats
    {
        public float MaxHp;              // Total health
        public float Armor;              // Flat damage reduction per hit
        public float HpRegenPerSec;      // Passive regeneration
        public int   BaseAttackCount;    // Projectiles fired per base cycle
        public float AttackSpeed;        // Attack speed multiplier

        public Stats(
            float maxHp, float armor, float hpRegenPerSec,
            int baseAttackCount, float attackSpeed)
        {
            MaxHp = maxHp;
            Armor = armor;
            HpRegenPerSec = hpRegenPerSec;
            BaseAttackCount = Mathf.Max(1, baseAttackCount);
            AttackSpeed = Mathf.Max(0.01f, attackSpeed);
        }
    }

    // Default "v0.1 – Early Game Balance"
    public static readonly Stats Default = new Stats(
        maxHp: 100f,
        armor: 5f,
        hpRegenPerSec: 1f,
        baseAttackCount: 1,
        attackSpeed: 1.0f
    );

    // ====== DICE & DAMAGE RULES (stateless helpers) ======
    /// <summary>
    /// Calculate tower dice multiplier from 2d6 roll.
    /// Formula: (dice1 + dice2) / 10f
    /// Range: 0.2 (snake eyes 1+1) to 1.2 (boxcars 6+6)
    /// </summary>
    public static float GetDiceMultiplier(int dice1, int dice2)
    {
        // Clamp dice to 1..6 for safety
        dice1 = Mathf.Clamp(dice1, 1, 6);
        dice2 = Mathf.Clamp(dice2, 1, 6);
        return (dice1 + dice2) / 10f; // e.g., 2→0.2 up to 12→1.2
    }
}
