# 🎨 Visual Setup Guide - ScriptableObject Flow

## 📊 Hierarchy & Setup Flow

```
Unity Project
│
├─ Assets/
│  │
│  ├─ Scriptables/
│  │  ├─ EnemyData.cs ──────────────┐
│  │  └─ Enemies/                   │ (ScriptableObject Class)
│  │     ├─ MeleeEnemyData.asset ◄──┘
│  │     ├─ RangedEnemyData.asset   │ (Data Assets)
│  │     └─ TankEnemyData.asset     │
│  │                                 │
│  ├─ Scripts/                       │
│  │  └─ Enemies/                    │
│  │     ├─ BaseEnemy.cs ◄───────────┘ (Uses EnemyData)
│  │     ├─ MeleeEnemy.cs
│  │     ├─ RangedEnemy.cs
│  │     └─ TankEnemy.cs
│  │
│  └─ Prefabs/
│     ├─ MeleeEnemy ◄───[MeleeEnemyData]
│     ├─ RangedEnemy ◄──[RangedEnemyData]
│     └─ TankEnemy ◄────[TankEnemyData]
│
└─ Hierarchy (Scene)
   └─ WaveManager
      └─ EnemySpawner ◄─┬─[MeleeEnemy Prefab]
                        ├─[RangedEnemy Prefab]
                        └─[TankEnemy Prefab]
```

---

## 🔄 Data Flow

```
Step 1: CREATE ASSETS
┌──────────────────────────────────────────┐
│ Right-click in Unity                     │
│ → Create → Tower of Odds → Enemy Data   │
└──────────────┬───────────────────────────┘
               ▼
     ┌─────────────────────┐
     │ MeleeEnemyData.asset│ ◄─── Edit stats in Inspector
     │ ├─ HP: 50           │
     │ ├─ Speed: 4         │
     │ └─ Damage: 15       │
     └─────────────────────┘

Step 2: CREATE PREFAB
┌──────────────────────────────────────────┐
│ 2D Object → Sprite → Square              │
│ Add Component → MeleeEnemy.cs            │
└──────────────┬───────────────────────────┘
               ▼
     ┌─────────────────────┐
     │  MeleeEnemy Prefab  │
     │ ┌─────────────────┐ │
     │ │ Enemy Data:     │ │
     │ │ [Drag Asset]    │ │ ◄─── Drag MeleeEnemyData here
     │ └─────────────────┘ │
     └─────────────────────┘

Step 3: ASSIGN TO SPAWNER
     ┌─────────────────────┐
     │   EnemySpawner      │
     │ ┌─────────────────┐ │
     │ │ Melee Prefab:   │ │
     │ │ [Drag Prefab]   │ │ ◄─── Drag MeleeEnemy Prefab
     │ └─────────────────┘ │
     └─────────────────────┘

Step 4: RUNTIME
┌──────────────────────────────────────────┐
│ Wave starts → Spawner spawns enemy       │
└──────────────┬───────────────────────────┘
               ▼
     ┌─────────────────────┐
     │  MeleeEnemy         │
     │  (instantiated)     │
     └─────────┬───────────┘
               │
               ▼
     BaseEnemy.Start()
         ├─ LoadStatsFromData()
         │  └─ Read MeleeEnemyData
         │     ├─ maxHealth = 50
         │     ├─ moveSpeed = 4
         │     └─ attackDamage = 15
         │
         └─ ApplyVisuals()
            └─ SpriteRenderer.color = Red
```

---

## 🎯 Inspector View - Correct Setup

### Enemy Prefab (MeleeEnemy)
```
╔═══════════════════════════════════════════╗
║ Inspector - MeleeEnemy                    ║
╠═══════════════════════════════════════════╣
║                                           ║
║ Transform                                 ║
║ ├─ Position: (0, 0, 0)                    ║
║ └─ Scale: (0.5, 0.5, 1)                   ║
║                                           ║
║ ┌───────────────────────────────────────┐ ║
║ │ Sprite Renderer                       │ ║
║ │ ├─ Sprite: Square                     │ ║
║ │ └─ Color: Will change to Red at play  │ ║
║ └───────────────────────────────────────┘ ║
║                                           ║
║ ┌───────────────────────────────────────┐ ║
║ │ ✅ Melee Enemy (Script)               │ ║
║ ├───────────────────────────────────────┤ ║
║ │ Enemy Data                            │ ║
║ │ ┌───────────────────────────────────┐ │ ║
║ │ │ ● MeleeEnemyData                  │ │ ║ ◄─ Asset assigned!
║ │ └───────────────────────────────────┘ │ ║
║ │                                       │ ║
║ │ Runtime Stats (Read-only)             │ ║
║ │ ├─ Max Health: 0 (will be 50 at play)│ ║
║ │ ├─ Current Health: 0                  │ ║
║ │ ├─ Move Speed: 0 (will be 4)          │ ║
║ │ ├─ Attack Damage: 0 (will be 15)      │ ║
║ │ ├─ Attack Range: 0 (will be 2)        │ ║
║ │ └─ Enemy Type: Melee                  │ ║
║ └───────────────────────────────────────┘ ║
║                                           ║
╚═══════════════════════════════════════════╝
```

### EnemyData Asset (MeleeEnemyData)
```
╔═══════════════════════════════════════════╗
║ Inspector - MeleeEnemyData                ║
╠═══════════════════════════════════════════╣
║                                           ║
║ ┌───────────────────────────────────────┐ ║
║ │ Basic Info                            │ ║
║ ├───────────────────────────────────────┤ ║
║ │ Enemy Type: ▼ Melee                   │ ║ ◄─ Select type
║ │ Enemy Name: "Melee Enemy"             │ ║
║ │ Description:                          │ ║
║ │ ┌───────────────────────────────────┐ │ ║
║ │ │ Fast aggressive enemy with low HP │ │ ║
║ │ │ Rushes to tower...                │ │ ║
║ │ └───────────────────────────────────┘ │ ║
║ └───────────────────────────────────────┘ ║
║                                           ║
║ ┌───────────────────────────────────────┐ ║
║ │ Combat Stats                          │ ║
║ ├───────────────────────────────────────┤ ║
║ │ Max Health: [   50   ]                │ ║ ◄─ Edit here!
║ │ Attack Damage: [   15   ]             │ ║ ◄─ Edit here!
║ │ Attack Range: [   2   ]               │ ║
║ │ Attack Cooldown: [   1   ]            │ ║
║ └───────────────────────────────────────┘ ║
║                                           ║
║ ┌───────────────────────────────────────┐ ║
║ │ Movement                              │ ║
║ ├───────────────────────────────────────┤ ║
║ │ Move Speed: [   4   ]                 │ ║ ◄─ Edit here!
║ └───────────────────────────────────────┘ ║
║                                           ║
║ ┌───────────────────────────────────────┐ ║
║ │ Visual                                │ ║
║ ├───────────────────────────────────────┤ ║
║ │ Enemy Color: [■ Red  ]                │ ║ ◄─ Click to change
║ └───────────────────────────────────────┘ ║
║                                           ║
╚═══════════════════════════════════════════╝
```

### EnemySpawner Component
```
╔═══════════════════════════════════════════╗
║ Inspector - WaveManager                   ║
╠═══════════════════════════════════════════╣
║                                           ║
║ ┌───────────────────────────────────────┐ ║
║ │ Enemy Spawner (Script)                │ ║
║ ├───────────────────────────────────────┤ ║
║ │ Enemy Prefabs                         │ ║
║ │ ├─ Melee Prefab                       │ ║
║ │ │  ┌─────────────────────────────────┐│ ║
║ │ │  │ ● MeleeEnemy                    ││ ║ ◄─ Drag prefab
║ │ │  └─────────────────────────────────┘│ ║
║ │ ├─ Ranged Prefab                      │ ║
║ │ │  ┌─────────────────────────────────┐│ ║
║ │ │  │ ● RangedEnemy                   ││ ║ ◄─ Drag prefab
║ │ │  └─────────────────────────────────┘│ ║
║ │ └─ Tank Prefab                        │ ║
║ │    ┌─────────────────────────────────┐│ ║
║ │    │ ● TankEnemy                     ││ ║ ◄─ Drag prefab
║ │    └─────────────────────────────────┘│ ║
║ │                                       │ ║
║ │ Spawn Settings                        │ ║
║ │ ├─ Spawn Radius: [   20   ]           │ ║
║ │ └─ Use Random Spawn: ☑                │ ║
║ └───────────────────────────────────────┘ ║
║                                           ║
╚═══════════════════════════════════════════╝
```

---

## ❌ Common Mistakes

### Mistake 1: Assigning Script Instead of Asset
```
WRONG:
┌─────────────────────┐
│ Enemy Data:         │
│ [EnemyData.cs] ✗    │ ◄─ This is SCRIPT, not ASSET!
└─────────────────────┘

CORRECT:
┌─────────────────────┐
│ Enemy Data:         │
│ [MeleeEnemyData] ✓  │ ◄─ This is ASSET (created via Create menu)
└─────────────────────┘
```

### Mistake 2: No Data Assigned
```
WRONG:
┌─────────────────────┐
│ Enemy Data:         │
│ None (Enemy Data) ✗ │ ◄─ Empty field!
└─────────────────────┘
Result: Enemy has 0 HP, 0 damage, won't work!

CORRECT:
┌─────────────────────┐
│ Enemy Data:         │
│ ● MeleeEnemyData ✓  │ ◄─ Asset assigned
└─────────────────────┘
Result: Stats loaded correctly!
```

### Mistake 3: Wrong Asset Type
```
WRONG:
Melee Prefab ◄─ RangedEnemyData ✗
(Melee enemy using Ranged stats!)

CORRECT:
Melee Prefab ◄─ MeleeEnemyData ✓
Ranged Prefab ◄─ RangedEnemyData ✓
Tank Prefab ◄─ TankEnemyData ✓
```

---

## 🎯 Drag & Drop Reference

### Where to Drag EnemyData Assets:
```
1. Enemy Prefab Inspector
   MeleeEnemy Component
   ├─ Enemy Data: [ DROP HERE ] ◄─ Drag MeleeEnemyData.asset
   
2. Can also drag to:
   - Prefab in Project window
   - GameObject in Hierarchy
   - Inspector window
```

### Where to Drag Enemy Prefabs:
```
EnemySpawner Component (on WaveManager)
├─ Melee Prefab: [ DROP HERE ] ◄─ Drag MeleeEnemy prefab
├─ Ranged Prefab: [ DROP HERE ] ◄─ Drag RangedEnemy prefab
└─ Tank Prefab: [ DROP HERE ] ◄─ Drag TankEnemy prefab
```

---

## ✅ Final Check Before Play

```
☑ MeleeEnemyData.asset created
☑ RangedEnemyData.asset created  
☑ TankEnemyData.asset created
☑ All assets have valid stats (not 0)
☑ MeleeEnemy prefab → MeleeEnemyData assigned
☑ RangedEnemy prefab → RangedEnemyData assigned
☑ TankEnemy prefab → TankEnemyData assigned
☑ EnemySpawner → All 3 prefabs assigned
☑ Tower created with Tower.cs component
☑ GameManager exists
☑ WaveManager exists

→ READY TO PLAY! 🎮
```

---

## 🎨 File Icons Reference

```
Assets/Scriptables/Enemies/
├─ 📄 MeleeEnemyData      ◄─ ScriptableObject icon (data file)
├─ 📄 RangedEnemyData     ◄─ ScriptableObject icon
└─ 📄 TankEnemyData       ◄─ ScriptableObject icon

Assets/Scripts/Enemies/
├─ 📜 BaseEnemy.cs        ◄─ C# script icon
├─ 📜 MeleeEnemy.cs       ◄─ C# script icon
└─ 📜 RangedEnemy.cs      ◄─ C# script icon

Assets/Prefabs/
├─ 🎮 MeleeEnemy          ◄─ Prefab icon (blue cube)
├─ 🎮 RangedEnemy         ◄─ Prefab icon
└─ 🎮 TankEnemy           ◄─ Prefab icon
```

Icon types:
- 📄 = ScriptableObject asset (data)
- 📜 = C# script file (code)
- 🎮 = Prefab (game object template)

---

🎉 **Ikuti diagram ini dan setup pasti berhasil!**
