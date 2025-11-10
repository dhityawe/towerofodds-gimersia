# 🎮 System Architecture & Flow Diagram

## 📐 System Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                         GAME MANAGER                         │
│  - Singleton pattern                                         │
│  - Game state management                                     │
│  - Currency/Economy system                                   │
│  - Tower upgrade interface                                   │
└──────────────┬─────────────────────────────┬────────────────┘
               │                             │
               │                             │
       ┌───────▼─────────┐          ┌────────▼────────┐
       │  WAVE MANAGER   │          │     TOWER       │
       │  - Wave scaling │          │  - HP system    │
       │  - Progression  │◄─────────┤  - Auto-attack  │
       │  - Events       │          │  - Targeting    │
       └───────┬─────────┘          │  - Upgrades     │
               │                    └────────┬────────┘
               │                             │
       ┌───────▼─────────┐                  │
       │ ENEMY SPAWNER   │                  │ targets
       │  - Spawn logic  │                  │
       │  - Prefabs      │                  │
       └───────┬─────────┘                  │
               │ spawns                     │
               │                            │
       ┌───────▼─────────┐                  │
       │  BASE ENEMY     │◄─────────────────┘
       │  - AI behavior  │  attacks
       │  - Movement     │
       │  - Attack       │
       └───────┬─────────┘
               │
       ┌───────┴──────────┬───────────────┐
       │                  │               │
┌──────▼──────┐  ┌────────▼──────┐  ┌────▼──────┐
│MELEE ENEMY  │  │ RANGED ENEMY  │  │TANK ENEMY │
│- Rush       │  │ - Kiting      │  │- Tank     │
│- Fast       │  │ - Distance    │  │- Slow     │
│- Glass      │  │ - Support     │  │- Heavy    │
└─────────────┘  └───────────────┘  └───────────┘
```

---

## 🔄 Gameplay Flow

```
START
  │
  ▼
┌─────────────────┐
│ Game Initialize │
│ - Find Tower    │
│ - Setup Managers│
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│  Wave 1 Starts  │ ◄──────────────────────┐
│ - Calculate     │                        │
│   wave data     │                        │
└────────┬────────┘                        │
         │                                 │
         ▼                                 │
┌─────────────────┐                        │
│  Enemy Spawner  │                        │
│  - Spawn rate   │                        │
│  - Distribution │                        │
└────────┬────────┘                        │
         │                                 │
         │ (spawns multiple enemies)       │
         ▼                                 │
┌─────────────────┐     ┌──────────────┐  │
│  Enemies Move   │────►│ Tower Finds  │  │
│  - Toward tower │     │ Closest      │  │
│  - Per behavior │     │ Enemy        │  │
└────────┬────────┘     └──────┬───────┘  │
         │                     │          │
         │                     ▼          │
         │            ┌──────────────┐    │
         │            │ Tower Attacks│    │
         │            │ - Deal damage│    │
         │            └──────┬───────┘    │
         │                   │            │
         ▼                   ▼            │
┌─────────────────┐  ┌──────────────┐    │
│ Enemies Attack  │  │ Enemy Dies?  │    │
│ - When in range │  │ - Yes: Reward│    │
│ - Deal damage   │  │ - No: Continue    │
└────────┬────────┘  └──────┬───────┘    │
         │                  │            │
         ▼                  │            │
┌─────────────────┐         │            │
│ Tower Dies?     │         │            │
│ - Yes: GAME OVER│         │            │
│ - No: Continue  │         │            │
└────────┬────────┘         │            │
         │                  │            │
         │   ┌──────────────┘            │
         │   │                           │
         ▼   ▼                           │
┌─────────────────┐                      │
│  Wave Complete? │                      │
│  - Yes: Bonus   │                      │
│  - Break time   │                      │
└────────┬────────┘                      │
         │                               │
         ▼                               │
┌─────────────────┐                      │
│ Next Wave Start │──────────────────────┘
│ - Wave + 1      │
│ - Harder stats  │
└─────────────────┘
```

---

## ⚔️ Combat Flow (Tower vs Enemy)

```
TOWER PERSPECTIVE:
┌──────────────┐
│ Update Loop  │
└──────┬───────┘
       │
       ▼
┌──────────────────┐     NO
│ Has valid target?├─────────┐
└──────┬───────────┘         │
       │ YES                 │
       ▼                     ▼
┌──────────────────┐   ┌────────────────┐
│ Target in range? │   │ Find Closest   │
└──────┬───────────┘   │ Enemy in Range │
       │ YES           └────────┬───────┘
       ▼                        │
┌──────────────────┐            │
│ Cooldown ready?  │◄───────────┘
└──────┬───────────┘
       │ YES
       ▼
┌──────────────────┐
│  Attack Enemy    │
│  - Deal damage   │
│  - Reset CD      │
└──────────────────┘


ENEMY PERSPECTIVE:
┌──────────────┐
│ Update Loop  │
└──────┬───────┘
       │
       ▼
┌──────────────────┐
│ Calculate dist   │
│ to Tower         │
└──────┬───────────┘
       │
       ▼
┌──────────────────┐     NO
│ In attack range? ├─────────┐
└──────┬───────────┘         │
       │ YES                 │
       ▼                     ▼
┌──────────────────┐   ┌────────────────┐
│ Cooldown ready?  │   │ Move Behavior  │
└──────┬───────────┘   │ - Melee: Rush  │
       │ YES           │ - Ranged: Kite │
       ▼               │ - Tank: Steady │
┌──────────────────┐   └────────────────┘
│  Attack Tower    │
│  - Deal damage   │
│  - Reset CD      │
└──────────────────┘
```

---

## 📊 Wave Scaling Diagram

```
WAVE PROGRESSION:

Wave 1          Wave 10         Wave 20         Wave 35+
┌─────┐         ┌─────┐         ┌─────┐         ┌─────┐
│ 20s │         │ 38s │         │ 58s │         │ 90s │
│0.5/s│         │0.95s│         │1.45s│         │2.2/s│
│ 10  │────────►│ 36  │────────►│ 84  │────────►│ 198+│
└─────┘         └─────┘         └─────┘         └─────┘
                                                  (capped)

COMPOSITION SHIFT:

Early (1-5)     Mid (6-15)      Late (16+)
┌─────────┐     ┌─────────┐     ┌─────────┐
│░░░░░░░  │     │░░░▒▒▒   │     │░░▒▒▒▒▒▒ │
│70% Melee│     │40% Melee│     │25% Melee│
│         │     │         │     │         │
│▒▒▒      │     │▒▒▒▒     │     │▒▒▒▒     │
│25%Range │     │40%Range │     │35%Range │
│         │     │         │     │         │
│▓        │     │▓▓       │     │▓▓▓▓     │
│5% Tank  │     │20% Tank │     │40% Tank │
└─────────┘     └─────────┘     └─────────┘

Legend: ░ = Melee, ▒ = Ranged, ▓ = Tank
```

---

## 💰 Economy Flow

```
┌─────────────┐
│ Enemy Dies  │
└──────┬──────┘
       │
       ▼
┌─────────────────────────┐
│ Currency Earned:        │
│ - Melee: 10             │
│ - Ranged: 15 (1.5x)     │
│ - Tank: 30 (3x)         │
└──────┬──────────────────┘
       │
       ▼
┌─────────────────────────┐
│ Add to Player Currency  │
└──────┬──────────────────┘
       │
       │
┌──────▼──────────────────┐
│ Wave Complete           │
│ Bonus: wave_num × 50    │
└──────┬──────────────────┘
       │
       ▼
┌─────────────────────────┐
│ Player Can Upgrade:     │
│ - Health (+100 HP)      │
│ - Damage (+10 DMG)      │
│ - Attack Speed (-10%)   │
│ - Range (+2 units)      │
└─────────────────────────┘
```

---

## 🎯 Targeting Priority System

```
TOWER TARGETING ALGORITHM:

1. Find all enemies
2. Filter: IsAlive = true
3. Filter: Distance ≤ attackRange
4. Sort: By distance (ascending)
5. Select: First element (closest)

Example:
Tower at (0,0,0), Range = 15

Enemies:
┌──────┬──────┬────────┬─────────┬──────────┐
│ Type │ HP   │ Pos    │ Distance│ Priority │
├──────┼──────┼────────┼─────────┼──────────┤
│Melee │ 50   │(5,0,0) │   5     │    1st   │ ◄─ TARGET!
│Ranged│ 40   │(8,0,0) │   8     │    2nd   │
│Tank  │ 200  │(12,0,0)│   12    │    3rd   │
│Melee │ 30   │(20,0,0)│   20    │ Out range│
└──────┴──────┴────────┴─────────┴──────────┘

Tower shoots Melee at distance 5!
```

---

## 🧠 Enemy AI Behaviors

```
MELEE BEHAVIOR:
┌─────────────┐
│ While Alive │
└──────┬──────┘
       │
       ▼
   ┌───────┐ NO
   │In Atk?├────► Move straight to tower
   │Range? │      at full speed (4.0)
   └───┬───┘
       │YES
       ▼
   ┌───────┐ NO
   │CD OK? ├────► Wait
   └───┬───┘
       │YES
       ▼
   Attack tower
   Damage: 15


RANGED BEHAVIOR:
┌─────────────┐
│ While Alive │
└──────┬──────┘
       │
       ▼
┌──────────────┐
│ Dist to Tower│
└──────┬───────┘
       │
       ├── < 8 units ──► Back away (kite)
       │
       ├── 8-10 units ─► Stay in position
       │
       └── > 10 units ─► Move closer
       
       If in attack range (≤10):
       └─► Attack (Damage: 10)


TANK BEHAVIOR:
┌─────────────┐
│ While Alive │
└──────┬──────┘
       │
       ▼
   ┌───────┐ NO
   │In Atk?├────► Move slowly to tower
   │Range? │      at speed 1.5
   └───┬───┘      (tanking shots)
       │YES
       ▼
   ┌───────┐ NO
   │CD OK? ├────► Wait & absorb damage
   └───┬───┘      (20% reduction)
       │YES
       ▼
   Attack tower
   Damage: 25 (heavy!)
```

---

## 📈 Difficulty Curve Graph

```
Difficulty
    │
 10 │                                    ╱─────
    │                              ╱────╯
  8 │                        ╱────╯
    │                  ╱────╯
  6 │            ╱────╯
    │      ╱────╯
  4 │ ╱───╯              Wave 16+: Tank Heavy
    │╯                   (Exponential)
  2 │    Wave 6-15: Mixed
    │    (Steady Growth)
  0 └────┬────┬────┬────┬────┬────┬────────►
      Wave 1-5    10    15   20   25   30  Wave
      (Easy)

Legend:
Wave 1-5:   Tutorial (70% melee)
Wave 6-15:  Challenge (balanced mix)
Wave 16+:   Endgame (40% tank)
```

---

## 🔄 Data Flow

```
Player Actions          System Events           Visual Feedback
─────────────          ──────────────          ────────────────

[Start Game]
     │
     └──────────────►  Initialize
                      GameManager
                      WaveManager
                      Tower
                           │
                           ├─────────────►  UI Updates
                           │               - Health bar
                           │               - Wave number
                           ▼
                      Wave 1 Start
                           │
                           ├─────────────►  Spawn Particles
                           │               - Enemy appears
                           ▼
                      Enemy Spawned
                           │
                           ├─────────────►  Animation
                           │               - Movement
                           ▼
                      Combat Start
                           │
                           ├─────────────►  Effects
                           │               - Attack VFX
                           │               - Damage numbers
                           ▼
                      Enemy Dies
                           │
                           ├─────────────►  Death Effect
                           │               - Particle
                           │               - Sound
                           ├─────────────►  UI Update
                           │               - Currency +10
                           │               - Kills +1
                           ▼
                      Wave Complete
                           │
                           ├─────────────►  UI Update
                           │               - Wave bonus
                           │               - Next wave timer
                           ▼
[Player Upgrades]
     │
     └──────────────►  Spend Currency
                      Upgrade Stats
                           │
                           ├─────────────►  Tower Grows
                           │               - Visual change
                           │               - Stat display
                           ▼
                      Next Wave
                      (Repeat)
```

---

## 🎮 Class Relationships (UML-style)

```
┌──────────────────────────┐
│      GameManager         │ ◄──── Singleton
│  ─────────────────────   │
│  + currency              │
│  + enemiesKilled         │
│  + OnEnemyKilled()       │
│  + OnTowerDestroyed()    │
│  + SpendCurrency()       │
└────────┬─────────────────┘
         │ manages
         │
    ┌────┴────┬────────────┐
    │         │            │
    ▼         ▼            ▼
┌────────┐ ┌──────────┐ ┌────────┐
│WaveMgr │ │  Tower   │ │Spawner │
└───┬────┘ └────┬─────┘ └───┬────┘
    │           │            │
    │spawns     │attacks     │creates
    │           │            │
    └───────────┴────────────┘
                │
                ▼
        ┌───────────────┐
        │  BaseEnemy    │ ◄──── Abstract
        │ ────────────  │
        │ # health      │
        │ # moveSpeed   │
        │ # attackDmg   │
        │ + TakeDamage()│
        │ # Attack()    │ ◄──── Abstract
        └───────┬───────┘
                │ inherits
        ┌───────┴────────┬──────────┐
        ▼                ▼          ▼
    ┌────────┐      ┌────────┐  ┌────────┐
    │ Melee  │      │Ranged  │  │ Tank   │
    │ ────── │      │─────── │  │─────── │
    │+Attack()│      │+Attack()│  │+Attack()│
    │        │      │+Move() │  │+TakeDmg│
    └────────┘      └────────┘  └────────┘
```

---

**Visual Guide Created**
This diagram shows complete system flow and interactions! 🎮
