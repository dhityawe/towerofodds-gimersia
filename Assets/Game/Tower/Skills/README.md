# Tower Skill System

A modular, ScriptableObject-based skill system for the Tower of Odds game.

## Architecture Overview

### Core Components

1. **TowerBase.cs** - Stateless helper with tower stats struct and dice multiplier logic
2. **TowerRuntime.cs** - MonoBehaviour that manages tower instance state and skill slots
3. **TowerSkill.cs** - Abstract base class for all skills (ScriptableObject)
4. **Skill Implementations** - 5 unique skills with different behaviors

### Tower Stats (No BaseDamage)

```csharp
public struct Stats
{
    float MaxHp;           // Total health
    float Armor;           // Flat damage reduction
    float HpRegenPerSec;   // Passive regeneration
    int BaseAttackCount;   // Used by skills (spread count, orbit count, etc.)
    float AttackSpeed;     // Attack speed multiplier (higher = faster)
}
```

**Key Design Decision**: Each skill has its own `baseDamage` field. Tower stats provide modifiers (`BaseAttackCount`, `AttackSpeed`) that skills interpret uniquely.

---

## Skill System

### How It Works

1. **Skill Slots**: Each tower has 3 skill slots (configurable via inspector or runtime API)
2. **Lifecycle Hooks**:
   - `OnEquip()` - Called when skill is equipped (spawn persistent objects)
   - `OnUnequip()` - Called when skill is removed (cleanup)
   - `Activate()` - Called every attack cycle (shoot projectiles, etc.)
   - `OnUpdate()` - Called every frame (for continuous effects)

3. **Attack Cycle**: Tower attacks at interval `1 / AttackSpeed` seconds. On each attack:
   - Dice are rolled (1-6, 1-6) — replace with your Preparation Phase system
   - All equipped skills' `Activate()` methods are called with dice values

### Skill Slot Management API

```csharp
// Equip a skill to slot 0-2
tower.EquipSkill(skillAsset, slotIndex);

// Unequip a skill
tower.UnequipSkill(slotIndex);

// Get skill in a slot
TowerSkill skill = tower.GetSkill(0);

// Find first empty slot
int emptySlot = tower.GetFirstEmptySlot();

// Check if slot is empty
bool isEmpty = tower.IsSlotEmpty(1);
```

---

## The 5 Skills

### 1. **Dice Skill**
- **Behavior**: Shoots a single dice projectile
- **Damage**: `baseDamage * randomRoll(1-6)`
- **Special**: Temporarily increases tower's `AttackSpeed` based on the roll
- **Uses Tower Stats**: `AttackSpeed` (boosted by roll)

**CreateAssetMenu**: `Tower/Skills/Dice`

```csharp
[Header("Dice Specific")]
public float attackSpeedGainPerDamage = 0.05f; // 5% speed boost per damage point
```

---

### 2. **Chip Skill**
- **Behavior**: Shoots a single chip that spreads on enemy hit
- **Damage**: `baseDamage * diceMultiplier(d1, d2)`
- **Spread Count**: Uses `tower.BaseAttackCount` to determine how many spread projectiles
- **Spread Angle**: Configurable (default 120°)
- **Special**: Spread only happens once per chip

**CreateAssetMenu**: `Tower/Skills/Chip`

```csharp
[Header("Chip Specific")]
public float spreadAngle = 120f;
```

**TODO**: Implement `ChipProjectile` that spawns spread projectiles on hit.

---

### 3. **Cards Skill**
- **Behavior**: Cards orbit continuously around the tower
- **Orbit Count**: Uses `tower.BaseAttackCount` for number of cards
- **Orbit Speed**: Uses `tower.AttackSpeed` for rotation speed
- **Damage**: Applied via collision (continuous)
- **Lifecycle**: Cards are spawned in `OnEquip()`, updated in `OnUpdate()`, destroyed in `OnUnequip()`

**CreateAssetMenu**: `Tower/Skills/Cards`

```csharp
[Header("Cards Specific")]
public float orbitRadius = 2f;
public float baseOrbitSpeed = 90f; // degrees per second
```

**Implementation Notes**:
- Currently spawns placeholder GameObjects
- Replace with actual card prefab instantiation
- Add collision detection in `OnUpdate()` to damage enemies

---

### 4. **Heartflare Skill**
- **Behavior**: Pink light pulses from small to max radius
- **Damage**: `baseDamage * diceMultiplier(d1, d2)`
- **Pulse Speed**: Uses `tower.AttackSpeed` to control pulse expansion rate
- **Visual**: Configurable color (default pink)

**CreateAssetMenu**: `Tower/Skills/Heartflare`

```csharp
[Header("Heartflare Specific")]
public float maxRadius = 5f;
public float basePulseDuration = 1f;
public Color pulseColor = new Color(1f, 0.4f, 0.7f, 0.5f);
```

**TODO**: Implement `HeartflarePulse` effect that expands and damages enemies within radius.

---

### 5. **BombChip Skill**
- **Behavior**: Shoots a chip that explodes on enemy hit
- **Direct Damage**: `baseDamage * diceMultiplier(d1, d2)`
- **Explosion Damage**: `directDamage * explosionDamageMultiplier`
- **Explosion Radius**: Configurable (default 2.5)

**CreateAssetMenu**: `Tower/Skills/BombChip`

```csharp
[Header("BombChip Specific")]
public float explosionRadius = 2.5f;
public float explosionDamageMultiplier = 0.8f;
```

**TODO**: Implement `BombChipProjectile` with explosion logic on hit.

---

## Setup Guide

### 1. Create Skill Assets

In Unity Editor:
1. Right-click in Project window → `Create > Tower > Skills > [Skill Type]`
2. Configure `baseDamage` and skill-specific parameters
3. Assign an icon (optional)

### 2. Assign Skills to Tower

**Method A: Inspector (Design-time)**
1. Select your Tower GameObject
2. In `TowerRuntime` component, expand "Skill Slots"
3. Drag skill assets into slots 0-2

**Method B: Runtime (Code)**
```csharp
TowerRuntime tower = GetComponent<TowerRuntime>();

// Load skill assets
TowerSkill diceSkill = Resources.Load<DiceSkill>("Skills/DiceSkill");
TowerSkill chipSkill = Resources.Load<ChipSkill>("Skills/ChipSkill");

// Equip
tower.EquipSkill(diceSkill, 0);
tower.EquipSkill(chipSkill, 1);
```

### 3. Replace Placeholder Dice Rolls

In `TowerRuntime.DoAttack()`, replace:
```csharp
int d1 = UnityEngine.Random.Range(1, 7);
int d2 = UnityEngine.Random.Range(1, 7);
```

With your Preparation Phase dice system:
```csharp
int d1 = PreparationPhase.GetDice1();
int d2 = PreparationPhase.GetDice2();
```

---

## Next Steps (TODOs)

### Projectile Systems
Each skill needs a corresponding projectile prefab/script:

1. **DiceProjectile** - Basic projectile with damage
2. **ChipProjectile** - Projectile that spreads on hit
3. **BombChipProjectile** - Projectile that explodes on hit
4. **HeartflarePulse** - Expanding damage ring effect
5. **CardPrefab** - Visual card with collision detection

### Enemy Targeting
Currently skills use placeholder `targetEnemy`. Implement:
```csharp
public interface ITargetProvider
{
    Transform GetNearestEnemy(Vector3 fromPosition);
}
```

### Advanced Features
- **Skill Upgrades**: Add `level` field to skills, scale damage/effects
- **Skill Combinations**: Detect multiple skills and apply synergies
- **Cooldowns**: Per-skill cooldowns instead of global attack timer
- **Visual Effects**: Skill activation VFX, damage numbers
- **Skill UI**: Inventory, drag-drop equipping

---

## Design Benefits

✅ **Modular**: Each skill is a separate asset, easy to balance and test  
✅ **Extensible**: Add new skills by inheriting `TowerSkill`  
✅ **Designer-Friendly**: No code required to create skill variants  
✅ **Safe**: Skills manage their own state; no global mutation  
✅ **Reusable**: Skills can be shared across multiple tower types  
✅ **Testable**: Skills can be unit tested without Unity runtime

---

## Example Workflow

```csharp
// Game start: Load tower with default skills
void Start()
{
    TowerRuntime tower = FindObjectOfType<TowerRuntime>();
    
    // Load skills from Resources or AssetDatabase
    var dice = Resources.Load<DiceSkill>("Skills/Dice");
    var heartflare = Resources.Load<HeartflareSkill>("Skills/Heartflare");
    
    tower.EquipSkill(dice, 0);
    tower.EquipSkill(heartflare, 1);
}

// Player unlocks new skill
void UnlockSkill(TowerSkill newSkill)
{
    TowerRuntime tower = FindObjectOfType<TowerRuntime>();
    int slot = tower.GetFirstEmptySlot();
    
    if (slot != -1)
    {
        tower.EquipSkill(newSkill, slot);
        Debug.Log($"Equipped {newSkill.skillName} to slot {slot}");
    }
    else
    {
        Debug.Log("All skill slots full! Choose a skill to replace.");
        // Show UI for slot selection
    }
}

// Replace a skill
void ReplaceSkill(TowerSkill newSkill, int slotIndex)
{
    TowerRuntime tower = FindObjectOfType<TowerRuntime>();
    tower.EquipSkill(newSkill, slotIndex); // Auto-unequips old skill
}
```

---

## File Structure

```
Assets/Game/Tower/
├── TowerBase.cs              # Stateless helpers, Stats struct
├── TowerRuntime.cs           # Tower instance, skill slot manager
└── Skills/
    ├── TowerSkill.cs         # Abstract base class
    ├── DiceSkill.cs
    ├── ChipSkill.cs
    ├── CardsSkill.cs
    ├── HeartflareSkill.cs
    └── BombChipSkill.cs
```

Create skill assets in: `Assets/Game/Scriptables/Skills/`

---

## FAQ

**Q: Can a skill use both `Activate()` and `OnUpdate()`?**  
A: Yes! See `CardsSkill` — it spawns cards in `OnEquip()`, updates their position in `OnUpdate()`, and can trigger effects in `Activate()`.

**Q: How do I make a skill that doesn't attack every cycle?**  
A: Add a cooldown timer to your skill class. Check it in `Activate()` and early-return if on cooldown.

**Q: Can I have more than 3 skill slots?**  
A: Yes! Change `MAX_SKILL_SLOTS` constant in `TowerRuntime.cs`.

**Q: How do I save/load equipped skills?**  
A: Serialize skill asset names/GUIDs and restore via `Resources.Load()` or `AssetDatabase.LoadAssetAtPath()`.

---

**Created by**: GitHub Copilot  
**Date**: November 8, 2025  
**Version**: 1.0
