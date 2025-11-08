# Tower of Odds - Game Design Document

## Overview
Game idle tower defense dengan 3 tipe enemy (Melee, Ranged, Tank) dan wave system yang auto-scaling.

---

## 📊 BALANCING STATS

### 🏰 TOWER STATS
```
Max Health: 1000 HP
Attack Damage: 20 damage
Attack Range: 15 units
Attack Cooldown: 0.5 seconds (2 attacks/sec)
DPS: 40 damage/second
```

**Tower Capabilities:**
- Dapat mengupgrade Health, Damage, Attack Speed, dan Range
- Targeting: Prioritas enemy terdekat (closest first)
- Auto-attack semua enemy dalam range

---

### 👾 ENEMY STATS (Balanced)

#### 1. MELEE ENEMY (Glass Cannon - Fast & Aggressive)
```
HP: 50
Move Speed: 4 units/s
Attack Damage: 15
Attack Range: 2 units (sangat dekat)
Attack Cooldown: 1 second
DPS: 15 damage/second
Reward: 10 currency
```
**Behavior:** Rush ke tower secepat mungkin, attack jarak dekat
**Threat Level:** ⭐⭐ (Medium - berbahaya dalam jumlah banyak)

#### 2. RANGED ENEMY (Harasser - Keep Distance)
```
HP: 40
Move Speed: 2.5 units/s
Attack Damage: 10
Attack Range: 10 units
Preferred Distance: 8 units
Attack Cooldown: 1.5 seconds
DPS: 6.67 damage/second
Reward: 15 currency (1.5x)
```
**Behavior:** Maintain jarak 8 units dari tower, attack dari jauh
**Threat Level:** ⭐⭐ (Medium - susah dijangkau tower, damage rendah)

#### 3. TANK ENEMY (Heavy Hitter - Slow & Tanky)
```
HP: 200
Damage Reduction: 20%
Effective HP: 250 (dengan reduction)
Move Speed: 1.5 units/s
Attack Damage: 25
Attack Range: 3 units
Attack Cooldown: 2 seconds
DPS: 12.5 damage/second
Reward: 30 currency (3x)
```
**Behavior:** Maju pelan tapi pasti, absorb damage, hit hard
**Threat Level:** ⭐⭐⭐⭐ (High - butuh banyak shot untuk kill)

---

## 🌊 WAVE SYSTEM

### Wave Scaling Formula

#### Duration (Durasi Wave)
```
Formula: min(20 + (wave - 1) × 2, 90)
Wave 1: 20 seconds
Wave 2: 22 seconds
Wave 5: 28 seconds
Wave 10: 38 seconds
Wave 35: 88 seconds
Wave 36+: 90 seconds (capped)
```

#### Spawn Rate (Enemies per Second)
```
Formula: 0.5 + (wave - 1) × 0.05
Wave 1: 0.5 enemies/sec = 10 enemies total
Wave 2: 0.55 enemies/sec = 12 enemies total
Wave 5: 0.7 enemies/sec = 20 enemies total
Wave 10: 0.95 enemies/sec = 36 enemies total
Wave 20: 1.45 enemies/sec = 87 enemies total
Wave 30: 1.95 enemies/sec = 165 enemies total
```

#### Enemy Distribution (Komposisi)

**Wave 1-5 (Early Game):**
```
Melee: 70% → 60%
Ranged: 25% → 32.5%
Tank: 5% → 7.5%
```
*Focus: Introduce mechanics, mostly melee*

**Wave 6-15 (Mid Game):**
```
Melee: 50% → 40%
Ranged: 35% → 40%
Tank: 15% → 20%
```
*Focus: Balanced mix, increasing difficulty*

**Wave 16+ (Late Game):**
```
Melee: 35% → 25% (capped)
Ranged: 35% (constant)
Tank: 30% → 40% (capped)
```
*Focus: Tank heavy, very challenging*

---

## 🎮 GAMEPLAY MECHANICS

### Tower Targeting Priority
1. Enemy dalam range (≤15 units)
2. Enemy yang masih hidup
3. **PRIORITAS: JARAK TERDEKAT** (closest enemy first)
4. Auto-switch target jika current target mati/keluar range

### Enemy Behavior by Type

**Melee:**
- Straight rush ke tower
- No special positioning
- High threat in groups (swarm)

**Ranged:**
- Kite behavior: maintain 8 units distance
- Back off jika terlalu dekat (< 8 units)
- Advance jika terlalu jauh (> 10 units)
- Creates "safe zone" positioning

**Tank:**
- Slow steady advance
- 20% damage reduction (takes 80% of damage)
- Blocks other enemies (natural wall)

---

## 💰 ECONOMY SYSTEM

### Currency Rewards
```
Melee Kill: 10 currency
Ranged Kill: 15 currency
Tank Kill: 30 currency
Wave Complete Bonus: wave_number × 50 currency

Example Wave 10:
- Kill 20 melee (40%): 200 currency
- Kill 15 ranged (40%): 225 currency
- Kill 7 tank (20%): 210 currency
- Wave bonus: 500 currency
Total: 1,135 currency
```

### Suggested Upgrade Costs (for balancing)
```
Health Upgrade: 100 × level (adds +100 HP)
Damage Upgrade: 150 × level (adds +10 damage)
Attack Speed Upgrade: 200 × level (reduces cooldown by 10%)
Range Upgrade: 250 × level (adds +2 range)
```

---

## 🎯 BALANCE CONSIDERATIONS

### DPS Analysis
```
Tower DPS: 40 (base)
Melee DPS: 15 (single unit)
Ranged DPS: 6.67 (single unit)
Tank DPS: 12.5 (single unit)
```

### Time to Kill (Tower vs Enemy)
```
Tower kills Melee: 2.5 seconds (5 hits)
Tower kills Ranged: 2 seconds (4 hits)
Tower kills Tank: 10 seconds (20 hits, dengan reduction)
```

### Survival Time (Enemy vs Tower)
```
Melee kills Tower: 66.7 seconds (solo)
Ranged kills Tower: 100 seconds (solo)
Tank kills Tower: 40 seconds (solo, high damage)
```

### Wave 1 Example
```
Duration: 20 seconds
Spawn Rate: 0.5/sec
Total Enemies: 10
- 7 Melee (70%)
- 2 Ranged (20%)
- 1 Tank (10%)

Tower can kill ~8 enemies (2.5s avg TTK)
Result: Challenging but winnable
Tower damage taken: ~30-60 HP
```

### Wave 10 Example
```
Duration: 38 seconds
Spawn Rate: 0.95/sec
Total Enemies: 36
- 15 Melee (42%)
- 14 Ranged (39%)
- 7 Tank (19%)

Without upgrades: Tower will likely die
With upgrades: Manageable with smart progression
```

---

## 🔧 SETUP INSTRUCTIONS

### 1. Scene Setup
1. Create empty GameObject "Tower" dengan tag "Tower"
2. Add `Tower.cs` component
3. Create empty GameObject "GameManager"
4. Add `GameManager.cs` component
5. Create empty GameObject "WaveManager"
6. Add `WaveManager.cs` dan `EnemySpawner.cs` components

### 2. Prefab Creation
Buat 3 prefabs untuk enemies:
- MeleeEnemyPrefab (add `MeleeEnemy.cs`)
- RangedEnemyPrefab (add `RangedEnemy.cs`)
- TankEnemyPrefab (add `TankEnemy.cs`)

Assign prefabs ke `EnemySpawner` component di Inspector.

### 3. Tag Setup
- Tower GameObject: Tag = "Tower"
- Enemy GameObjects: Layer = "Enemy" (optional, untuk optimization)

### 4. Testing
1. Press Play
2. Wave 1 akan start otomatis
3. Enemies akan spawn dan attack tower
4. Tower akan auto-target dan attack enemies
5. Check console untuk logs (damage, kills, currency, dll)

---

## 📈 PROGRESSION RECOMMENDATIONS

### Early Game (Wave 1-5)
- Focus: Survive and learn mechanics
- Recommended: Upgrade damage first
- Expected currency: ~500-1000

### Mid Game (Wave 6-15)
- Focus: Balance upgrades
- Recommended: Health + Attack Speed
- Expected currency: ~5000-15000

### Late Game (Wave 16+)
- Focus: Maximize efficiency
- Recommended: All stats high
- Expected currency: ~50000+

---

## 🚀 FUTURE ENHANCEMENTS

1. **Multiple Towers**: Add placement system
2. **Enemy Abilities**: Special attacks, shields, healing
3. **Tower Abilities**: Active skills, AOE attacks
4. **Power-ups**: Temporary buffs
5. **Prestige System**: Reset for permanent bonuses
6. **Boss Waves**: Every 5 waves
7. **Visual Effects**: Particles, animations
8. **Sound Effects**: Combat feedback
9. **UI System**: Health bars, wave counter, currency display
10. **Save System**: Persistent progress

---

## 📝 NOTES

- Balance tested untuk ~30 waves
- Difficulty curve: Gradual dengan spike di wave 16
- Idle aspect: Bisa add auto-currency generation
- Scaling: Exponential setelah wave 20
- Economy: Balanced untuk progression ~1-2 hours gameplay

---

**Version:** 1.0
**Created:** November 2025
**Game Type:** Idle Tower Defense
