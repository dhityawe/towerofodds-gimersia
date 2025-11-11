using UnityEngine;

/// <summary>
/// Multi-shot skill that attacks multiple closest enemies at once.
/// Damage splits evenly between targets (or full damage per target based on settings).
/// </summary>
[CreateAssetMenu(fileName = "MultiShotSkill", menuName = "Tower/Skills/MultiShotSkill")]
public class MultiShotSkill : TowerSkill
{
    [Header("Multi-Shot Properties")]
    [SerializeField] private int numberOfShots = 3;
    [SerializeField] private float attackRange = 15f;
    [SerializeField] private bool splitDamage = false; // If true, damage is divided among targets
    
    [Header("Projectile")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 20f;

    public override void Activate(TowerRuntime tower, int slotIndex, int d1, int d2)
    {
        // Find multiple closest enemies
        var targets = tower.FindClosestEnemies(numberOfShots, attackRange);
        
        if (targets.Length == 0)
        {
            Debug.Log("MultiShot: No targets in range");
            return;
        }

        // Calculate base damage with dice
        float totalDamage = GetDamageWithDice(d1, d2);
        float damagePerTarget = splitDamage ? totalDamage / targets.Length : totalDamage;

        // Shoot at each target
        foreach (var target in targets)
        {
            if (target == null || !target.IsAlive) continue;

            if (projectilePrefab != null)
            {
                GameObject projectileObj = Instantiate(projectilePrefab, tower.transform.position, Quaternion.identity);
                var projectile = projectileObj.GetComponent<TowerOfOdds.Combat.Projectile>();
                
                if (projectile != null)
                {
                    projectile.Init(target.transform, damagePerTarget, projectileSpeed, false);
                }
                else
                {
                    Destroy(projectileObj);
                }
            }
            else
            {
                // Direct damage
                target.TakeDamage(damagePerTarget);
            }
        }

        string damageMode = splitDamage ? "split" : "full";
        Debug.Log($"MultiShot: Hit {targets.Length} targets with {damagePerTarget:F1} dmg each ({damageMode} mode)");
    }

    public override void OnEquip(TowerRuntime tower, int slotIndex)
    {
        Debug.Log($"MultiShotSkill equipped: {numberOfShots} shots per activation");
    }
}
