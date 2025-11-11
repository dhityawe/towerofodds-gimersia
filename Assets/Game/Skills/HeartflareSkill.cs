// HeartflareSkill.cs
// Pink light pulses from small to max radius; pulse speed based on AttackSpeed

using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Heartflare Skill", menuName = "Tower/Skills/Heartflare", order = 3)]
public class HeartflareSkill : TowerSkill
{
    [Header("Heartflare Specific")]
    [Tooltip("Maximum pulse radius")]
    [SerializeField] private float maxRadius = 5f;
    
    [Tooltip("Minimum pulse radius (starting size)")]
    [SerializeField] private float minRadius = 0.5f;
    
    [Tooltip("Pulse expansion duration in seconds")]
    [SerializeField] private float pulseDuration = 0.8f;
    
    [Tooltip("Visual pulse prefab (optional)")]
    [SerializeField] private GameObject pulsePrefab;
    
    [Tooltip("Pulse color")]
    [SerializeField] private Color pulseColor = new Color(1f, 0.4f, 0.7f, 0.5f);
    
    [Tooltip("Cooldown between pulses in seconds (0 = no cooldown, uses attack speed only)")]
    [SerializeField] private float skillCooldown = 0f;

    // Runtime state per tower instance
    private Dictionary<int, List<PulseData>> activePulses = new Dictionary<int, List<PulseData>>();
    private Dictionary<int, int> currentDiceRoll1 = new Dictionary<int, int>();
    private Dictionary<int, int> currentDiceRoll2 = new Dictionary<int, int>();
    private Dictionary<int, float> cooldownTimers = new Dictionary<int, float>();

    private class PulseData
    {
        public GameObject pulseObject;
        public float currentRadius;
        public float elapsedTime;
        public float maxDuration;
        public HashSet<TowerOfOdds.Enemies.BaseEnemy> hitEnemies;
        public float damage;
        public Vector3 originalPrefabScale; // Store original prefab scale

        public PulseData(float duration, float damage)
        {
            currentRadius = 0f;
            elapsedTime = 0f;
            maxDuration = duration;
            hitEnemies = new HashSet<TowerOfOdds.Enemies.BaseEnemy>();
            this.damage = damage;
            originalPrefabScale = Vector3.one; // Default if no prefab
        }
    }

    public override void OnEquip(TowerRuntime tower, int slotIndex)
    {
        int instanceID = tower.GetInstanceID();
        if (!activePulses.ContainsKey(instanceID))
        {
            activePulses[instanceID] = new List<PulseData>();
        }

        // Initialize dice rolls
        if (!currentDiceRoll1.ContainsKey(instanceID))
        {
            currentDiceRoll1[instanceID] = 1;
            currentDiceRoll2[instanceID] = 1;
        }
        
        // Initialize cooldown timer (start ready)
        if (!cooldownTimers.ContainsKey(instanceID))
        {
            cooldownTimers[instanceID] = 0f;
        }

        Debug.Log($"[Heartflare] Equipped to slot {slotIndex}");
    }

    public override void OnUpdate(TowerRuntime tower, int slotIndex, float deltaTime)
    {
        int instanceID = tower.GetInstanceID();
        if (!activePulses.ContainsKey(instanceID)) return;

        // Update cooldown timer
        if (cooldownTimers.ContainsKey(instanceID) && cooldownTimers[instanceID] > 0f)
        {
            cooldownTimers[instanceID] -= deltaTime;
        }

        var pulsesToRemove = new List<PulseData>();

        foreach (var pulse in activePulses[instanceID])
        {
            // Update pulse expansion
            pulse.elapsedTime += deltaTime;
            float progress = Mathf.Clamp01(pulse.elapsedTime / pulse.maxDuration);
            
            // Lerp radius from min to max
            pulse.currentRadius = Mathf.Lerp(minRadius, maxRadius, progress);

            // Update visual size if pulse object exists
            if (pulse.pulseObject != null)
            {
                // Update scale based on radius and original prefab scale
                UpdatePulseScale(pulse, pulse.currentRadius);
                
                // Fade out alpha over time
                UpdatePulseVisual(pulse.pulseObject, 1f - progress);
            }

            // Check collision with enemies
            CheckPulseCollision(tower, pulse);

            // Remove pulse if duration expired
            if (pulse.elapsedTime >= pulse.maxDuration)
            {
                pulsesToRemove.Add(pulse);
            }
        }

        // Cleanup expired pulses
        foreach (var pulse in pulsesToRemove)
        {
            if (pulse.pulseObject != null)
            {
                Destroy(pulse.pulseObject);
            }
            activePulses[instanceID].Remove(pulse);
        }
    }

    public override void OnUnequip(TowerRuntime tower, int slotIndex)
    {
        int instanceID = tower.GetInstanceID();
        if (!activePulses.ContainsKey(instanceID)) return;

        // Cleanup all active pulses
        foreach (var pulse in activePulses[instanceID])
        {
            if (pulse.pulseObject != null)
            {
                Destroy(pulse.pulseObject);
            }
        }
        activePulses[instanceID].Clear();
        activePulses.Remove(instanceID);
        currentDiceRoll1.Remove(instanceID);
        currentDiceRoll2.Remove(instanceID);
        cooldownTimers.Remove(instanceID);

        Debug.Log($"[Heartflare] Unequipped from slot {slotIndex}");
    }

    public override void Activate(TowerRuntime tower, int slotIndex, int diceRoll1, int diceRoll2)
    {
        int instanceID = tower.GetInstanceID();
        
        // Check cooldown
        if (cooldownTimers.ContainsKey(instanceID) && cooldownTimers[instanceID] > 0f)
        {
            Debug.Log($"[Heartflare] On cooldown: {cooldownTimers[instanceID]:F2}s remaining");
            return;
        }
        
        // Update dice rolls
        currentDiceRoll1[instanceID] = diceRoll1;
        currentDiceRoll2[instanceID] = diceRoll2;
        
        // Calculate damage: BaseDamage * M
        float towerDiceMultiplier = TowerBase.GetDiceMultiplier(diceRoll1, diceRoll2);
        float damage = baseDamage * towerDiceMultiplier;

        // Create new pulse
        PulseData newPulse = new PulseData(pulseDuration, damage);

        // Spawn visual pulse object
        if (pulsePrefab != null)
        {
            newPulse.pulseObject = Instantiate(pulsePrefab, tower.transform.position, Quaternion.identity);
            newPulse.pulseObject.name = $"Heartflare_Pulse_{slotIndex}";
            
            // Store original prefab scale for reference (not used in current implementation)
            newPulse.originalPrefabScale = pulsePrefab.transform.localScale;
            
            // Set initial scale based on minRadius (in game units)
            UpdatePulseScale(newPulse, minRadius);
        }
        else
        {
            // Fallback: Create simple visual
            newPulse.pulseObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            newPulse.pulseObject.name = $"Heartflare_Pulse_{slotIndex}";
            newPulse.pulseObject.transform.position = tower.transform.position;
            
            // Start with minimal radius
            float initialDiameter = minRadius * 2f;
            newPulse.pulseObject.transform.localScale = new Vector3(initialDiameter, initialDiameter, initialDiameter);
            
            // Remove collider (we do manual collision)
            var collider = newPulse.pulseObject.GetComponent<Collider>();
            if (collider != null) Destroy(collider);
            
            // Apply color
            var renderer = newPulse.pulseObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = pulseColor;
            }
        }

        activePulses[instanceID].Add(newPulse);
        
        // Set cooldown
        if (skillCooldown > 0f)
        {
            cooldownTimers[instanceID] = skillCooldown;
        }

        Debug.Log($"[Heartflare] Pulse spawned | Damage: {damage:F1} | Tower Multiplier: {towerDiceMultiplier:F2} | Duration: {pulseDuration}s | Cooldown: {skillCooldown}s");
    }

    private void CheckPulseCollision(TowerRuntime tower, PulseData pulse)
    {
        // Get all enemies in range
        var allEnemies = tower.GetAllEnemiesInRange(maxRadius + 2f);

        foreach (var enemy in allEnemies)
        {
            if (enemy == null || !enemy.IsAlive) continue;
            if (pulse.hitEnemies.Contains(enemy)) continue; // Already hit by this pulse

            // Check if enemy is within current pulse radius
            float distance = Vector3.Distance(tower.transform.position, enemy.GetPosition());
            
            // Enemy is hit when pulse wave passes through (within a small threshold)
            float hitThreshold = 0.5f; // Tolerance for hit detection
            if (Mathf.Abs(distance - pulse.currentRadius) <= hitThreshold)
            {
                // Apply damage
                enemy.TakeDamage(pulse.damage);
                pulse.hitEnemies.Add(enemy);
                
                Debug.Log($"[Heartflare] Pulse hit {enemy.name} for {pulse.damage:F1} damage at radius {pulse.currentRadius:F1}");
            }
        }
    }

    private void UpdatePulseVisual(GameObject pulseObject, float alpha)
    {
        var renderer = pulseObject.GetComponent<Renderer>();
        if (renderer != null && renderer.material != null)
        {
            Color color = renderer.material.color;
            color.a = alpha * pulseColor.a;
            renderer.material.color = color;
        }
    }

    private void UpdatePulseScale(PulseData pulse, float radius)
    {
        if (pulse.pulseObject == null) return;

        // Calculate target diameter based on game units
        float diameter = radius * 2f;
        
        // Scale relative to original prefab scale
        // If prefab was 0.1, this maintains that proportion
        pulse.pulseObject.transform.localScale = new Vector3(
            pulse.originalPrefabScale.x * diameter,
            pulse.originalPrefabScale.y * diameter,
            pulse.originalPrefabScale.z * diameter
        );
    }
}
