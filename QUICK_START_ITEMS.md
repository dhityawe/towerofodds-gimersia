# Quick Start: Create All 12 Items (10 Minutes)

## 🚀 **Fast Track Setup**

### **Step 1: Navigate to Folder (10 sec)**
`Assets/Game/Tower/Items/Data Items/`

---

## 🟢 **Common Items (5 minutes)**

### **1. Cardstock Plating** ⚔️
Right-click → `Create > Tower > Items > Stat Modifier`
```
Name: Cardstock Plating
Description: Thin armor plating protects from minor hits
Cost: 15
Rarity: Common
Max Stack: 10
Stack Bonus %: 1
Stat Type: ArmorFlat
Value: 1
```

### **2. Chip Battery** 🔋
```
Name: Chip Battery
Description: Slowly regenerates health between waves
Cost: 20
Rarity: Common
Max Stack: 10
Stack Bonus %: 1
Stat Type: RegenFlat
Value: 0.5
```

### **3. Spare Deck** 🎴
```
Name: Spare Deck
Description: Add more projectiles to your attack cycle
Cost: 25
Rarity: Common
Max Stack: 5
Stack Bonus %: 1
Stat Type: AttackCountFlat
Value: 1
```

### **4. Light Frame** ⚡
```
Name: Light Frame
Description: Reduces attack interval for faster firing
Cost: 20
Rarity: Common
Max Stack: 8
Stack Bonus %: 0
Stat Type: AttackSpeedMultiply
Value: 0.95
```

---

## 🔷 **Uncommon Items (3 minutes)**

### **5. Tempered Core** 💙
```
Name: Tempered Core
Description: Reinforced core increases maximum health
Cost: 50
Rarity: Uncommon
Max Stack: 5
Stack Bonus %: 2
Stat Type: MaxHpPercent
Value: 20
```

### **6. Hardened Seal** 🛡️
```
Name: Hardened Seal
Description: Heavy armor plating blocks significant damage
Cost: 60
Rarity: Uncommon
Max Stack: 5
Stack Bonus %: 2
Stat Type: ArmorFlat
Value: 3
```

### **7. Pulse Pump** 💚
```
Name: Pulse Pump
Description: Advanced regeneration system for sustained combat
Cost: 55
Rarity: Uncommon
Max Stack: 5
Stack Bonus %: 2
Stat Type: RegenFlat
Value: 1.0
```

### **8. Rotary Magazine** 🎯
```
Name: Rotary Magazine
Description: Dual-barrel system fires twice as many projectiles
Cost: 70
Rarity: Uncommon
Max Stack: 3
Stack Bonus %: 2
Stat Type: AttackCountFlat
Value: 2
```

### **9. Precision Bearings** ⚙️
```
Name: Precision Bearings
Description: Precision mechanics reduce attack interval
Cost: 65
Rarity: Uncommon
Max Stack: 5
Stack Bonus %: 0
Stat Type: AttackSpeedFlat
Value: -0.15
```

---

## 🟨 **Rare Items (2 minutes)**

### **10. Monolith Core** 🗿
```
Name: Monolith Core
Description: Massive core expansion grants incredible durability
Cost: 120
Rarity: Rare
Max Stack: 3
Stack Bonus %: 5
Stat Type: MaxHpPercent
Value: 40
```

### **11. Phase Bearings** ⚡💜
```
Name: Phase Bearings
Description: Phase-shift technology accelerates all systems
Cost: 130
Rarity: Rare
Max Stack: 3
Stack Bonus %: 0
Stat Type: AttackSpeedMultiply
Value: 0.80
```

### **12. Hydra Rack** 🐉
```
Name: Hydra Rack
Description: Multi-barrel array fires devastating volleys
Cost: 150
Rarity: Rare
Max Stack: 2
Stack Bonus %: 3
Stat Type: AttackCountFlat
Value: 3
```

---

## ✅ **Verification (30 seconds)**

1. Select all 12 items
2. Check Inspector → All fields filled?
3. Drag all into `ShopManager` → `Item Pool`
4. Play mode → Open shop → See 3 random items?

**Done! All 12 items ready to test.** 🎉

---

## 🎨 **Add Icons Later (Optional)**

Use free asset packs:
- **Kenney Game Icons**: kenney.nl/assets/game-icons
- **Flaticon**: flaticon.com (credit required)
- **Quick Placeholders**: Colored squares (Red=HP, Blue=Armor, Green=Regen, Yellow=Speed)

Drag sprites into `Icon` field in Inspector.

---

## 🧪 **Quick Test Script**

Add to any GameObject for testing:

```csharp
using UnityEngine;

public class ItemTester : MonoBehaviour
{
    void Update()
    {
        // Press 1-9 to give chips
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            PlayerDataManager.Instance.AddChips(100);
            Debug.Log("Added 100 chips!");
        }

        // Press O to open shop
        if (Input.GetKeyDown(KeyCode.O))
        {
            GameStateManager.Instance.TransitionToOpenShop(ShopType.Items);
            Debug.Log("Opened item shop!");
        }
    }
}
```

Press `1` → Press `O` → Buy items → Check stats in `TowerRuntime` Inspector!

---

## 📊 **Expected Results**

### **Tank Build Test**
Buy:
- Monolith Core (+40% HP)
- Tempered Core (+20% HP)
- Hardened Seal (×3 stacks)

**Expected Stats:**
- MaxHp: ~160-180 (from 100 base)
- Armor: ~14 (from 5 base)

### **Speed Build Test**
Buy:
- Phase Bearings (×0.80)
- Light Frame (×5 stacks)
- Precision Bearings (×3 stacks)

**Expected Stats:**
- AttackSpeed: ~2.5-3.0 (from 1.0 base)
- Attack interval: ~0.33-0.40s

### **Spam Build Test**
Buy:
- Hydra Rack (+3 count)
- Rotary Magazine (+2 count)
- Spare Deck (×3 stacks)

**Expected Stats:**
- BaseAttackCount: ~9 (from 1 base)

---

## 🎯 **Balance Tuning Guide**

Too weak? → Increase `Value` or `Stack Bonus %`
Too strong? → Increase `Cost` or reduce `Max Stack`
Too common? → Change `Rarity` to Uncommon/Rare

**Tuning takes 5 seconds** (just change in Inspector, hit Play again).

---

## 🔄 **Iteration Workflow**

1. Play test for 2 minutes
2. Pause game
3. Select item asset
4. Change values in Inspector
5. Resume game (changes apply instantly!)
6. Repeat until balanced

**No recompile needed!** 🚀
