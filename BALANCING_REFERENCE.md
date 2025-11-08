# QUICK BALANCING REFERENCE

## 📊 Stats Cheat Sheet

### Tower
```
HP: 1000 | DMG: 25 | Range: 15 | CD: 0.5s | DPS: 50
```

### Enemies
```
Melee:  HP: 50  | Speed: 4.0 | DMG: 15 | CD: 1.0s | DPS: 15.0 | Shots to Kill: 2
Ranged: HP: 50  | Speed: 2.5 | DMG: 10 | CD: 1.5s | DPS: 6.7  | Shots to Kill: 2
Tank:   HP: 175 | Speed: 1.5 | DMG: 25 | CD: 2.0s | DPS: 12.5 | Shots to Kill: 7
        (+ 20% damage reduction = effective 8 shots)
```

**Note:** Currency & Upgrade system disabled for now.

---

## 🌊 Wave Progression

| Wave | Duration | Spawn/s | Total | Melee% | Ranged% | Tank% |
|------|----------|---------|-------|--------|---------|-------|
| 1    | 20s      | 0.5     | 10    | 70%    | 25%     | 5%    |
| 5    | 28s      | 0.7     | 20    | 60%    | 32.5%   | 7.5%  |
| 10   | 38s      | 0.95    | 36    | 42%    | 39%     | 19%   |
| 15   | 48s      | 1.2     | 58    | 40%    | 40%     | 20%   |
| 20   | 58s      | 1.45    | 84    | 32.5%  | 35%     | 32.5% |
| 30   | 78s      | 1.95    | 152   | 27.5%  | 35%     | 37.5% |
| 35+  | 88-90s   | 2.2+    | 180+  | 25%    | 35%     | 40%   |

---

## ⚔️ Combat Math

### Time to Kill (Tower attacking Enemy)
```
Melee:  1.0 seconds (2 shots)
Ranged: 1.0 seconds (2 shots)
Tank:   3.5 seconds (7 shots base, 8 shots with damage reduction)
```

### Time to Kill (Enemy attacking Tower - solo)
```
Melee:  66.7 seconds
Ranged: 100.0 seconds
Tank:   40.0 seconds (highest threat!)
```

### Effective DPS vs Tower
```
Wave 1 (10 enemies):
- 7 Melee × 15 DPS = 105 DPS max
- 2 Ranged × 6.7 DPS = 13.4 DPS max
- 1 Tank × 12.5 DPS = 12.5 DPS max
Total: ~130 DPS (if all attacking simultaneously)

Tower can handle ~3-4 enemies attacking at once safely
```

---

## 💰 Economy System - DISABLED

Currency and upgrade system has been disabled.
Focus is on pure tower defense survival gameplay.

---

## 🎯 Suggested Upgrade Costs - DISABLED

Upgrade system not implemented yet.

---

## 🔧 Balancing Tips

### Too Easy?
1. Increase enemy spawn rate: `enemySpawnScaling = 0.1` (default: 0.05)
2. Increase tank percentage earlier
3. Reduce tower base damage: `attackDamage = 20` (default: 25)
4. Increase enemy HP by 25%

### Too Hard?
1. Increase tower HP: `maxHealth = 1500` (default: 1000)
2. Increase tower damage: `attackDamage = 30` (default: 25)
3. Reduce enemy spawn rate: `enemySpawnScaling = 0.03` (default: 0.05)
4. Reduce tank percentage

### Better Early Game
1. Reduce enemy damage
2. Slower spawn rate early: `baseEnemiesPerSecond = 0.3`
3. Longer break between waves: `timeBetweenWaves = 15f`
4. Less tanks in early waves

### Better Late Game
1. Higher spawn rate scaling
2. More tanks in late waves
3. Boss waves every 5 waves
4. Special enemy abilities

---

## 📈 Difficulty Curve

### Target Survival (no upgrades - survival mode)
```
Wave 1-5:   Easy (should always win)
Wave 6-10:  Medium (70-90% win rate)
Wave 11-15: Hard (40-60% win rate)
Wave 16-20: Very Hard (20-40% win rate)
Wave 21+:   Extreme (requires perfect play)
```

---

## 🎮 Testing Checklist

Test these scenarios:
- [ ] Can survive Wave 1 with no damage
- [ ] Wave 5 is challenging but winnable
- [ ] Wave 10 requires some strategy
- [ ] Melee enemies don't insta-kill tower
- [ ] Ranged enemies maintain distance correctly
- [ ] Tank enemies absorb damage properly (8 shots)
- [ ] Tower prioritizes closest enemy
- [ ] Enemies killed in correct shots (Melee: 2, Ranged: 2, Tank: 7-8)
- [ ] Death happens gradually, not suddenly

---

## 🐛 Common Balance Issues

### Tower Dies Too Fast
```
Problem: Enemies dealing too much damage
Fix: Reduce enemy attack damage by 20-30%
OR: Increase tower HP by 50%
OR: Reduce spawn rate
```

### Tower Too Strong
```
Problem: Enemies die instantly (less than expected shots)
Fix: Increase enemy HP
OR: Reduce tower damage to 20-22
OR: Increase spawn rate
```

### Economy System
```
Status: DISABLED
No currency or upgrades in current version
```

### Game Too Long
```
Problem: Waves take forever
Fix: Increase spawn rate
OR: Reduce wave duration
OR: Reduce time between waves
```

---

## 📊 Analytics to Track

Monitor these values during playtesting:
- Average wave completion time
- Tower HP at end of each wave
- Currency earned per wave
- Enemies killed per wave
- Time to first death
- Most dangerous enemy type
- Most effective upgrade path

---

**Last Updated:** November 2025
**Version:** 1.0
