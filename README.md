# Tower of Odds - Gimersia Edition

**2D** Tower Defense game dengan wave system yang auto-scaling dan 3 tipe enemy (Melee, Ranged, Tank).

---

## 🎮 Features

- **Auto-Scaling Wave System**: Difficulty meningkat otomatis berdasarkan wave
- **3 Enemy Types**: Masing-masing dengan behavior unik
  - 🗡️ **Melee**: Fast & aggressive
  - 🏹 **Ranged**: Kiting & distance control
  - 🛡️ **Tank**: Slow & tanky
- **Smart Tower AI**: Auto-targeting dengan prioritas jarak terdekat
- **Pure Survival Mode**: No upgrades, pure skill-based gameplay
- **2D Gameplay**: Top-down atau side-view perspective
- **Balanced Gameplay**: Stats yang sudah di-balance untuk 20+ waves
- **📊 Data-Driven Design**: Enemy stats menggunakan ScriptableObject (edit langsung di Inspector!)
- **🎯 Projectile System**: Tower dan Ranged enemy menembakkan bullet dengan travel time
- **⭕ Circle Spawn**: Enemies spawn di tepi lingkaran dan bergerak ke tower

---

## 📁 File Structure

```
Assets/
├── Scriptables/
│   ├── EnemyData.cs             # ScriptableObject untuk enemy stats
│   └── Enemies/
│       ├── MeleeEnemyData.asset # Melee stats (editable!)
│       ├── RangedEnemyData.asset# Ranged stats (editable!)
│       └── TankEnemyData.asset  # Tank stats (editable!)
├── Scripts/
│   ├── Combat/
│   │   └── Projectile.cs        # Bullet travel & damage
│   ├── Enemies/
│   │   ├── BaseEnemy.cs         # Base class (uses EnemyData)
│   │   ├── MeleeEnemy.cs        # Fast attacker
│   │   ├── RangedEnemy.cs       # Distance fighter
│   │   └── TankEnemy.cs         # Heavy hitter
│   ├── Tower/
│   │   └── Tower.cs             # Tower dengan projectile system
│   └── Manager/
│       ├── GameManager.cs       # Game state management
│       ├── WaveManager.cs       # Wave scaling system
│       └── EnemySpawner.cs      # Circle edge spawning
└── GameUI.cs                    # UI controller
```

---

## � Documentation

- **[SETUP_SCRIPTABLEOBJECT.md](SETUP_SCRIPTABLEOBJECT.md)** - 🛠️ **BACA INI DULU!** Step-by-step setup ScriptableObject
- **[VISUAL_SETUP_GUIDE.md](VISUAL_SETUP_GUIDE.md)** - 🎨 Visual diagrams & flow setup
- **[ENEMY_DATA_GUIDE.md](ENEMY_DATA_GUIDE.md)** - 📊 Cara pakai EnemyData system
- **[BALANCING_REFERENCE.md](BALANCING_REFERENCE.md)** - ⚖️ Stats reference & balancing
- **[2D_SETUP_GUIDE.md](2D_SETUP_GUIDE.md)** - 🎮 2D specific setup guide

---

## �🚀 Quick Setup

**⚠️ IMPORTANT:** Baca **[SETUP_SCRIPTABLEOBJECT.md](SETUP_SCRIPTABLEOBJECT.md)** terlebih dahulu untuk setup EnemyData assets!

### 1. Scene Setup

1. **Create Tower**
   - Create Empty GameObject, name: "Tower"
   - Set Tag: "Tower"
   - Add Component: `Tower.cs`
   - Position: (0, 0, 0) atau di center map

2. **Create Managers**
   - Create Empty GameObject, name: "GameManager"
   - Add Component: `GameManager.cs`
   
   - Create Empty GameObject, name: "WaveManager"
   - Add Component: `WaveManager.cs`
   - Add Component: `EnemySpawner.cs`

3. **Create Enemy Prefabs (2D)**
   
   **Melee Enemy Prefab:**
   - Create 2D Object (Sprite), name: "MeleeEnemy"
   - Add Component: `SpriteRenderer` (Color: Red)
   - Add Component: `MeleeEnemy.cs`
   - Scale: (0.5, 0.5, 1) untuk 2D
   - Add `CircleCollider2D` atau `BoxCollider2D` (optional)
   - Drag ke Prefabs folder
   
   **Ranged Enemy Prefab:**
   - Create 2D Object (Sprite), name: "RangedEnemy"
   - Add Component: `SpriteRenderer` (Color: Blue)
   - Add Component: `RangedEnemy.cs`
   - Scale: (0.4, 0.4, 1)
   - Add `CircleCollider2D` atau `BoxCollider2D` (optional)
   - Drag ke Prefabs folder
   
   **Tank Enemy Prefab:**
   - Create 2D Object (Sprite), name: "TankEnemy"
   - Add Component: `SpriteRenderer` (Color: Gray)
   - Add Component: `TankEnemy.cs`
   - Scale: (0.8, 0.8, 1)
   - Add `CircleCollider2D` atau `BoxCollider2D` (optional)
   - Drag ke Prefabs folder
   
   **Tower (2D):**
   - Create 2D Object (Sprite), name: "Tower"
   - Add Component: `SpriteRenderer` (Color: Green)
   - Set Tag: "Tower"
   - Add Component: `Tower.cs`
   - Position: (0, 0, 0)
   - Scale: (1, 1, 1)

4. **Assign Prefabs**
   - Select "WaveManager" GameObject
   - Di Inspector, pada `EnemySpawner` component:
     - Assign "MeleeEnemy" ke `Melee Prefab`
     - Assign "RangedEnemy" ke `Ranged Prefab`
     - Assign "TankEnemy" ke `Tank Prefab`

5. **Press Play!** 🎉

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

