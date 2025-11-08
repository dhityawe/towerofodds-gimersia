// CardsSkill.cs
// Cards orbit around the tower; count based on BaseAttackCount, speed based on AttackSpeed

using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Cards Skill", menuName = "Tower/Skills/Cards", order = 2)]
public class CardsSkill : TowerSkill
{
    [Header("Cards Specific")]
    [Tooltip("Orbit radius around tower")]
    public float orbitRadius = 2f;
    [Tooltip("Base rotation speed (degrees per second)")]
    public float baseOrbitSpeed = 90f;

    // Runtime state per tower instance (keyed by tower instance ID)
    private Dictionary<int, List<CardOrbitData>> activeCards = new Dictionary<int, List<CardOrbitData>>();

    private class CardOrbitData
    {
        public GameObject cardObject;
        public float currentAngle;
        public int cardIndex;
    }

    public override void OnEquip(TowerRuntime tower, int slotIndex)
    {
        int instanceID = tower.GetInstanceID();
        if (!activeCards.ContainsKey(instanceID))
        {
            activeCards[instanceID] = new List<CardOrbitData>();
        }

        int cardCount = Mathf.Max(1, tower.GetStats().BaseAttackCount);
        float angleStep = 360f / cardCount;

        // Spawn cards
        for (int i = 0; i < cardCount; i++)
        {
            // TODO: Instantiate actual card prefab
            GameObject card = new GameObject($"Card_{slotIndex}_{i}");
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

        float orbitSpeed = baseOrbitSpeed * tower.GetStats().AttackSpeed;
        
        foreach (var cardData in activeCards[instanceID])
        {
            if (cardData.cardObject == null) continue;

            // Update angle
            cardData.currentAngle += orbitSpeed * deltaTime;
            if (cardData.currentAngle >= 360f) cardData.currentAngle -= 360f;

            // Calculate position
            float rad = cardData.currentAngle * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(
                Mathf.Cos(rad) * orbitRadius,
                0f,
                Mathf.Sin(rad) * orbitRadius
            );
            cardData.cardObject.transform.position = tower.transform.position + offset;

            // TODO: Check collision with enemies and apply damage
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

        Debug.Log($"[Cards] Unequipped from slot {slotIndex}");
    }

    public override void Activate(TowerRuntime tower, int slotIndex, int diceRoll1, int diceRoll2)
    {
        // Cards don't activate on attack cycle; they orbit continuously
        // Damage is applied via collision in OnUpdate
    }
}
