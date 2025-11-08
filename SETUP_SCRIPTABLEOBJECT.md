# 🛠️ Setup ScriptableObject - Step by Step

## ⚠️ Masalah: "ScriptableObject tidak bisa dimasukkan ke EnemyData field"

**Penyebab:** ScriptableObject assets harus dibuat **di dalam Unity Editor**, bukan manual dengan text file.

---

## ✅ Cara Setup yang Benar

### 📋 Prerequisite
1. ✅ Script `EnemyData.cs` sudah ada di `Assets/Scriptables/`
2. ✅ Unity sudah open project ini
3. ✅ Tidak ada compile errors

---

## 🎯 Step 1: Create EnemyData Assets

### A. Via Right-Click Menu (Recommended)

**Untuk Melee Enemy:**
```
1. Di Unity Project window, navigate ke: Assets/Scriptables/Enemies/
2. Right-click di folder → Create → Tower of Odds → Enemy Data
3. Rename asset: "MeleeEnemyData"
4. Select asset, edit di Inspector:
   
   Basic Info:
   ├─ Enemy Type: Melee
   ├─ Enemy Name: "Melee Enemy"
   └─ Description: "Fast aggressive enemy..."

   Combat Stats:
   ├─ Max Health: 50
   ├─ Attack Damage: 15
   ├─ Attack Range: 2
   └─ Attack Cooldown: 1

   Movement:
   └─ Move Speed: 4

   Special Behaviors:
   ├─ Preferred Distance: 8 (tidak dipakai Melee, ignore)
   └─ Damage Reduction: 0

   Projectile:
   ├─ Projectile Prefab: None (Melee tidak pakai projectile)
   └─ Projectile Speed: 15

   Visual:
   └─ Enemy Color: Red (R:1, G:0, B:0)
```

**Untuk Ranged Enemy:**
```
1. Right-click → Create → Tower of Odds → Enemy Data
2. Rename: "RangedEnemyData"
3. Edit di Inspector:
   
   Basic Info:
   ├─ Enemy Type: Ranged
   ├─ Enemy Name: "Ranged Enemy"
   └─ Description: "Medium speed, kiting enemy..."

   Combat Stats:
   ├─ Max Health: 50
   ├─ Attack Damage: 10
   ├─ Attack Range: 10
   └─ Attack Cooldown: 1.5

   Movement:
   └─ Move Speed: 2.5

   Special Behaviors:
   ├─ Preferred Distance: 8 (PENTING untuk kiting!)
   └─ Damage Reduction: 0

   Projectile:
   ├─ Projectile Prefab: (assign projectile prefab nanti)
   └─ Projectile Speed: 15

   Visual:
   └─ Enemy Color: Blue (R:0, G:0.5, B:1)
```

**Untuk Tank Enemy:**
```
1. Right-click → Create → Tower of Odds → Enemy Data
2. Rename: "TankEnemyData"
3. Edit di Inspector:
   
   Basic Info:
   ├─ Enemy Type: Tank
   ├─ Enemy Name: "Tank Enemy"
   └─ Description: "Slow but tanky..."

   Combat Stats:
   ├─ Max Health: 175
   ├─ Attack Damage: 25
   ├─ Attack Range: 3
   └─ Attack Cooldown: 2

   Movement:
   └─ Move Speed: 1.5

   Special Behaviors:
   ├─ Preferred Distance: 8 (tidak dipakai Tank, ignore)
   └─ Damage Reduction: 0.2 (20% reduction!)

   Projectile:
   ├─ Projectile Prefab: None
   └─ Projectile Speed: 15

   Visual:
   └─ Enemy Color: Gray (R:0.5, G:0.5, B:0.5)
```

### B. Via Assets Menu (Alternative)
```
1. Assets → Create → Tower of Odds → Enemy Data
2. Rename dan edit seperti di atas
```

---

## 🎯 Step 2: Create Enemy Prefabs

### Create Melee Enemy Prefab

```
1. Hierarchy → Right-click → 2D Object → Sprites → Square
2. Rename: "MeleeEnemy"
3. Setup Transform:
   - Position: (0, 0, 0)
   - Scale: (0.5, 0.5, 1)

4. Add Component: MeleeEnemy (script)
5. Pada komponen MeleeEnemy di Inspector:
   ┌─────────────────────────────┐
   │ Melee Enemy (Script)        │
   ├─────────────────────────────┤
   │ Enemy Data                  │
   │ ┌─────────────────────────┐ │
   │ │ [Drag MeleeEnemyData]   │ │ ← DRAG SINI!
   │ └─────────────────────────┘ │
   └─────────────────────────────┘

6. Drag MeleeEnemyData asset ke field "Enemy Data"

7. Sprite Renderer:
   - Color akan auto-set ke Red saat Play (dari EnemyData)
   - Atau manual set ke Red sekarang

8. (Optional) Add CircleCollider2D untuk collision

9. Drag ke Assets/Prefabs/ untuk buat prefab
10. Delete dari Hierarchy
```

### Create Ranged Enemy Prefab

```
1. Hierarchy → 2D Object → Sprites → Circle
2. Rename: "RangedEnemy"
3. Scale: (0.4, 0.4, 1)
4. Add Component: RangedEnemy (script)
5. Drag RangedEnemyData ke field "Enemy Data"
6. Sprite color akan jadi Blue saat play
7. Buat prefab di Assets/Prefabs/
```

### Create Tank Enemy Prefab

```
1. Hierarchy → 2D Object → Sprites → Square
2. Rename: "TankEnemy"
3. Scale: (0.8, 0.8, 1)
4. Add Component: TankEnemy (script)
5. Drag TankEnemyData ke field "Enemy Data"
6. Sprite color akan jadi Gray saat play
7. Buat prefab di Assets/Prefabs/
```

---

## 🎯 Step 3: Assign Prefabs ke EnemySpawner

```
1. Hierarchy → Select "WaveManager" (atau GameObject yang punya EnemySpawner)
2. Pada komponen Enemy Spawner di Inspector:
   
   ┌─────────────────────────────┐
   │ Enemy Spawner (Script)      │
   ├─────────────────────────────┤
   │ Enemy Prefabs               │
   │ ├─ Melee Prefab             │
   │ │  └─ [Drag MeleeEnemy]     │ ← Drag prefab
   │ ├─ Ranged Prefab            │
   │ │  └─ [Drag RangedEnemy]    │ ← Drag prefab
   │ └─ Tank Prefab              │
   │    └─ [Drag TankEnemy]      │ ← Drag prefab
   │                             │
   │ Spawn Settings              │
   │ ├─ Spawn Radius: 20         │
   │ └─ Use Random Spawn: ✓      │
   └─────────────────────────────┘
```

---

## 🎯 Step 4: Create Projectile Prefab (Optional tapi recommended)

### Untuk Tower Projectile

```
1. Hierarchy → 2D Object → Sprites → Circle
2. Rename: "TowerProjectile"
3. Scale: (0.15, 0.15, 1)
4. Add Component: Projectile (script dari Assets/Scripts/Combat/)
5. Sprite Renderer:
   - Color: Yellow atau White
6. Drag ke Assets/Prefabs/ untuk buat prefab
```

### Untuk Ranged Enemy Projectile

```
1. Hierarchy → 2D Object → Sprites → Circle
2. Rename: "EnemyProjectile"
3. Scale: (0.1, 0.1, 1)
4. Add Component: Projectile (script)
5. Sprite Renderer:
   - Color: Red atau Orange
6. Drag ke Assets/Prefabs/ untuk buat prefab
```

### Assign Projectile ke Tower

```
1. Select Tower GameObject di Hierarchy
2. Pada komponen Tower (Script):
   
   ┌─────────────────────────────┐
   │ Tower (Script)              │
   ├─────────────────────────────┤
   │ Projectile                  │
   │ ├─ Projectile Prefab        │
   │ │  └─ [Drag TowerProjectile]│ ← Drag prefab
   │ └─ Projectile Speed: 20     │
   └─────────────────────────────┘
```

### Assign Projectile ke Ranged Enemy Data

```
1. Select RangedEnemyData asset
2. Di Inspector:
   
   ┌─────────────────────────────┐
   │ Ranged Enemy Data           │
   ├─────────────────────────────┤
   │ Projectile (Ranged Enemy)   │
   │ ├─ Projectile Prefab        │
   │ │  └─ [Drag EnemyProjectile]│ ← Drag prefab
   │ └─ Projectile Speed: 15     │
   └─────────────────────────────┘
```

---

## ✅ Verification Checklist

Sebelum Play, pastikan:

**ScriptableObjects Created:**
- [ ] `MeleeEnemyData.asset` exists di `Assets/Scriptables/Enemies/`
- [ ] `RangedEnemyData.asset` exists
- [ ] `TankEnemyData.asset` exists
- [ ] Semua ada icon ScriptableObject (file icon)

**Enemy Prefabs Setup:**
- [ ] MeleeEnemy prefab punya `MeleeEnemy` script component
- [ ] Field "Enemy Data" terisi dengan `MeleeEnemyData`
- [ ] RangedEnemy prefab → `RangedEnemyData`
- [ ] TankEnemy prefab → `TankEnemyData`

**EnemySpawner Assigned:**
- [ ] Melee Prefab field terisi
- [ ] Ranged Prefab field terisi
- [ ] Tank Prefab field terisi

**Projectiles (Optional):**
- [ ] Tower punya Projectile Prefab assigned
- [ ] RangedEnemyData punya Projectile Prefab assigned

---

## 🐛 Troubleshooting

### "Create menu tidak muncul"
**Fix:**
```
1. Check apakah EnemyData.cs ada di Assets/Scriptables/
2. Check compile errors (harus 0 errors)
3. Restart Unity Editor
4. Reimport script: Right-click EnemyData.cs → Reimport
```

### "Field Enemy Data tidak muncul di Inspector"
**Fix:**
```
1. Check apakah BaseEnemy.cs sudah ada field:
   [Header("Enemy Data")]
   [SerializeField] protected Scriptables.EnemyData enemyData;

2. Recompile scripts (Ctrl+R atau restart Unity)
```

### "Drag & Drop tidak work"
**Fix:**
```
1. Pastikan yang di-drag adalah EnemyData ASSET (icon ScriptableObject)
2. Bukan script .cs file!
3. Asset harus dibuat via Create menu, bukan manual
```

### "Enemy spawn tapi stats salah"
**Fix:**
```
1. Play Mode → Select enemy di Hierarchy
2. Check Inspector → Lihat "Runtime Stats (Read-only)"
3. Pastikan stats match dengan EnemyData
4. Jika null, berarti Enemy Data tidak assigned ke prefab
```

### "Warna enemy tidak berubah"
**Fix:**
```
1. Check apakah enemy prefab punya SpriteRenderer
2. Check EnemyData.enemyColor tidak putih/default
3. Method ApplyVisuals() dipanggil di BaseEnemy.Start()
```

---

## 📸 Visual Guide

### Correct Setup (Enemy Prefab Inspector)
```
┌────────────────────────────────┐
│ ✅ Melee Enemy (Script)        │
├────────────────────────────────┤
│ Enemy Data                     │
│ ┌────────────────────────────┐ │
│ │ MeleeEnemyData             │ │ ← Asset icon visible
│ └────────────────────────────┘ │
│                                │
│ Runtime Stats (Read-only)      │
│ ├─ Max Health: 50              │ ← Auto-filled dari data
│ ├─ Move Speed: 4               │
│ └─ Attack Damage: 15           │
└────────────────────────────────┘
```

### Wrong Setup (Empty Field)
```
┌────────────────────────────────┐
│ ❌ Melee Enemy (Script)        │
├────────────────────────────────┤
│ Enemy Data                     │
│ ┌────────────────────────────┐ │
│ │ None (Enemy Data)          │ │ ← KOSONG! Drag asset sini
│ └────────────────────────────┘ │
│                                │
│ Runtime Stats (Read-only)      │
│ ├─ Max Health: 0               │ ← Salah karena no data
│ ├─ Move Speed: 0               │
│ └─ Attack Damage: 0            │
└────────────────────────────────┘
```

---

## 🎯 Quick Setup Summary

**3 Main Steps:**
1. **Create EnemyData Assets** (Right-click → Create → Tower of Odds → Enemy Data)
2. **Create Enemy Prefabs** (Add script → Drag EnemyData asset)
3. **Assign to Spawner** (Drag prefabs ke EnemySpawner)

**Time needed:** ~5-10 minutes untuk setup semua

**Result:** 
✅ Enemy stats editable di Inspector
✅ No hardcoded values
✅ Easy to balance & test

---

🎉 **Setelah setup, tinggal Play dan test!**
