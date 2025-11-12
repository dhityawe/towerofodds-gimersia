using UnityEngine;
using System;

namespace TowerOfOdds.Manager
{
    [Serializable]
    public class WaveData
    {
        public int waveNumber;
        public float duration;              // Wave duration in seconds
        public float enemiesPerSecond;      // Spawn rate
        public EnemyDistribution distribution;
        
        public int TotalEnemies => Mathf.RoundToInt(duration * enemiesPerSecond);
    }

    [Serializable]
    public class EnemyDistribution
    {
        [Range(0f, 1f)] public float meleePercent;
        [Range(0f, 1f)] public float rangedPercent;
        [Range(0f, 1f)] public float tankPercent;

        public EnemyDistribution(float melee, float ranged, float tank)
        {
            // Normalize percentages
            float total = melee + ranged + tank;
            meleePercent = melee / total;
            rangedPercent = ranged / total;
            tankPercent = tank / total;
        }

        public Enemies.EnemyType GetRandomEnemyType()
        {
            float random = UnityEngine.Random.Range(0f, 1f);
            
            if (random < meleePercent)
                return Enemies.EnemyType.Melee;
            else if (random < meleePercent + rangedPercent)
                return Enemies.EnemyType.Ranged;
            else
                return Enemies.EnemyType.Tank;
        }
    }

    public class WaveManager : MonoBehaviour
    {
        [Header("Wave Settings")]
        [SerializeField] private int currentWave = 0;
        
        [Header("Current Wave Info")]
        [SerializeField] private WaveData currentWaveData;
        [SerializeField] private float waveTimer;
        [SerializeField] private bool waveActive = false;

        [Header("Scaling Parameters")]
        [SerializeField] private float baseDuration = 20f;           // Starting at 20 seconds
        [SerializeField] private float durationIncrement = 2f;       // +2 seconds per wave
        [SerializeField] private float maxDuration = 90f;            // Cap at 90 seconds
        [SerializeField] private float baseEnemiesPerSecond = 0.5f;  // Start with 0.5 enemies/sec
        [SerializeField] private float enemySpawnScaling = 0.05f;    // +0.05 per wave

        private EnemySpawner enemySpawner;
        private bool gameActive = true;

        public event Action<int> OnWaveStart;
        public event Action<int> OnWaveComplete;

        public int CurrentWave => currentWave;
        public WaveData CurrentWaveData => currentWaveData;
        public bool IsWaveActive => waveActive;

        private void Start()
        {
            enemySpawner = GetComponent<EnemySpawner>();
            if (enemySpawner == null)
            {
                Debug.LogError("EnemySpawner component not found on WaveManager!");
            }

            // Don't auto-start wave - GameStateManager controls this
        }

        private void Update()
        {
            if (!gameActive) return;

            if (waveActive)
            {
                waveTimer += Time.deltaTime;

                if (waveTimer >= currentWaveData.duration)
                {
                    EndWave();
                }
            }
        }

        public void StartNextWave()
        {
            currentWave++;
            currentWaveData = GenerateWaveData(currentWave);
            waveTimer = 0f;
            waveActive = true;

            Debug.Log($"=== WAVE {currentWave} START ===");
            Debug.Log($"Duration: {currentWaveData.duration}s");
            Debug.Log($"Spawn Rate: {currentWaveData.enemiesPerSecond}/s");
            Debug.Log($"Total Enemies: {currentWaveData.TotalEnemies}");
            Debug.Log($"Distribution - Melee: {currentWaveData.distribution.meleePercent:P0}, " +
                      $"Ranged: {currentWaveData.distribution.rangedPercent:P0}, " +
                      $"Tank: {currentWaveData.distribution.tankPercent:P0}");

            OnWaveStart?.Invoke(currentWave);

            if (enemySpawner != null)
            {
                enemySpawner.StartWave(currentWaveData);
            }
        }

        private void EndWave()
        {
            waveActive = false;

            Debug.Log($"=== WAVE {currentWave} COMPLETE ===");
            OnWaveComplete?.Invoke(currentWave);
        }

        private WaveData GenerateWaveData(int waveNumber)
        {
            WaveData data = new WaveData();
            data.waveNumber = waveNumber;

            // Calculate duration with cap
            data.duration = Mathf.Min(baseDuration + (waveNumber - 1) * durationIncrement, maxDuration);

            // Calculate enemies per second (increases each wave)
            data.enemiesPerSecond = baseEnemiesPerSecond + (waveNumber - 1) * enemySpawnScaling;

            // Calculate enemy distribution based on wave number
            data.distribution = CalculateEnemyDistribution(waveNumber);

            return data;
        }

        private EnemyDistribution CalculateEnemyDistribution(int wave)
        {
            // Wave 1-5: Mostly melee, some ranged
            // Wave 6-15: Balanced mix with more variety
            // Wave 16+: More tanks and challenging composition

            float melee, ranged, tank;

            if (wave <= 5)
            {
                // Early waves: Easy, mostly melee
                melee = 70f - (wave * 2f);      // 70% -> 60%
                ranged = 25f + (wave * 1.5f);   // 25% -> 32.5%
                tank = 5f + (wave * 0.5f);      // 5% -> 7.5%
            }
            else if (wave <= 15)
            {
                // Mid waves: Balanced
                melee = 50f - ((wave - 5) * 1f);     // 50% -> 40%
                ranged = 35f + ((wave - 5) * 0.5f);  // 35% -> 40%
                tank = 15f + ((wave - 5) * 0.5f);    // 15% -> 20%
            }
            else
            {
                // Late waves: Harder, more tanks
                melee = 35f - Mathf.Min((wave - 15) * 0.5f, 10f);  // 35% -> 25% (cap)
                ranged = 35f;                                       // 35% constant
                tank = 30f + Mathf.Min((wave - 15) * 0.5f, 10f);   // 30% -> 40% (cap)
            }

            return new EnemyDistribution(melee, ranged, tank);
        }

        public void StopWaves()
        {
            gameActive = false;
            waveActive = false;
        }

        public float GetWaveProgress()
        {
            if (!waveActive || currentWaveData == null) return 0f;
            return waveTimer / currentWaveData.duration;
        }
    }
}
