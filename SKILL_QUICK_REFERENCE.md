# Tower Skill System - Quick Reference

## Core Methods

### TowerRuntime - Enemy Targeting
```csharp
// Single closest enemy
var enemy = tower.FindClosestEnemy(15f);

// Multiple enemies (sorted by distance)
var enemies = tower.FindClosestEnemies(3, 15f);

// All enemies in range
var allEnemies = tower.GetAllEnemiesInRange(15f);
```

### Enemy Properties
```csharp
enemy.transform          // For projectile targeting
enemy.TakeDamage(float)  // Apply damage
enemy.IsAlive            // Check alive status
enemy.GetPosition()      // Vector3 position
enemy.GetCurrentHealth() // Current HP
enemy.GetMaxHealth()     // Max HP
```

### Projectile Usage
```csharp
// Spawn and init projectile to shoot enemy
GameObject obj = Instantiate(projectilePrefab, tower.transform.position, Quaternion.identity);
var projectile = obj.GetComponent<TowerOfOdds.Combat.Projectile>();

// Init: (target, damage, speed, isTargetTower)
projectile.Init(enemy.transform, damage, 20f, false);
```

### Skill Base Methods
```csharp
// In your skill's Activate method:
float damage = GetDamageWithDice(d1, d2);  // Apply dice multiplier
float baseDmg = baseDamage;                // Your skill's base damage field

// Lifecycle hooks:
OnEquip(tower, slot)    // Called when skill equipped
OnUnequip(tower, slot)  // Called when skill unequipped
OnUpdate(tower, slot, deltaTime)  // Called every frame if needed
```

## Skill Template

```csharp
using UnityEngine;

[CreateAssetMenu(fileName = "NewSkill", menuName = "Tower/Skills/NewSkill")]
public class NewSkill : TowerSkill
{
    [SerializeField] private float attackRange = 15f;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 20f;

    public override void Activate(TowerRuntime tower, int slot, int d1, int d2)
    {
        var target = tower.FindClosestEnemy(attackRange);
        if (target == null) return;

        float damage = GetDamageWithDice(d1, d2);
        
        if (projectilePrefab != null)
        {
            var obj = Instantiate(projectilePrefab, tower.transform.position, Quaternion.identity);
            var proj = obj.GetComponent<TowerOfOdds.Combat.Projectile>();
            proj?.Init(target.transform, damage, projectileSpeed, false);
        }
        else
        {
            target.TakeDamage(damage);
        }
    }
}
```

## Dice System

```csharp
// Multiplier calculation
float multiplier = TowerBase.GetDiceMultiplier(d1, d2);
// Formula: (d1 + d2) / 10.0
// Range: 0.2x (snake eyes) to 1.2x (double 6s)

// Usage in skill:
float finalDamage = baseDamage * GetDamageWithDice(d1, d2);
```

## Setup Checklist

**1. Create Skill Asset:**
- Right-click → Create → Tower → Skills → [SkillType]

**2. Configure in Inspector:**
- Base Damage: Your value
- Attack Range: 15 (or custom)
- Projectile Prefab: Drag projectile
- Projectile Speed: 20 (or custom)

**3. Equip to Tower:**
- Select TowerRuntime GameObject
- In Inspector, set Skill Slots size
- Drag skill asset to slot

**4. Test:**
- Play mode
- Spawn enemies
- Check console logs
- Verify projectiles spawn and hit

## Common Patterns

### Direct Damage (No Projectile)
```csharp
target.TakeDamage(damage);
```

### Multi-Target
```csharp
var targets = tower.FindClosestEnemies(3, attackRange);
foreach (var target in targets)
{
    // Shoot each target
}
```

### Conditional Target Selection
```csharp
var allEnemies = tower.GetAllEnemiesInRange(attackRange);
foreach (var enemy in allEnemies)
{
    if (enemy.GetCurrentHealth() < enemy.GetMaxHealth() * 0.3f)
    {
        // Target low HP enemy
    }
}
```

### Stat Modification (OnEquip)
```csharp
public override void OnEquip(TowerRuntime tower, int slot)
{
    var stats = tower.GetStats();
    stats.AttackSpeed += 0.2f;  // Boost attack speed
}
```

## Debug Tips

```csharp
// Check if target found
if (target == null)
{
    Debug.Log($"{skillName}: No target in range");
    return;
}

// Log damage dealt
Debug.Log($"{skillName}: Dealt {damage:F1} dmg to {target.name} (dice: {d1}+{d2})");

// Check projectile spawn
if (projectilePrefab == null)
{
    Debug.LogWarning($"{skillName}: No projectile prefab assigned!");
}
```

## File Locations

**Skills:**
- `Assets/Game/Tower/Skills/` - Skill scripts
- `Assets/Game/Tower/Skills/*.asset` - Skill ScriptableObjects (create via menu)

**Tower:**
- `Assets/Game/Tower/TowerRuntime.cs` - Main tower with enemy finding
- `Assets/Game/Tower/TowerBase.cs` - Stats and helpers

**Enemy System:**
- `Assets/Scripts/Enemies/BaseEnemy.cs` - Enemy base class
- `Assets/Scriptables/Enemies/*.asset` - Enemy data

**Combat:**
- `Assets/Scripts/Combat/Projectile.cs` - Projectile system
