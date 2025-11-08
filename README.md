# Tower of Odds - Gimersia Edition

**2D** Tower Defense game dengan **skill-based combat**, dice system, wave system yang auto-scaling dan 3 tipe enemy (Melee, Ranged, Tank).

---

## 🎮 Features

- **🎲 Skill-Based Combat**: Tower tidak punya basic attack, semua damage dari skills!
- **🎯 Dice Roll System**: Setiap attack roll 2d6 untuk damage multiplier (0.2x - 1.2x)
- **⚔️ Multiple Skills**: Equip hingga 3 skills sekaligus, semua activate per attack cycle
- **Auto-Scaling Wave System**: Difficulty meningkat otomatis berdasarkan wave
- **3 Enemy Types**: Masing-masing dengan behavior unik
  - 🗡️ **Melee**: Fast & aggressive
  - 🏹 **Ranged**: Kiting & distance control
  - 🛡️ **Tank**: Slow & tanky
- **Smart Enemy Targeting**: Auto-targeting closest enemy dengan dynamic retargeting
- **Pure Survival Mode**: No upgrades, pure skill-based gameplay
- **2D Gameplay**: Top-down atau side-view perspective
- **Balanced Gameplay**: Stats yang sudah di-balance untuk 20+ waves
- **📊 Data-Driven Design**: Enemy stats menggunakan ScriptableObject (edit langsung di Inspector!)
- **🎯 Projectile System**: Tower skills dan Ranged enemy menembakkan bullet dengan travel time
- **⭕ Circle Spawn**: Enemies spawn di tepi lingkaran dan bergerak ke tower

---

## 🆕 NEW: Skill System

**Tower sekarang menggunakan sistem skill!** Tidak ada basic attack lagi.

### Example Skills:
- **DiceSkill**: Fires dice projectile (1-6 pips), hastens next attack based on roll
- **MultiShotSkill**: Attack 3 closest enemies simultaneously

### Cara Equip Skills:
1. Create skill asset via: Create → Tower → Skills
2. Configure skill properties di Inspector (damage, range, projectile)
3. Drag skill ke TowerRuntime's Skill Slots (max 3 skills)
4. All equipped skills activate setiap attack cycle!

**� Read:** `INTEGRATION_GUIDE.md` untuk setup lengkap & `SKILL_QUICK_REFERENCE.md` untuk quick reference.

---

## �📁 File Structure

```
Assets/
├── Game/                        # 🆕 NEW: Skill-based tower system
│   └── Tower/
│       ├── TowerRuntime.cs      # Main tower with skill support
│       ├── TowerBase.cs         # Stats & dice helpers
│       └── Skills/
│           ├── TowerSkill.cs       # Base class untuk skills
│           ├── BasicAttackSkill.cs # Single target attack
│           ├── DiceSkill.cs        # Dice variance skill
│           └── MultiShotSkill.cs   # Multi-target skill
├── Scriptables/
│   ├── EnemyData.cs             # ScriptableObject untuk enemy stats
│   └── Enemies/
│       ├── MeleeEnemyData.asset # Melee stats (editable!)
│       ├── RangedEnemyData.asset# Ranged stats (editable!)
│       └── TankEnemyData.asset  # Tank stats (editable!)
├── Scripts/
│   ├── Combat/
│   │   └── Projectile.cs        # Bullet travel & damage (supports both tower systems)
│   ├── Enemies/
│   │   ├── BaseEnemy.cs         # Base class (uses EnemyData)
│   │   ├── MeleeEnemy.cs        # Fast attacker
│   │   ├── RangedEnemy.cs       # Distance fighter
│   │   └── TankEnemy.cs         # Heavy hitter
│   ├── Tower/
│   │   └── Tower.cs             # 🔄 OLD tower system (legacy)
│   └── Manager/
│       ├── GameManager.cs       # Game state management
│       ├── WaveManager.cs       # Wave scaling system
│       └── EnemySpawner.cs      # Circle edge spawning
└── GameUI.cs                    # UI controller
```

---

## 📚 Documentation

### 🆕 NEW: Skill System Guides
- **[INTEGRATION_GUIDE.md](INTEGRATION_GUIDE.md)** - 🔗 **Integration TowerRuntime + Enemy System** (READ THIS FIRST!)
- **[SKILL_QUICK_REFERENCE.md](SKILL_QUICK_REFERENCE.md)** - ⚡ Quick reference untuk create skills

### Enemy & Data System
- **[SETUP_SCRIPTABLEOBJECT.md](SETUP_SCRIPTABLEOBJECT.md)** - 🛠️ Step-by-step setup ScriptableObject
- **[VISUAL_SETUP_GUIDE.md](VISUAL_SETUP_GUIDE.md)** - 🎨 Visual diagrams & flow setup
- **[ENEMY_DATA_GUIDE.md](ENEMY_DATA_GUIDE.md)** - 📊 Cara pakai EnemyData system
- **[BALANCING_REFERENCE.md](BALANCING_REFERENCE.md)** - ⚖️ Stats reference & balancing
- **[2D_SETUP_GUIDE.md](2D_SETUP_GUIDE.md)** - 🎮 2D specific setup guide

---

## 🚀 Quick Setup (NEW Skill System)

**⚠️ IMPORTANT:** Baca **[INTEGRATION_GUIDE.md](INTEGRATION_GUIDE.md)** untuk setup lengkap skill system!

### 1. Scene Setup

1. **Create Tower (NEW System)**
   - Create Empty GameObject, name: "Tower"
   - Set Tag: "Tower"
   - Add Component: `TowerRuntime.cs` (bukan Tower.cs lama!)
   - Position: (0, 0, 0) atau di center map
   - Configure stats di Inspector:
     - Max HP: 100
     - Armor: 5
     - HP Regen Per Sec: 1
     - Attack Speed: 1.0
     - Attack Cooldown: 0.5

2. **Create Skill Assets**
   - Right-click in Project → Create → Tower → Skills
   - Create DiceSkill (dice variance + tempo)
   - Create MultiShotSkill (multi-target) - optional
   - Configure each skill:
     - Base Damage
     - Attack Range (default: 15)
     - Projectile Prefab (drag existing projectile)
     - Projectile Speed (default: 20)

3. **Equip Skills to Tower**
   - Select TowerRuntime GameObject
   - In Inspector, set Skill Slots size = 3
   - Drag skill assets ke slots:
     - Slot 0: DiceSkill
     - Slot 1: MultiShotSkill (optional)
     - Slot 2: (empty or custom skill)

4. **Create Managers**
   - Create Empty GameObject, name: "GameManager"
   - Add Component: `GameManager.cs`
   
   - Create Empty GameObject, name: "WaveManager"
   - Add Component: `WaveManager.cs`
   - Add Component: `EnemySpawner.cs`

5. **Create Enemy Prefabs & Assign** (sama seperti sebelumnya)
   - Follow enemy creation dari dokumentasi lama
   - OR baca **[SETUP_SCRIPTABLEOBJECT.md](SETUP_SCRIPTABLEOBJECT.md)** untuk detail

6. **Press Play!** 🎉
   - Watch tower auto-attack via skills
   - Check console untuk damage logs
   - Observe dice rolls affecting damage

---

## 🎯 Testing Checklist

After setup, test:
- ✅ Wave 1 starts automatically
- ✅ Enemies spawn and move toward tower
- ✅ Tower attacks closest enemy
- ✅ Enemies attack tower when in range
- ✅ Currency earned on enemy kill
- ✅ Wave completes and next wave starts
- ✅ Game over when tower HP = 0

---

## 💡 Optional: Add UI

1. Create Canvas (UI -> Canvas)
2. Add Empty GameObject "GameUI"
3. Add Component: `GameUI.cs`
4. Create UI elements:
   - TextMeshPro for tower health
   - TextMeshPro for wave number
   - TextMeshPro for currency
   - Slider for health bar
5. Assign references di Inspector

---

## ⚙️ Configuration

### Tower Stats (default)
```csharp
Max Health: 1000
Attack Damage: 25 (2 shots Melee/Ranged, 7 shots Tank)
Attack Range: 15
Attack Cooldown: 0.5s
```

**Note:** Currency & upgrade system disabled. Pure survival mode!

### Enemy Stats (Updated for 2D)
See `BALANCING_REFERENCE.md` untuk detail lengkap balancing.

### Wave Scaling
```csharp
Base Duration: 20s → Max 90s (+2s per wave)
Base Spawn Rate: 0.5/s (+0.05/s per wave)
Enemy Distribution: Dynamic berdasarkan wave number
```

---

## 🔧 Customization

### Modify Tower Stats
Edit di `Tower.cs`:
```csharp
[SerializeField] private float maxHealth = 1000f;
[SerializeField] private float attackDamage = 20f;
[SerializeField] private float attackRange = 15f;
[SerializeField] private float attackCooldown = 0.5f;
```

### Modify Enemy Stats
Edit di `MeleeEnemy.cs`, `RangedEnemy.cs`, `TankEnemy.cs`:
```csharp
maxHealth = 50f;
moveSpeed = 4f;
attackDamage = 15f;
attackRange = 2f;
attackCooldown = 1f;
```

### Modify Wave Scaling
Edit di `WaveManager.cs`:
```csharp
[SerializeField] private float baseDuration = 20f;
[SerializeField] private float durationIncrement = 2f;
[SerializeField] private float maxDuration = 90f;
[SerializeField] private float baseEnemiesPerSecond = 0.5f;
[SerializeField] private float enemySpawnScaling = 0.05f;
```

---

## 📊 Balance Overview (2D Version)

| Enemy Type | HP  | Speed | Damage | DPS  | Shots to Kill |
|------------|-----|-------|--------|------|---------------|
| Melee      | 50  | 4.0   | 15     | 15.0 | 2 shots       |
| Ranged     | 50  | 2.5   | 10     | 6.7  | 2 shots       |
| Tank       | 175 | 1.5   | 25     | 12.5 | 7-8 shots     |

**Tower:** HP: 1000 | DMG: 25 | DPS: 50

---

## 🐛 Troubleshooting

**Enemies not spawning?**
- Check if prefabs assigned di EnemySpawner
- Check console for errors

**Tower not attacking?**
- Make sure Tower has tag "Tower"
- Check if enemies in range (15 units default)

**Game Over not triggering?**
- Make sure GameManager assigned di scene
- Check Tower.cs calls gameManager.OnTowerDestroyed()

**No currency earned?**
- Currency system disabled in this version (survival mode only)

---

## 📚 Documentation

- `GAME_DESIGN_DOCUMENT.md` - Full balancing & design details
- Code comments - Inline documentation in scripts

---

## 🎓 Learning Resources

Scripts menggunakan:
- **Inheritance**: BaseEnemy → MeleeEnemy/RangedEnemy/TankEnemy
- **Events**: Wave start/complete callbacks
- **Singleton**: GameManager pattern
- **Coroutines**: Enemy spawning
- **LINQ**: Enemy targeting/filtering

---

## 🚀 Future Improvements

- [ ] Multiple tower types
- [ ] Tower placement system
- [ ] Boss waves (every 5 waves)
- [ ] Particle effects
- [ ] Sound effects
- [ ] Save/Load system
- [ ] Prestige/Reset mechanics
- [ ] Power-ups & abilities
- [ ] Better UI/UX
- [ ] Mobile controls

---

## 📝 Credits

**Developer**: Gimersia Team
**Engine**: Unity
**Type**: Idle Tower Defense
**Version**: 1.0

---

## 📄 License

Free to use for learning purposes.

---

**Happy Gaming! 🎮**

