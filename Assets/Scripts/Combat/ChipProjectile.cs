using UnityEngine;
using System.Collections.Generic;

namespace TowerOfOdds.Combat
{
    /// <summary>
    /// Chip projectile that splits into mini-chips on first enemy hit.
    /// Only splits once, and each enemy can only be hit by one mini-chip.
    /// </summary>
    public class ChipProjectile : MonoBehaviour
    {
        private Transform target;
        private float speed;
        private float primaryDamage;
        private float miniChipDamage;
        private int splitCount;
        private GameObject miniChipPrefab;
        private bool hasHit;
        private TowerRuntime sourceTower;
        private AudioClip hitSound;
        private float hitSoundVolume = 1f;

        /// <summary>
        /// Initialize the chip projectile.
        /// </summary>
        /// <param name="targetTransform">Initial target enemy</param>
        /// <param name="primaryDmg">Damage for first hit</param>
        /// <param name="miniDmg">Damage for each mini-chip</param>
        /// <param name="splits">Number of mini-chips to spawn on hit</param>
        /// <param name="travelSpeed">Projectile speed</param>
        /// <param name="miniPrefab">Prefab for mini-chip projectiles</param>
        /// <param name="tower">Source tower reference for enemy finding</param>
        /// <param name="hitAudio">Audio clip to play on hit</param>
        /// <param name="audioVolume">Volume for hit sound</param>
        public void Init(Transform targetTransform, float primaryDmg, float miniDmg, int splits, 
                        float travelSpeed, GameObject miniPrefab, TowerRuntime tower, 
                        AudioClip hitAudio = null, float audioVolume = 1f)
        {
            target = targetTransform;
            primaryDamage = primaryDmg;
            miniChipDamage = miniDmg;
            splitCount = Mathf.Max(1, splits);
            speed = travelSpeed;
            miniChipPrefab = miniPrefab;
            sourceTower = tower;
            hitSound = hitAudio;
            hitSoundVolume = audioVolume;
        }

        private void Update()
        {
            if (hasHit) return; // Already hit and splitting

            if (target == null)
            {
                Destroy(gameObject);
                return;
            }

            // Move towards target
            Vector3 currentPos = transform.position;
            Vector3 targetPos = new Vector3(target.position.x, target.position.y, 0f);

            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(currentPos, targetPos, step);

            // Check if reached target
            if (Vector3.Distance(transform.position, targetPos) <= 0.15f)
            {
                OnHitEnemy();
            }
        }

        private void OnHitEnemy()
        {
            if (hasHit) return; // Prevent multiple hits
            hasHit = true;

            // Play hit sound
            if (hitSound != null)
            {
                AudioSource.PlayClipAtPoint(hitSound, transform.position, hitSoundVolume);
            }

            // Apply primary damage
            Enemies.BaseEnemy enemy = target.GetComponent<Enemies.BaseEnemy>();
            if (enemy != null && enemy.IsAlive)
            {
                enemy.TakeDamage(primaryDamage);
                Debug.Log($"[ChipProjectile] Primary hit on {enemy.name} for {primaryDamage:F1} damage");

                // Split into mini-chips
                SpawnMiniChips(enemy);
            }

            // Destroy main chip
            Destroy(gameObject);
        }

        private void SpawnMiniChips(Enemies.BaseEnemy hitEnemy)
        {
            if (miniChipPrefab == null)
            {
                Debug.LogWarning("[ChipProjectile] No mini-chip prefab assigned!");
                return;
            }

            if (sourceTower == null)
            {
                Debug.LogWarning("[ChipProjectile] No source tower reference!");
                return;
            }

            // Find nearby enemies (excluding the one we just hit)
            var nearbyEnemies = FindNearbyEnemies(hitEnemy, splitCount, 15f);

            // If no valid targets found, don't spawn any mini-chips
            if (nearbyEnemies.Count == 0)
            {
                Debug.Log($"[ChipProjectile] No other enemies in range, skipping mini-chip spawn");
                return;
            }

            Debug.Log($"[ChipProjectile] Splitting into {nearbyEnemies.Count} mini-chips | Found {nearbyEnemies.Count} valid targets");

            // Spawn mini-chips only for valid targets (no random direction spawns)
            for (int i = 0; i < nearbyEnemies.Count; i++)
            {
                Vector3 spawnPos = transform.position;
                SpawnMiniChipToTarget(spawnPos, nearbyEnemies[i]);
            }
        }

        private void SpawnMiniChipToTarget(Vector3 spawnPos, Enemies.BaseEnemy targetEnemy)
        {
            GameObject miniChipObj = Instantiate(miniChipPrefab, spawnPos, Quaternion.identity);
            var miniChip = miniChipObj.GetComponent<MiniChipProjectile>();

            if (miniChip != null)
            {
                miniChip.Init(targetEnemy.transform, miniChipDamage, speed * 1.2f); // Mini-chips slightly faster
                Debug.Log($"[ChipProjectile] Mini-chip → {targetEnemy.name} ({miniChipDamage:F1} dmg)");
            }
            else
            {
                Debug.LogWarning("[ChipProjectile] Mini-chip prefab missing MiniChipProjectile component!");
                Destroy(miniChipObj);
            }
        }

        /// <summary>
        /// Find nearby enemies, excluding the one already hit.
        /// Each enemy can only be targeted once.
        /// </summary>
        private List<Enemies.BaseEnemy> FindNearbyEnemies(Enemies.BaseEnemy excludeEnemy, int maxCount, float searchRange)
        {
            var result = new List<Enemies.BaseEnemy>();
            var allEnemies = sourceTower.GetAllEnemiesInRange(searchRange);

            // Sort by distance from hit point
            var sorted = new List<(Enemies.BaseEnemy enemy, float distance)>();
            foreach (var enemy in allEnemies)
            {
                if (enemy == excludeEnemy) continue; // Skip the enemy we just hit
                if (!enemy.IsAlive) continue;

                float distance = Vector3.Distance(transform.position, enemy.GetPosition());
                sorted.Add((enemy, distance));
            }

            // Sort by distance (closest first)
            sorted.Sort((a, b) => a.distance.CompareTo(b.distance));

            // Take up to maxCount
            for (int i = 0; i < Mathf.Min(maxCount, sorted.Count); i++)
            {
                result.Add(sorted[i].enemy);
            }

            return result;
        }
    }
}
