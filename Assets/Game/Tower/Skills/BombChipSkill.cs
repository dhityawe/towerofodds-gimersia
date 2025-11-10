// BombChipSkill.cs
// Shoots a chip that explodes on enemy hit, damaging all enemies in radius

using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BombChip Skill", menuName = "Tower/Skills/BombChip", order = 4)]
public class BombChipSkill : TowerSkill
{
    [Header("BombChip Specific")]
    [Tooltip("Attack range to find target")]
    [SerializeField] private float attackRange = 15f;
    
    [Tooltip("Projectile prefab to spawn")]
    [SerializeField] private GameObject bombChipPrefab;
    
    [Tooltip("Projectile travel speed")]
    [SerializeField] private float projectileSpeed = 15f;
    
    [Tooltip("Explosion radius from hit point")]
    [SerializeField] private float explosionRadius = 2.5f;
    
    [Tooltip("Explosion visual effect prefab (optional)")]
    [SerializeField] private GameObject explosionEffectPrefab;
    
    [Tooltip("Explosion effect duration")]
    [SerializeField] private float explosionDuration = 0.3f;
    
    [Tooltip("Cooldown between attacks in seconds (0 = no cooldown, uses attack speed only)")]
    [SerializeField] private float skillCooldown = 0f;

    // Runtime cooldown tracking per tower instance
    private Dictionary<int, float> cooldownTimers = new Dictionary<int, float>();

    public override void OnEquip(TowerRuntime tower, int slotIndex)
    {
        int instanceID = tower.GetInstanceID();
        
        // Initialize cooldown timer (start ready)
        if (!cooldownTimers.ContainsKey(instanceID))
        {
            cooldownTimers[instanceID] = 0f;
        }

        Debug.Log($"[BombChip] Equipped to slot {slotIndex}");
    }

    public override void OnUpdate(TowerRuntime tower, int slotIndex, float deltaTime)
    {
        int instanceID = tower.GetInstanceID();
        
        // Update cooldown timer
        if (cooldownTimers.ContainsKey(instanceID) && cooldownTimers[instanceID] > 0f)
        {
            cooldownTimers[instanceID] -= deltaTime;
        }
    }

    public override void OnUnequip(TowerRuntime tower, int slotIndex)
    {
        int instanceID = tower.GetInstanceID();
        cooldownTimers.Remove(instanceID);

        Debug.Log($"[BombChip] Unequipped from slot {slotIndex}");
    }

    public override void Activate(TowerRuntime tower, int slotIndex, int diceRoll1, int diceRoll2)
    {
        int instanceID = tower.GetInstanceID();
        
        // Check cooldown
        if (cooldownTimers.ContainsKey(instanceID) && cooldownTimers[instanceID] > 0f)
        {
            Debug.Log($"[BombChip] On cooldown: {cooldownTimers[instanceID]:F2}s remaining");
            return;
        }
        
        // Get target enemy
        var targetEnemy = tower.FindClosestEnemy(attackRange);
        if (targetEnemy == null)
        {
            Debug.Log("[BombChip] No valid target");
            return;
        }

        // Calculate damage: BaseDamage * M
        float towerDiceMultiplier = TowerBase.GetDiceMultiplier(diceRoll1, diceRoll2);
        float directDamage = baseDamage * towerDiceMultiplier;
        float aoeDamage = baseDamage * towerDiceMultiplier; // Same as direct damage

        // Spawn bomb chip projectile
        GameObject projectileObj;
        if (bombChipPrefab != null)
        {
            projectileObj = Instantiate(bombChipPrefab, tower.transform.position, Quaternion.identity);
        }
        else
        {
            // Fallback: Create simple visual
            projectileObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            projectileObj.transform.localScale = Vector3.one * 0.3f;
            var renderer = projectileObj.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = new Color(1f, 0.5f, 0f); // Orange
            }
        }

        projectileObj.transform.position = tower.transform.position;
        projectileObj.name = "BombChip_Projectile";

        // Add BombChipProjectile component
        var bombChip = projectileObj.AddComponent<BombChipProjectile>();
        bombChip.Init(projectileSpeed, directDamage, targetEnemy.transform, tower, explosionRadius, aoeDamage, explosionEffectPrefab, explosionDuration);

        // Set cooldown
        if (skillCooldown > 0f)
        {
            cooldownTimers[instanceID] = skillCooldown;
        }

        Debug.Log($"[BombChip] Fired | Direct Hit: {directDamage:F1} | AoE: {aoeDamage:F1} | Radius: {explosionRadius} | Multiplier: {towerDiceMultiplier:F2} | Cooldown: {skillCooldown}s");
    }
}
