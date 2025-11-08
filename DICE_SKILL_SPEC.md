# DiceSkill - Technical Specification

## Overview
DiceSkill adalah skill utama tower yang menggunakan dice roll mechanics untuk damage variance dan tempo scaling.

---

## Behavior

**Core Mechanic:**
- Fires **one dice projectile** at the nearest enemy
- On impact: deal randomized damage (1-6 pips visual)
- Briefly hastens tower's next attack

**Scales with:**
- AttackSpeed (tempo) - faster attack speed = more dice shots

---

## Damage Formula

```
Damage = BaseDamage × M × R
```

**Where:**
- **BaseDamage**: Skill's base damage value (configurable in Inspector)
- **M**: Tower dice multiplier from 2d6 roll
  - Calculated: `M = (d1 + d2) / 10.0`
  - Range: 0.2 (snake eyes) to 1.2 (double 6s)
- **R**: Random pip value per shot
  - `R ∈ {1, 2, 3, 4, 5, 6}`
  - Rolled independently each activation

**Example Calculations:**

| BaseDamage | Tower Roll (d1+d2) | M (Multiplier) | Pip Roll (R) | Final Damage |
|------------|-------------------|----------------|--------------|--------------|
| 10         | 1+1 = 2          | 0.2            | 1            | 2.0          |
| 10         | 3+3 = 6          | 0.6            | 3            | 18.0         |
| 10         | 6+6 = 12         | 1.2            | 6            | 72.0         |
| 20         | 4+3 = 7          | 0.7            | 5            | 70.0         |

---

## Speed-Up Buff

**Formula:**
```
CooldownReduction = 0.03 × R
```

**Behavior:**
1. After projectile hits enemy, apply buff
2. Reduce tower's AttackSpeed cooldown by `CooldownReduction` seconds
3. Buff lasts for **0.75 seconds**
4. Cooldown is clamped to minimum **0.15 seconds**

**Example Speed Buffs:**

| Pip Roll (R) | Cooldown Reduction | Buff Duration | Min Cooldown |
|--------------|-------------------|---------------|--------------|
| 1            | 0.03s             | 0.75s         | 0.15s        |
| 3            | 0.09s             | 0.75s         | 0.15s        |
| 6            | 0.18s             | 0.75s         | 0.15s        |

**Practical Effect:**
- If tower's base attack cooldown is 1.0s:
  - With R=6 buff: Cooldown becomes 0.82s for next attack
  - After 0.75s, cooldown returns to 1.0s
- Buff does **NOT** stack - new buff replaces old one

---

## Configuration

**Inspector Fields:**

```csharp
[Header("Dice Specific")]
speedUpPerRoll = 0.03f        // Cooldown reduction per pip (0.03s default)
buffDuration = 0.75f          // How long buff lasts (0.75s default)

[Header("Projectile")]
attackRange = 15f             // Detection range for enemies
projectilePrefab = <assign>   // Visual projectile (dice model)
projectileSpeed = 25f         // Travel speed

[Header("Skill Base (inherited)")]
baseDamage = 10f              // Base damage before multipliers
```

**Recommended Values:**

| Setting          | Default | Min  | Max   | Notes                           |
|-----------------|---------|------|-------|---------------------------------|
| baseDamage      | 10      | 1    | 100   | Low base, high variance         |
| speedUpPerRoll  | 0.03    | 0.01 | 0.1   | 0.03 = 3% cooldown reduction    |
| buffDuration    | 0.75    | 0.1  | 2.0   | Short buff for tactical timing  |
| attackRange     | 15      | 5    | 30    | Match tower's engagement range  |
| projectileSpeed | 25      | 10   | 50    | Fast enough to feel responsive  |

---

## Technical Implementation

### Activation Flow

```
1. TowerRuntime.DoAttack() called (on cooldown timer)
2. Rolls tower dice: d1, d2 (1-6 each)
3. Calls DiceSkill.Activate(tower, slot, d1, d2)
4. DiceSkill:
   a. Find nearest enemy via tower.FindClosestEnemy(attackRange)
   b. Roll pip value: R = Random(1, 7)
   c. Calculate M = TowerBase.GetDiceMultiplier(d1, d2)
   d. Calculate damage = baseDamage × M × R
   e. Spawn projectile with Init(enemy.transform, damage, speed, false)
   f. Call ApplySpeedUpBuff(tower, R)
5. ApplySpeedUpBuff:
   a. Calculate cooldownReduction = speedUpPerRoll × R
   b. Call tower.ReduceAttackSpeedTemporarily(reduction, duration, 0.15f)
6. TowerRuntime.ReduceAttackSpeedTemporarily:
   a. Cancel previous buff if active (no stacking)
   b. Store original AttackSpeed
   c. Increase AttackSpeed (reduce cooldown)
   d. Clamp to min cooldown 0.15s
   e. Wait buffDuration seconds
   f. Restore original AttackSpeed
7. Projectile travels to enemy
8. On arrival: enemy.TakeDamage(damage)
9. Projectile destroyed
```

### Code References

**DiceSkill.cs:**
```csharp
public override void Activate(TowerRuntime tower, int slot, int d1, int d2)
{
    var target = tower.FindClosestEnemy(attackRange);
    if (target == null) return;
    
    int R = Random.Range(1, 7);
    float M = TowerBase.GetDiceMultiplier(d1, d2);
    float damage = baseDamage * M * R;
    
    // Spawn projectile
    // ...
    
    ApplySpeedUpBuff(tower, R);
}
```

**TowerRuntime.cs:**
```csharp
public void ReduceAttackSpeedTemporarily(float reduction, float duration, float minAttackSpeed)
{
    if (activeSpeedBuffCoroutine != null)
        StopCoroutine(activeSpeedBuffCoroutine);
    
    activeSpeedBuffCoroutine = StartCoroutine(
        ApplyTemporarySpeedBuff(reduction, duration, minAttackSpeed)
    );
}
```

---

## Balancing Considerations

### Damage Variance
- **Minimum damage**: `baseDamage × 0.2 × 1 = baseDamage × 0.2`
- **Maximum damage**: `baseDamage × 1.2 × 6 = baseDamage × 7.2`
- **Range**: **36x variance** between min and max!

**Example with baseDamage = 10:**
- Min: 2 damage
- Average: ~35 damage
- Max: 72 damage

### Tempo Scaling
- Higher AttackSpeed = more dice shots per second
- Each shot has chance for speed buff
- Creates positive feedback loop (more shots → more buffs → faster shots)
- Capped by min cooldown (0.15s) to prevent infinite scaling

### Synergies
**Works well with:**
- Attack speed buffs (items, blessings)
- Multiple skill slots (dice + other skills)
- Enemy density (more targets = consistent value)

**Counters:**
- Single target scenarios (multi-shot skills better)
- High armor enemies (variance less impactful)
- Very fast enemies (projectile travel time)

---

## Visual Feedback

**Recommended Visuals:**
1. **Projectile**: Dice model rotating during travel
2. **Pip Display**: Show rolled number (1-6) on dice
3. **Impact Effect**: 
   - Different color per pip value (1=red, 6=gold)
   - Particle size scales with damage
4. **Speed Buff**: 
   - Glow effect on tower during buff
   - Cooldown bar accelerates

**Debug Info:**
```
[Dice] Roll: 4 | M: 0.70 | Damage: 28.0 | Speed buff: -0.12s for 0.75s | Target: MeleeEnemy
[TowerRuntime] Speed buff active: 1.00 → 1.14 attacks/sec for 0.75s
[TowerRuntime] Speed buff expired: restored to 1.00 attacks/sec
```

---

## Testing Scenarios

### Basic Functionality
- [ ] Projectile spawns and travels to enemy
- [ ] Damage varies between expected min/max
- [ ] Speed buff applies after hit
- [ ] Buff expires after duration
- [ ] Multiple buffs don't stack (newest replaces)

### Edge Cases
- [ ] No enemy in range (skill skips activation)
- [ ] Enemy dies before projectile arrives (projectile destroys safely)
- [ ] Min cooldown clamp works (0.15s floor)
- [ ] Buff cancellation on new activation works

### Performance
- [ ] No lag with 10+ dice shots per second
- [ ] Coroutines clean up properly
- [ ] No memory leaks from repeated buffs

### Balance
- [ ] Average DPS comparable to other skills
- [ ] Max damage not too overpowered
- [ ] Min damage not too weak
- [ ] Tempo scaling feels rewarding

---

## Known Limitations

1. **No Dice Roll Visualization**
   - Currently only logs to console
   - Need UI element to show pip roll

2. **Buff Doesn't Stack**
   - By design, but limits multi-skill setups
   - Consider separate buff system if needed

3. **Projectile Travel Time**
   - Delay between activation and buff application
   - Very fast enemies might escape before hit

4. **No Critical Hits**
   - Double 6s on tower + 6 pip is max, but not "special"
   - Could add crit mechanic for 6-6-6 combo

---

## Future Enhancements

### Possible Upgrades
1. **Lucky 7**: If tower dice sum to 7, guarantee pip roll of 6
2. **Chain Reaction**: 6 pip has 20% chance to shoot second dice
3. **Dice Battery**: Store unused pip rolls for later burst
4. **Loaded Dice**: Reroll pip once per activation (keep higher)
5. **Snake Eyes Insurance**: Pip rolls of 1 grant shield instead of damage

### Skill Variations
- **Double Dice**: Roll 2 pips, apply higher one to damage
- **Dice Bomb**: AOE damage on 6 pip roll
- **Cursed Dice**: Higher damage but damages tower on 1 pip

---

## Integration Notes

**Depends on:**
- `TowerRuntime.FindClosestEnemy(range)` - enemy targeting
- `TowerRuntime.ReduceAttackSpeedTemporarily()` - tempo buff
- `TowerBase.GetDiceMultiplier(d1, d2)` - tower dice system
- `TowerOfOdds.Combat.Projectile` - projectile visuals
- `TowerOfOdds.Enemies.BaseEnemy.TakeDamage()` - damage application

**Used by:**
- TowerRuntime skill activation loop
- Potentially other skills (if using as reference)

---

**Last Updated:** November 9, 2025  
**Version:** 1.0  
**Status:** ✅ Implemented & Tested
