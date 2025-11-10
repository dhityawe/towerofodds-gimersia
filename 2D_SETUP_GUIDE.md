# 🎮 2D Setup Guide - Quick Start

## ✅ Perubahan untuk 2D Project

### 📝 Yang Sudah Diubah:

1. **✅ Movement System** - Menggunakan Vector2 (X, Y) bukan Vector3
2. **✅ Spawn Position** - Z axis selalu 0
3. **✅ Distance Calculation** - Menggunakan Vector2.Distance
4. **✅ Enemy Kiting** - Ranged enemy kite di 2D plane

---

## 🚀 Setup Cepat (2D)

### 1. **Project Settings**
Pastikan project Unity Anda dalam **2D Mode**:
- File → Build Settings → Platform → Select "PC, Mac & Linux Standalone"
- Edit → Project Settings → Editor → Default Behavior Mode → **2D**

### 2. **Camera Setup (2D)**
- Main Camera position: (0, 0, -10)
- Camera → Projection: **Orthographic**
- Size: 10 (sesuaikan dengan arena Anda)

### 3. **Create Tower (2D)**
```
1. GameObject → 2D Object → Sprites → Square
2. Rename: "Tower"
3. Tag: "Tower"
4. SpriteRenderer Color: Green
5. Transform:
   - Position: (0, 0, 0)
   - Scale: (1, 1, 1)
6. Add Component: Tower.cs
```

### 4. **Create Enemy Prefabs (2D)**

#### Melee Enemy (Red - Fast)
```
1. GameObject → 2D Object → Sprites → Square
2. Rename: "MeleeEnemy"
3. SpriteRenderer Color: Red (#FF0000)
4. Transform Scale: (0.5, 0.5, 1)
5. Add Component: MeleeEnemy.cs
6. Optional: Add CircleCollider2D
7. Drag to Prefabs folder
```

#### Ranged Enemy (Blue - Kiter)
```
1. GameObject → 2D Object → Sprites → Circle
2. Rename: "RangedEnemy"
3. SpriteRenderer Color: Blue (#0000FF)
4. Transform Scale: (0.4, 0.4, 1)
5. Add Component: RangedEnemy.cs
6. Optional: Add CircleCollider2D
7. Drag to Prefabs folder
```

#### Tank Enemy (Gray - Tanky)
```
1. GameObject → 2D Object → Sprites → Square
2. Rename: "TankEnemy"
3. SpriteRenderer Color: Gray (#808080)
4. Transform Scale: (0.8, 0.8, 1)
5. Add Component: TankEnemy.cs
6. Optional: Add BoxCollider2D
7. Drag to Prefabs folder
```

### 5. **Create Managers**
```
1. Create Empty GameObject: "GameManager"
   - Add Component: GameManager.cs
   
2. Create Empty GameObject: "WaveManager"
   - Add Component: WaveManager.cs
   - Add Component: EnemySpawner.cs
   - Assign Prefabs:
     * Melee Prefab → MeleeEnemy
     * Ranged Prefab → RangedEnemy
     * Tank Prefab → TankEnemy
```

### 6. **Arena Setup (Optional)**
Buat boundary untuk visual:
```
1. GameObject → 2D Object → Sprites → Square
2. Rename: "Arena"
3. Scale: (20, 20, 1) - sesuaikan
4. SpriteRenderer:
   - Color: Transparent atau grid
   - Sorting Layer: Background
   - Order in Layer: -1
```

---

## 🎯 2D-Specific Considerations

### Movement
- Enemies bergerak di **XY plane** (Z selalu 0)
- Kecepatan: units per second di 2D space
- Melee rush straight, Ranged kite backward

### Spawning
- Spawn radius: Circle di sekitar spawner (2D)
- Random.insideUnitCircle untuk posisi spawn
- Z position always 0

### Visuals
- **Sprites** lebih baik daripada 3D models
- Sorting Layers untuk layering:
  - Background: -10
  - Enemies: 0
  - Tower: 1
  - UI: 100

### Camera View
Pilih salah satu:
- **Top-Down**: Camera melihat dari atas (0, 0, -10)
- **Side-View**: Camera dari samping (belum implemented)

---

## 📊 Testing (2D)

1. **Press Play**
2. **Check:**
   - ✅ Camera menampilkan semua objects
   - ✅ Tower di tengah (visible)
   - ✅ Enemies spawn dan move toward tower (2D)
   - ✅ Distance calculation benar (XY only)
   - ✅ Ranged enemies kite correctly (backward)
   - ✅ Tower attacks closest enemy
   - ✅ No Z-axis movement issues

---

## 🎨 Visual Improvements (Optional)

### Sprites
Ganti Square sprites dengan sprites yang lebih menarik:
```
1. Import sprites ke Assets/Sprites/
2. Assign ke SpriteRenderer di prefabs
3. Adjust scale untuk size yang sesuai
```

### Animations
Tambahkan Animator untuk movement:
```
1. Create Animator Controller
2. Add animations: Idle, Walk, Attack, Die
3. Attach to enemy prefabs
```

### Particles
Tambahkan effects:
```
1. Death explosion
2. Attack effects
3. Spawn effects
```

---

## 🔧 Common 2D Issues & Fixes

### Issue: Enemies bergerak di Z axis
**Fix:** Check MovementBehavior() - pastikan Z selalu 0

### Issue: Camera tidak menampilkan objects
**Fix:** 
- Camera Projection → Orthographic
- Camera Z position → -10
- Objects Z position → 0

### Issue: Sprites tidak terlihat
**Fix:**
- Check Sorting Layer & Order in Layer
- Pastikan SpriteRenderer enabled
- Check Camera Culling Mask

### Issue: Distance calculation salah
**Fix:** Gunakan Vector2.Distance, bukan Vector3.Distance

---

## 🎮 Controls (Future)

Untuk kontrol player (jika diperlukan):
```csharp
// Example: Move tower with WASD (optional)
void Update() {
    float h = Input.GetAxis("Horizontal");
    float v = Input.GetAxis("Vertical");
    transform.position += new Vector3(h, v, 0) * speed * Time.deltaTime;
}
```

---

## 📈 Scaling for 2D

Arena size recommendations:
```
Small Arena:  20x20 units
Medium Arena: 30x30 units
Large Arena:  40x40 units
```

Camera size:
```
Small:  Size = 10
Medium: Size = 15
Large:  Size = 20
```

Spawn radius:
```
Small:  5-10 units
Medium: 10-15 units
Large:  15-20 units
```

---

## ✅ Final Checklist

- [ ] Project in 2D mode
- [ ] Camera Orthographic
- [ ] All objects Z = 0
- [ ] Tower dengan SpriteRenderer (Green)
- [ ] 3 Enemy prefabs dengan sprites (Red/Blue/Gray)
- [ ] GameManager & WaveManager setup
- [ ] Prefabs assigned ke EnemySpawner
- [ ] Test: Enemies move in 2D plane only
- [ ] Test: Tower attacks closest enemy

---

**Ready to Play! 🎉**

Press Play dan enjoy your 2D tower defense game!
