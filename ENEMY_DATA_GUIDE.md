# 📊 Enemy Data - ScriptableObject System

## ✨ Keuntungan Menggunakan ScriptableObject

Sekarang enemy stats **tidak lagi hardcoded** di script! Semua data disimpan dalam **EnemyData ScriptableObject** yang bisa diedit langsung di Inspector.

### ✅ Benefits:
- **Edit stats tanpa buka script** - Ubah HP, damage, speed langsung di Inspector
- **Reusable & Modular** - Bisa buat variant enemy (Fast Melee, Armored Tank, dll)
- **No compile** - Perubahan stats tidak perlu recompile script
- **Version control friendly** - Data terpisah dari logic
- **Designer-friendly** - Non-programmer bisa balance game
- **Easy to test** - Coba variant stats dengan cepat

---

## 📁 File Structure

```
Assets/
  Scriptables/
    EnemyData.cs                    # ScriptableObject class
    Enemies/
      MeleeEnemyData.asset          # Melee enemy stats
      RangedEnemyData.asset         # Ranged enemy stats
      TankEnemyData.asset           # Tank enemy stats
  Scripts/
    Enemies/
      BaseEnemy.cs                  # Refactored untuk pakai EnemyData
      MeleeEnemy.cs                 # Simpel, hanya behavior
      RangedEnemy.cs                # Load preferredDistance dari data
      TankEnemy.cs                  # Load damageReduction dari data
```

---

## 🎮 Cara Pakai di Unity

### 1. **Setup Enemy Prefabs**

Untuk **setiap enemy prefab** (MeleeEnemy, RangedEnemy, TankEnemy):

```
1. Select prefab
2. Pada komponen Enemy script (e.g., MeleeEnemy)
3. Drag EnemyData asset ke field "Enemy Data"
   - MeleeEnemy → MeleeEnemyData
   - RangedEnemy → RangedEnemyData
   - TankEnemy → TankEnemyData
```

### 2. **Edit Stats di Inspector**

```
1. Select EnemyData asset di Assets/Scriptables/Enemies/
2. Edit values di Inspector:
   - Max Health
   - Attack Damage
   - Attack Range
   - Attack Cooldown
   - Move Speed
   - Preferred Distance (Ranged)
   - Damage Reduction (Tank)
   - Projectile Prefab
   - Enemy Color
3. Changes save otomatis!
```

### 3. **Create Variant Enemy**

Mau buat **Fast Melee** atau **Armored Tank**?

```
1. Right-click di Assets/Scriptables/Enemies/
2. Create → Tower of Odds → Enemy Data
3. Rename: "FastMeleeData"
4. Edit stats:
   - Enemy Type: Melee
   - Move Speed: 6 (lebih cepat dari 4)
   - Max Health: 30 (lebih rendah)
   - Attack Damage: 20 (lebih tinggi)
5. Create new prefab atau duplicate existing
6. Assign FastMeleeData ke prefab
7. Done! Sekarang punya enemy baru tanpa coding
```

---

## 📊 Default Stats

### Melee Enemy (Red)
```yaml
Enemy Type: Melee
Max Health: 50
Attack Damage: 15
Attack Range: 2
Attack Cooldown: 1s
Move Speed: 4
Shots to Kill: 2 (Tower DMG 25)
Color: Red (1, 0, 0)
```

### Ranged Enemy (Blue)
```yaml
Enemy Type: Ranged
Max Health: 50
Attack Damage: 10
Attack Range: 10
Attack Cooldown: 1.5s
Move Speed: 2.5
Preferred Distance: 8
Shots to Kill: 2 (Tower DMG 25)
Color: Blue (0, 0.5, 1)
```

### Tank Enemy (Gray)
```yaml
Enemy Type: Tank
Max Health: 175
Attack Damage: 25
Attack Range: 3
Attack Cooldown: 2s
Move Speed: 1.5
Damage Reduction: 20%
Shots to Kill: 7-8 (with reduction)
Color: Gray (0.5, 0.5, 0.5)
```

---

## 🔧 Advanced: Custom Enemy Data

### Example: Boss Enemy

```csharp
// Buat BossEnemyData.asset
Enemy Type: Tank
Enemy Name: "Boss Tank"
Description: "Massive boss with high HP and shield"

Max Health: 500
Attack Damage: 50
Attack Range: 5
Attack Cooldown: 3
Move Speed: 0.8
Damage Reduction: 0.5 (50% reduction!)
Enemy Color: Dark Red
```

### Example: Suicide Bomber

```csharp
// Buat SuicideBomberData.asset
Enemy Type: Melee
Enemy Name: "Bomber"
Description: "Explodes on contact"

Max Health: 20
Attack Damage: 100 (one-shot damage!)
Attack Range: 1
Attack Cooldown: 0.1
Move Speed: 5
Enemy Color: Orange
```

---

## 🎨 Visual Customization

EnemyData juga menyimpan **visual settings**:

```yaml
enemyColor: {r: 1, g: 0, b: 0, a: 1}
```

Warna ini otomatis di-apply ke `SpriteRenderer` saat enemy spawn.

**Cara ubah warna:**
1. Select EnemyData asset
2. Klik color field "Enemy Color"
3. Pilih warna baru
4. Spawn enemy → warna berubah otomatis!

---

## 🔄 Migration dari Old System

**Before (Hardcoded):**
```csharp
protected override void Start()
{
    maxHealth = 50f;
    moveSpeed = 4f;
    attackDamage = 15f;
    // ...
    base.Start();
}
```

**After (ScriptableObject):**
```csharp
// MeleeEnemy.cs sekarang kosong, hanya behavior
// Stats loaded otomatis dari MeleeEnemyData.asset
```

**Benefits:**
- ✅ MeleeEnemy.cs lebih bersih (no hardcoded values)
- ✅ Bisa buat variant tanpa duplicate script
- ✅ Balance game tanpa recompile

---

## 📝 Creating New Enemy Data

**Via Unity Menu:**
```
Right-click in Project window
→ Create
→ Tower of Odds
→ Enemy Data
```

**Fields yang harus diisi:**
```
✅ Enemy Type (Melee/Ranged/Tank)
✅ Enemy Name
✅ Max Health
✅ Attack Damage
✅ Attack Range
✅ Attack Cooldown
✅ Move Speed
```

**Optional fields:**
```
⚙️ Preferred Distance (Ranged only)
⚙️ Damage Reduction (Tank only)
⚙️ Projectile Prefab (Ranged)
⚙️ Enemy Color
```

---

## 🐛 Troubleshooting

### Enemy tidak spawn / error di console
**Solution:** Check apakah `Enemy Data` field sudah di-assign di prefab.

### Stats tidak berubah setelah edit EnemyData
**Solution:** 
1. Check apakah prefab menggunakan EnemyData yang benar
2. Restart play mode
3. Check `Runtime Stats` di Inspector saat play

### Warna enemy tidak berubah
**Solution:**
1. Pastikan enemy prefab punya `SpriteRenderer` component
2. Check `Enemy Color` di EnemyData asset
3. Method `ApplyVisuals()` dipanggil di `BaseEnemy.Start()`

---

## 🎯 Best Practices

### 1. **Naming Convention**
```
✅ MeleeEnemyData.asset
✅ FastMeleeEnemyData.asset
✅ ArmoredTankEnemyData.asset
❌ enemy1.asset
❌ data.asset
```

### 2. **Organization**
```
Assets/Scriptables/Enemies/
  Basic/
    MeleeEnemyData.asset
    RangedEnemyData.asset
    TankEnemyData.asset
  Variants/
    FastMeleeData.asset
    SniperRangedData.asset
    BossTankData.asset
```

### 3. **Balance Testing**
- Edit stats di EnemyData
- Test in Play Mode
- Check `Runtime Stats (Read-only)` di Inspector
- Iterate tanpa restart Unity

### 4. **Version Control**
- Commit `.asset` files
- Easy to review stat changes in Git diff
- Designer dapat submit balance changes

---

## ✅ Summary

**What Changed:**
- ✅ Created `EnemyData.cs` ScriptableObject
- ✅ Refactored `BaseEnemy.cs` to load from EnemyData
- ✅ Simplified `MeleeEnemy`, `RangedEnemy`, `TankEnemy`
- ✅ Created 3 default EnemyData assets

**How to Use:**
1. Assign EnemyData to enemy prefabs
2. Edit stats in Inspector
3. Create variants by duplicating assets
4. Balance game without coding

**Upgrade Path:**
- Old system: Hardcoded values in scripts
- New system: Data-driven with ScriptableObjects
- **100% backward compatible** (falls back to defaults if no data)

---

🎉 **Sekarang enemy stats bisa diedit langsung di Inspector!**
