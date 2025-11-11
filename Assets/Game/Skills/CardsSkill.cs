// CardsSkill.cs
// Cards orbit around the tower; count based on BaseAttackCount, speed based on AttackSpeed

using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Cards Skill", menuName = "Tower/Skills/Cards", order = 2)]
public class CardsSkill : TowerSkill
{
    [Header("Cards Specific")]
    [Tooltip("Orbit radius around tower")]
    [SerializeField] private float orbitRadius = 2f;
    
    [Tooltip("Angular speed constant (k = 360° / 0.8f = 450)")]
    [SerializeField] private float angularSpeedConstant = 450f;
    
    [Tooltip("Card prefab (should have collider)")]
    [SerializeField] private GameObject cardPrefab;
    
    [Tooltip("Card collision radius for damage detection")]
    [SerializeField] private float cardCollisionRadius = 0.3f;
    
    [Tooltip("Damage tick interval in seconds")]
    [SerializeField] private float damageTickInterval = 0.2f;

    // Runtime state per tower instance (keyed by tower instance ID)
    private Dictionary<int, List<CardOrbitData>> activeCards = new Dictionary<int, List<CardOrbitData>>();
    private Dictionary<int, int> currentDiceRoll1 = new Dictionary<int, int>();
    private Dictionary<int, int> currentDiceRoll2 = new Dictionary<int, int>();

    private class CardOrbitData
    {
        public GameObject cardObject;
        public float currentAngle;
        public int cardIndex;
        public Dictionary<TowerOfOdds.Enemies.BaseEnemy, float> enemyDamageCooldowns;

        public CardOrbitData()
        {
            enemyDamageCooldowns = new Dictionary<TowerOfOdds.Enemies.BaseEnemy, float>();
        }
    }

    public override void OnEquip(TowerRuntime tower, int slotIndex)
    {
        int instanceID = tower.GetInstanceID();
        if (!activeCards.ContainsKey(instanceID))
        {
            activeCards[instanceID] = new List<CardOrbitData>();
        }

        // Initialize dice rolls
        if (!currentDiceRoll1.ContainsKey(instanceID))
        {
            currentDiceRoll1[instanceID] = 1;
            currentDiceRoll2[instanceID] = 1;
        }

        int cardCount = Mathf.Max(1, tower.GetStats().BaseAttackCount);
        float angleStep = 360f / cardCount;

        // Spawn cards
        for (int i = 0; i < cardCount; i++)
        {
            GameObject card;
            
            if (cardPrefab != null)
            {
                card = Instantiate(cardPrefab, tower.transform.position, Quaternion.identity);
                card.name = $"Card_{slotIndex}_{i}";
            }
            else
            {
                // Fallback: Create placeholder
                card = GameObject.CreatePrimitive(PrimitiveType.Cube);
                card.name = $"Card_{slotIndex}_{i}";
                card.transform.localScale = Vector3.one * 0.5f;
                
                // Remove collider from placeholder (we do manual collision)
                var collider = card.GetComponent<Collider>();
                if (collider != null) Destroy(collider);
            }
            
            card.transform.SetParent(tower.transform);
            
            CardOrbitData data = new CardOrbitData
            {
                cardObject = card,
                currentAngle = i * angleStep,
                cardIndex = i
            };
            activeCards[instanceID].Add(data);
        }

        Debug.Log($"[Cards] Equipped {cardCount} cards in slot {slotIndex}");
    }

    public override void OnUpdate(TowerRuntime tower, int slotIndex, float deltaTime)
    {
        int instanceID = tower.GetInstanceID();
        if (!activeCards.ContainsKey(instanceID)) return;

        // Calculate orbit angular speed: ω = k / AttackSpeed
        // Lower AttackSpeed = faster orbit (inverse relationship)
        float attackSpeed = tower.GetStats().AttackSpeed;
        float angularSpeed = angularSpeedConstant / Mathf.Max(0.1f, attackSpeed); // degrees per second
        
        // Get current dice multiplier
        float towerDiceMultiplier = TowerBase.GetDiceMultiplier(
            currentDiceRoll1[instanceID], 
            currentDiceRoll2[instanceID]
        );
        
        // Calculate damage per tick: DamagePerTick = BaseDamage * M
        float damagePerTick = baseDamage * towerDiceMultiplier;
        
        foreach (var cardData in activeCards[instanceID])
        {
            if (cardData.cardObject == null) continue;

            // Update angle
            cardData.currentAngle += angularSpeed * deltaTime;
            if (cardData.currentAngle >= 360f) cardData.currentAngle -= 360f;

            // Calculate position (2D orbit on XY plane)
            float rad = cardData.currentAngle * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(
                Mathf.Cos(rad) * orbitRadius,
                Mathf.Sin(rad) * orbitRadius,
                0f
            );
            cardData.cardObject.transform.position = tower.transform.position + offset;

            // Update damage cooldowns
            UpdateDamageCooldowns(cardData, deltaTime);

            // Check collision with enemies and apply damage
            CheckCardCollision(cardData, tower, damagePerTick);
        }
    }

    public override void OnUnequip(TowerRuntime tower, int slotIndex)
    {
        int instanceID = tower.GetInstanceID();
        if (!activeCards.ContainsKey(instanceID)) return;

        // Cleanup cards
        foreach (var cardData in activeCards[instanceID])
        {
            if (cardData.cardObject != null)
            {
                Destroy(cardData.cardObject);
            }
        }
        activeCards[instanceID].Clear();
        activeCards.Remove(instanceID);
        currentDiceRoll1.Remove(instanceID);
        currentDiceRoll2.Remove(instanceID);

        Debug.Log($"[Cards] Unequipped from slot {slotIndex}");
    }

    public override void Activate(TowerRuntime tower, int slotIndex, int diceRoll1, int diceRoll2)
    {
        // Update dice rolls for damage calculation
        int instanceID = tower.GetInstanceID();
        currentDiceRoll1[instanceID] = diceRoll1;
        currentDiceRoll2[instanceID] = diceRoll2;
        
        float towerDiceMultiplier = TowerBase.GetDiceMultiplier(diceRoll1, diceRoll2);
        Debug.Log($"[Cards] Dice updated: {diceRoll1}+{diceRoll2} = {towerDiceMultiplier:F2}x | Damage/tick: {baseDamage * towerDiceMultiplier:F1}");
    }

    private void UpdateDamageCooldowns(CardOrbitData cardData, float deltaTime)
    {
        // Decrease all cooldowns
        var keysToRemove = new List<TowerOfOdds.Enemies.BaseEnemy>();
        var keysToUpdate = new List<TowerOfOdds.Enemies.BaseEnemy>();

        foreach (var kvp in cardData.enemyDamageCooldowns)
        {
            if (kvp.Key == null || !kvp.Key.IsAlive)
            {
                keysToRemove.Add(kvp.Key);
            }
            else
            {
                keysToUpdate.Add(kvp.Key);
            }
        }

        // Remove dead/null enemies
        foreach (var key in keysToRemove)
        {
            cardData.enemyDamageCooldowns.Remove(key);
        }

        // Update cooldowns
        foreach (var key in keysToUpdate)
        {
            cardData.enemyDamageCooldowns[key] -= deltaTime;
            if (cardData.enemyDamageCooldowns[key] <= 0f)
            {
                cardData.enemyDamageCooldowns.Remove(key);
            }
        }
    }

    private void CheckCardCollision(CardOrbitData cardData, TowerRuntime tower, float damagePerTick)
    {
        // Find all enemies in range
        var allEnemies = tower.GetAllEnemiesInRange(orbitRadius + 5f); // Extra range for safety

        foreach (var enemy in allEnemies)
        {
            if (enemy == null || !enemy.IsAlive) continue;

            // Check distance
            float distance = Vector3.Distance(cardData.cardObject.transform.position, enemy.GetPosition());
            
            if (distance <= cardCollisionRadius)
            {
                // Check if we can damage this enemy (cooldown expired)
                if (!cardData.enemyDamageCooldowns.ContainsKey(enemy))
                {
                    // Apply damage
                    enemy.TakeDamage(damagePerTick);
                    
                    // Set cooldown
                    cardData.enemyDamageCooldowns[enemy] = damageTickInterval;
                    
                    Debug.Log($"[Cards] Card {cardData.cardIndex} hit {enemy.name} for {damagePerTick:F1} damage");
                }
            }
        }
    }
}
