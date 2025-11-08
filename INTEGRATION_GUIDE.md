# Integration Guide: TowerRuntime with Old Enemy System

## Overview
Sistem tower baru (TowerRuntime) telah berhasil di-integrasikan dengan sistem enemy lama. Tower sekarang menggunakan **skill-based combat** tanpa basic attack.

## Perubahan Utama

### 1. TowerRuntime - Enemy Targeting System
TowerRuntime sekarang memiliki 3 helper methods untuk menemukan enemy:

```csharp
// Find single closest enemy
TowerOfOdds.Enemies.BaseEnemy enemy = tower.FindClosestEnemy(range);

// Find multiple closest enemies (sorted by distance)
TowerOfOdds.Enemies.BaseEnemy[] enemies = tower.FindClosestEnemies(count, range);

// Get ALL enemies in range
TowerOfOdds.Enemies.BaseEnemy[] allEnemies = tower.GetAllEnemiesInRange(range);
```

**Location:** `Assets/Game/Tower/TowerRuntime.cs` (lines added after DoAttack method)

### 2. Projectile - Dual Tower Support
Projectile sekarang mendukung **KEDUA** sistem tower (old & new):

```csharp
// Priority: TowerRuntime first, then old Tower
if (targetIsTower)
{
    TowerRuntime newTower = target.GetComponent<TowerRuntime>();
    if (newTower != null)
        newTower.ApplyDamage(damage);  // New system
    else
        // Fallback to old tower via reflection
}
```

**Location:** `Assets/Scripts/Combat/Projectile.cs` (Update method)

### 3. Skills - Enemy Targeting Examples
Created example skills with complete enemy targeting:

#### A. DiceSkill
- **Purpose:** Dice-based attack with tempo scaling
- **Behavior:** 
  - Fires one dice projectile at nearest enemy
  - On impact: deal randomized damage 1-6 (visual pip)
  - Briefly hastens tower's next attack (speed-up buff)
- **Formulas:**
  - Damage = BaseDamage × M × R, where:
    - M = Tower dice multiplier from 2d6 roll
    - R ∈ {1..6} chosen per shot
  - Speed-up buff: Reduce AttackSpeed cooldown by (0.03 × R) for 0.75s
  - Cooldown clamped to minimum 0.15s
- **Scales with:** AttackSpeed (tempo)
- **File:** `Assets/Game/Tower/Skills/DiceSkill.cs`

#### B. MultiShotSkill
- **Purpose:** Multi-target attack
- **Behavior:** Shoots at multiple closest enemies simultaneously
- **Features:**
  - Uses `tower.FindClosestEnemies(numberOfShots, range)`
  - Can split damage or deal full damage per target
  - Configurable shot count (default 3)
- **File:** `Assets/Game/Tower/Skills/MultiShotSkill.cs`

## Setup Instructions

### Step 1: Create Skill Assets
1. Right-click in Project window → Create → Tower → Skills
2. Create skills:
   - DiceSkill (dice variance + tempo boost)
   - MultiShotSkill (multi-target)
   - (Additional custom skills as needed)

### Step 2: Configure Skill Properties
For each skill, set in Inspector:

**DiceSkill:**
- Base Damage: 10 (will be multiplied by M × R)
- Speed Up Per Roll: 0.03 (cooldown reduction per pip)
- Buff Duration: 0.75 (seconds)
- Attack Range: 15
- Projectile Prefab: (use or create dice visual)
- Projectile Speed: 25

**MultiShotSkill:**
- Base Damage: 20
- Number Of Shots: 3
- Attack Range: 15
- Split Damage: false (for full damage per target) atau true (split total damage)
- Projectile Prefab: (same as basic or unique)
- Projectile Speed: 20

### Step 3: Setup TowerRuntime GameObject
1. Create new GameObject untuk tower atau update existing
2. Add component: `TowerRuntime`
3. Configure stats:
   - Max HP: 100
   - Armor: 5
   - HP Regen Per Sec: 1
   - Base Attack Count: 1
   - Attack Speed: 1.0
   - Attack Cooldown: 0.5

### Step 4: Equip Skills
Di Inspector TowerRuntime:
1. Set `Skill Slots` size = 3
2. Drag skill assets ke slots:
   - Slot 0: DiceSkill
   - Slot 1: MultiShotSkill (optional)
   - Slot 2: (empty or additional skill)

### Step 5: Test
1. Play mode
2. Spawn enemies via EnemySpawner
3. Observe:
   - Tower menemukan enemy terdekat
   - Skills activate setiap attack cycle
   - Projectiles spawn dan hit enemies
   - Enemies take damage dan die

## How Skills Work

### Activation Flow
```
1. TowerRuntime.DoAttack() triggers every attackCooldown seconds
2. Rolls 2d6 (d1, d2) for dice system
3. For each equipped skill:
   a. Calls skill.Activate(tower, slotIndex, d1, d2)
   b. Skill uses tower.FindClosestEnemy() to get target
   c. Skill calculates damage using GetDamageWithDice(d1, d2)
   d. Skill spawns projectile or applies direct damage
4. Projectile travels to enemy
5. On arrival, calls enemy.TakeDamage(damage)
```

### Dice Multiplier System
From `TowerBase.GetDiceMultiplier(d1, d2)`:
```
Multiplier = (d1 + d2) / 10.0

Examples:
- Snake eyes (1+1): 0.2x damage
- Average (3+4):   0.7x damage  
- Double 6s (6+6): 1.2x damage
```

Skills use: `float damage = baseDamage * GetDiceMultiplier(d1, d2)`

## Integration Points

### TowerRuntime → Enemy System
- **Method:** `FindClosestEnemy(range)` returns `TowerOfOdds.Enemies.BaseEnemy`
- **Usage:** `var enemy = tower.FindClosestEnemy(15f);`
- **Properties available:**
  - `enemy.transform` - for projectile targeting
  - `enemy.TakeDamage(float)` - apply damage
  - `enemy.IsAlive` - check if still alive
  - `enemy.GetPosition()` - current position

### Skills → Projectile
- **Init signature:** `projectile.Init(Transform target, float damage, float speed, bool isTargetTower)`
- **For shooting enemies:** `projectile.Init(enemy.transform, damage, speed, false)`
- **Auto-handles:** Travel, collision, damage application, destroy

### Projectile → TowerRuntime
- **Dual support:** Automatically detects TowerRuntime or old Tower
- **Priority:** Tries TowerRuntime first, then fallback
- **Damage method:** `TowerRuntime.ApplyDamage(damage)` (includes armor calculation)

## Compatibility Notes

### Old System Still Works
Jika masih ada old Tower.cs di scene:
- Projectiles tetap work via reflection fallback
- Enemies tetap bisa damage old tower
- Kedua sistem bisa coexist (testing/transition)

### Migration Path
Untuk full migration ke skill system:
1. Replace old Tower GameObject dengan TowerRuntime
2. Equip BasicAttackSkill untuk maintain basic attack behavior
3. Add DiceSkill/MultiShotSkill untuk extra features
4. (Optional) Remove old Tower.cs script setelah testing

## Troubleshooting

### "No target in range" di console
- **Cause:** Tidak ada enemy dalam attackRange
- **Fix:** Check enemy spawner working, increase attackRange

### Projectile tidak spawn
- **Cause:** Projectile Prefab belum di-set di skill Inspector
- **Fix:** Drag projectile prefab ke skill's Projectile Prefab field
- **Alternative:** Skill akan fallback ke direct damage

### Projectile tidak damage enemy
- **Cause:** Projectile prefab tidak punya Projectile component
- **Fix:** Ensure prefab has `TowerOfOdds.Combat.Projectile` script attached

### Skills tidak activate
- **Cause:** Skill slots kosong atau cooldown masih running
- **Fix:** 
  - Check TowerRuntime Inspector, equip skills ke slots
  - Check attackCooldown value (default 0.5s)
  - Check console untuk "DoAttack" debug logs

### Damage terlalu kecil
- **Cause:** Dice multiplier dapat snake eyes (1+1 = 0.2x)
- **Expected:** Variance system, kadang high kadang low
- **Fix:** Increase baseDamage di skill atau adjust balancing

## Example Skill Creation Template

```csharp
using UnityEngine;

[CreateAssetMenu(fileName = "YourSkill", menuName = "Tower/Skills/YourSkill")]
public class YourSkill : TowerSkill
{
    [SerializeField] private float attackRange = 15f;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 20f;

    public override void Activate(TowerRuntime tower, int slotIndex, int d1, int d2)
    {
        // 1. Find target
        var target = tower.FindClosestEnemy(attackRange);
        if (target == null) return;

        // 2. Calculate damage
        float damage = GetDamageWithDice(d1, d2);
        
        // 3. Apply effect (your custom logic here)
        
        // 4. Spawn projectile
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

## Testing Checklist

- [ ] TowerRuntime can find enemies (check console logs)
- [ ] Skills activate every attackCooldown seconds
- [ ] Projectiles spawn at tower position
- [ ] Projectiles travel to enemies
- [ ] Enemies take damage and die
- [ ] Dice multiplier affects damage variance
- [ ] Multiple skills can be equipped and all activate
- [ ] MultiShotSkill hits multiple targets
- [ ] No compile errors
- [ ] No runtime errors in console

## Next Steps

1. **Balancing:**
   - Adjust baseDamage values di skills
   - Tune attackRange per skill type
   - Balance dice multiplier impact

2. **Visual Polish:**
   - Create unique projectile prefabs untuk tiap skill
   - Add VFX on skill activation
   - Add impact effects on hit

3. **More Skills:**
   - AOE damage skill
   - Slow/debuff skill
   - Healing/shield skill
   - Chain lightning skill
   - etc.

4. **UI Integration:**
   - Show equipped skills di UI
   - Display cooldowns
   - Show dice rolls visually

---
**Integration completed:** ✅  
**Files modified:** 4  
**New files created:** 3  
**Ready for testing:** Yes
