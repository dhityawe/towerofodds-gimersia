using UnityEngine;
using System.Collections;

namespace TowerOfOdds.Manager
{
    public class EnemySpawner : MonoBehaviour
    {
        [Header("Enemy Prefabs")]
        [SerializeField] private GameObject meleePrefab;
        [SerializeField] private GameObject rangedPrefab;
        [SerializeField] private GameObject tankPrefab;

        [Header("Spawn Settings")]
        [SerializeField] private Transform[] spawnPoints;
        [SerializeField] private float spawnRadius = 20f;
        [SerializeField] private bool useRandomSpawnPoints = true;

        [Header("Current Spawn Info")]
        [SerializeField] private int enemiesSpawned;
        [SerializeField] private int targetEnemyCount;
        
        private WaveData currentWave;
        private Coroutine spawnCoroutine;

        private void Start()
        {
            // If no spawn points assigned, create a default circular spawn area
            if (spawnPoints == null || spawnPoints.Length == 0)
            {
                Debug.LogWarning("No spawn points assigned. Using circular spawn area.");
            }
        }

        public void StartWave(WaveData waveData)
        {
            currentWave = waveData;
            enemiesSpawned = 0;
            targetEnemyCount = waveData.TotalEnemies;

            if (spawnCoroutine != null)
            {
                StopCoroutine(spawnCoroutine);
            }

            spawnCoroutine = StartCoroutine(SpawnWaveCoroutine());
        }

        private IEnumerator SpawnWaveCoroutine()
        {
            float spawnInterval = 1f / currentWave.enemiesPerSecond;
            float waveEndTime = Time.time + currentWave.duration;

            Debug.Log($"Spawning {targetEnemyCount} enemies over {currentWave.duration}s (interval: {spawnInterval}s)");

            while (Time.time < waveEndTime && enemiesSpawned < targetEnemyCount)
            {
                SpawnEnemy();
                enemiesSpawned++;

                yield return new WaitForSeconds(spawnInterval);
            }

            Debug.Log($"Finished spawning wave. Total spawned: {enemiesSpawned}");
        }

        private void SpawnEnemy()
        {
            // Determine enemy type based on distribution
            Enemies.EnemyType enemyType = currentWave.distribution.GetRandomEnemyType();
            
            // Get appropriate prefab
            GameObject prefabToSpawn = GetEnemyPrefab(enemyType);
            
            if (prefabToSpawn == null)
            {
                Debug.LogError($"No prefab assigned for {enemyType} enemy!");
                return;
            }

            // Get spawn position
            Vector3 spawnPosition = GetSpawnPosition();

            // Spawn enemy
            GameObject enemy = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
            enemy.name = $"{enemyType}Enemy_{enemiesSpawned}";

            Debug.Log($"Spawned {enemyType} enemy at {spawnPosition}");
        }

        private GameObject GetEnemyPrefab(Enemies.EnemyType type)
        {
            switch (type)
            {
                case Enemies.EnemyType.Melee:
                    return meleePrefab;
                case Enemies.EnemyType.Ranged:
                    return rangedPrefab;
                case Enemies.EnemyType.Tank:
                    return tankPrefab;
                default:
                    return null;
            }
        }

        private Vector3 GetSpawnPosition()
        {
            if (useRandomSpawnPoints && spawnPoints != null && spawnPoints.Length > 0)
            {
                // Use predefined spawn points
                Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                return new Vector3(spawnPoint.position.x, spawnPoint.position.y, 0f);
            }
            else
            {
                // Spawn only on the perimeter (edge) of a circle around the spawner (2D)
                // Pick a random angle and place the spawn at exact radius along that angle.
                float angle = Random.Range(0f, Mathf.PI * 2f);
                Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                Vector2 spawnPos2D = new Vector2(transform.position.x, transform.position.y) + dir * spawnRadius;
                return new Vector3(spawnPos2D.x, spawnPos2D.y, 0f);
            }
        }

        private void OnDrawGizmosSelected()
        {
            // Visualize spawn radius
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, spawnRadius);

            // Draw spawn points
            if (spawnPoints != null)
            {
                Gizmos.color = Color.green;
                foreach (Transform point in spawnPoints)
                {
                    if (point != null)
                    {
                        Gizmos.DrawWireSphere(point.position, 1f);
                    }
                }
            }
        }
    }
}
