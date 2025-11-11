# Item Implementation Guide
## All Tower Items - Quick Setup Reference

---

## 📝 **How to Create Items (3 Steps)**

### **Step 1: Right-click in Unity Project**
`Assets/Game/Tower/Items/Data Items/`
→ `Create > Tower > Items > Stat Modifier`

### **Step 2: Configure in Inspector**
Fill out the values according to the tables below

### **Step 3: Add to Shop Pool**
Drag into `ShopManager` → `Item Pool` (or let it auto-load from Resources)

---

## 🟢 **Common Items (Cheap, Stackable)**

### **1. Cardstock Plating**
```
Item Name: Cardstock Plating
Description: Thin armor plating protects from minor hits
Icon: [Armor icon]
Cost: 15
Rarity: Common
Max Stack: 10
Stack Bonus Percent: 1

Stat Type: ArmorFlat
Value: 1
Stat Display Name: Armor
```
**Effect:** +1 Armor per stack (+1% bonus per stack = 1.1 at stack 10)

---

### **2. Chip Battery**
```
Item Name: Chip Battery
Description: Slowly regenerates health between waves
Icon: [Battery icon]
Cost: 20
Rarity: Common
Max Stack: 10
Stack Bonus Percent: 1

Stat Type: RegenFlat
Value: 0.5
Stat Display Name: HP Regen
```
**Effect:** +0.5 HP/sec per stack

---

### **3. Spare Deck**
```
Item Name: Spare Deck
Description: Add more projectiles to your attack cycle
Icon: [Cards icon]
Cost: 25
Rarity: Common
Max Stack: 5
Stack Bonus Percent: 1

Stat Type: AttackCountFlat
Value: 1
Stat Display Name: Attack Count
```
**Effect:** +1 BaseAttackCount

---

### **4. Light Frame**
```
Item Name: Light Frame
Description: Reduces attack interval for faster firing
Icon: [Speed icon]
Cost: 20
Rarity: Common
Max Stack: 8
Stack Bonus Percent: 0

Stat Type: AttackSpeedMultiply
Value: 0.95
Stat Display Name: Attack Speed
```
**Effect:** AttackSpeed × 0.95 (5% faster)
**Note:** Stack Bonus = 0 because each stack just applies 0.95x again

---

## 🔷 **Uncommon Items (Mid Power)**

### **5. Tempered Core**
```
Item Name: Tempered Core
Description: Reinforced core increases maximum health
Icon: [HP icon]
Cost: 50
Rarity: Uncommon
Max Stack: 5
Stack Bonus Percent: 2

Stat Type: MaxHpPercent
Value: 20
Stat Display Name: Max HP
```
**Effect:** +20% Max HP (of current MaxHp)
**Note:** Scales with current MaxHp, compounds nicely

---

### **6. Hardened Seal**
```
Item Name: Hardened Seal
Description: Heavy armor plating blocks significant damage
Icon: [Shield icon]
Cost: 60
Rarity: Uncommon
Max Stack: 5
Stack Bonus Percent: 2

Stat Type: ArmorFlat
Value: 3
Stat Display Name: Armor
```
**Effect:** +3 Armor per stack

---

### **7. Pulse Pump**
```
Item Name: Pulse Pump
Description: Advanced regeneration system for sustained combat
Icon: [Regen icon]
Cost: 55
Rarity: Uncommon
Max Stack: 5
Stack Bonus Percent: 2

Stat Type: RegenFlat
Value: 1.0
Stat Display Name: HP Regen
```
**Effect:** +1.0 HP/sec

---

### **8. Rotary Magazine**
```
Item Name: Rotary Magazine
Description: Dual-barrel system fires twice as many projectiles
Icon: [Magazine icon]
Cost: 70
Rarity: Uncommon
Max Stack: 3
Stack Bonus Percent: 2

Stat Type: AttackCountFlat
Value: 2
Stat Display Name: Attack Count
```
**Effect:** +2 BaseAttackCount

---

### **9. Precision Bearings**
```
Item Name: Precision Bearings
Description: Precision mechanics reduce attack interval
Icon: [Gears icon]
Cost: 65
Rarity: Uncommon
Max Stack: 5
Stack Bonus Percent: 0

Stat Type: AttackSpeedFlat
Value: -0.15
Stat Display Name: Attack Speed
```
**Effect:** Attack interval -0.15s (clamped to min 0.15s)
**Note:** Flat reduction feels "snappy", stack bonus = 0

---

## 🟨 **Rare Items (High Impact)**

### **10. Monolith Core**
```
Item Name: Monolith Core
Description: Massive core expansion grants incredible durability
Icon: [Monolith icon]
Cost: 120
Rarity: Rare
Max Stack: 3
Stack Bonus Percent: 5

Stat Type: MaxHpPercent
Value: 40
Stat Display Name: Max HP
```
**Effect:** +40% Max HP
**Note:** Huge HP boost, great with Armor

---

### **11. Phase Bearings**
```
Item Name: Phase Bearings
Description: Phase-shift technology accelerates all systems
Icon: [Lightning icon]
Cost: 130
Rarity: Rare
Max Stack: 3
Stack Bonus Percent: 0

Stat Type: AttackSpeedMultiply
Value: 0.80
Stat Display Name: Attack Speed
```
**Effect:** AttackSpeed × 0.80 (20% faster)
**Warning:** Very strong, balance carefully

---

### **12. Hydra Rack**
```
Item Name: Hydra Rack
Description: Multi-barrel array fires devastating volleys
Icon: [Hydra icon]
Cost: 150
Rarity: Rare
Max Stack: 2
Stack Bonus Percent: 3

Stat Type: AttackCountFlat
Value: 3
Stat Display Name: Attack Count
```
**Effect:** +3 BaseAttackCount
**Note:** Explosive scaling for multi-projectile skills

---

## 🎨 **Unity Workflow (Fast)**

### **Batch Creation (5 minutes)**
1. Create folder: `Assets/Game/Tower/Items/Data Items/Common/`
2. Create folder: `Assets/Game/Tower/Items/Data Items/Uncommon/`
3. Create folder: `Assets/Game/Tower/Items/Data Items/Rare/`

4. For each item above:
   - Right-click in appropriate folder
   - `Create > Tower > Items > Stat Modifier`
   - Rename to item name
   - Copy-paste values from table above into Inspector

### **Auto-Load into Shop**
Option A: **Manual** (ShopManager Inspector)
- Drag all items into `Item Pool` array

Option B: **Auto-Load from Resources** (Recommended)
- Move `Data Items/` folder to `Assets/Resources/Items/`
- ShopManager will auto-load all items in `Resources/Items/`

---

## 🔧 **Balance Notes**

### **Stack Bonus Percent Guide**
- **0%** - Multiplicative items (AttackSpeed multipliers)
- **1%** - Common items (safe, slow scaling)
- **2%** - Uncommon items (noticeable scaling)
- **3-5%** - Rare items (high impact)

### **Cost Guide**
- **Common:** 15-25 chips
- **Uncommon:** 50-70 chips
- **Rare:** 120-150 chips

### **Max Stack Guide**
- **High stacks (8-10):** Weak per-stack bonus (Common armor/regen)
- **Medium stacks (5):** Moderate bonus (Uncommon items)
- **Low stacks (2-3):** Strong bonus (Rare items)

---

## 🎯 **GameJam Best Practices Applied**

✅ **Single Generic Class** - One `StatModifierItem.cs` for all stat items
✅ **Inspector-Driven** - Zero hardcoding, all values in ScriptableObject
✅ **Reusable** - Create items in seconds, not minutes
✅ **Type-Safe Enum** - `StatModifierType` prevents typos
✅ **Clear Naming** - `MaxHpFlat` vs `MaxHpPercent` is self-documenting
✅ **Debug Friendly** - All items log their effects
✅ **Stacking Built-In** - Works with existing stack system
✅ **Description Auto-Update** - Shows current bonus dynamically

---

## 🚀 **Testing Checklist**

### **Quick Test (2 minutes)**
1. Create "Cardstock Plating" (+1 Armor)
2. Add to Item Pool
3. Play mode → Open shop
4. Purchase 3 times
5. Check Inspector: Armor should be base + 3.03 (stack bonus)

### **Full Test (10 minutes)**
- [ ] Create one item from each rarity
- [ ] Verify cost/rarity display in shop
- [ ] Stack each item to max
- [ ] Check tower stats in Inspector match expected values
- [ ] Verify description shows current bonus
- [ ] Test "UPGRADE" button appears for stacked items

---

## 📊 **Synergy Examples**

**Tank Build:**
- Monolith Core (+40% HP)
- Tempered Core (+20% HP)
- Hardened Seal (+3 Armor) × 5 stacks
- **Result:** ~200% HP, 15+ Armor

**Speed Build:**
- Phase Bearings (×0.80)
- Light Frame (×0.95) × 8 stacks
- Precision Bearings (-0.15s) × 5
- **Result:** ~70% faster attacks

**Spam Build:**
- Hydra Rack (+3 Count)
- Rotary Magazine (+2 Count)
- Spare Deck (+1 Count) × 5
- **Result:** 11+ projectiles per cycle

---

## 🛠️ **Extending the System**

### **Add New Stat Type (3 steps)**

1. **Add to Enum**
```csharp
public enum StatModifierType
{
    // ... existing types
    CritChanceFlat,  // New stat
}
```

2. **Add to ApplyEffect()**
```csharp
case StatModifierType.CritChanceFlat:
    tower.AddCritChance(value * stackMult);
    break;
```

3. **Add to TowerRuntime** (if needed)
```csharp
public void AddCritChance(float flat) => currentStats.CritChance += flat;
```

Done! Now you can create Crit items using the same `StatModifierItem` class.
