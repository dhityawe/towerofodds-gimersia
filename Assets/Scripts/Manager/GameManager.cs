using UnityEngine;
using UnityEngine.SceneManagement;

namespace TowerOfOdds.Manager
{
    public enum GameState
    {
        Menu,
        Playing,
        Paused,
        GameOver,
        Victory
    }

    public class GameManager : MonoBehaviour
    {
    [Header("Game State")]
    [SerializeField] private GameState currentState = GameState.Playing;

    [Header("References")]
    [SerializeField] private TowerRuntime tower;
    [SerializeField] private WaveManager waveManager;        [Header("Game Stats")]
        [SerializeField] private int enemiesKilled;
        [SerializeField] private int highestWave;
        [SerializeField] private float playTime;

        // Currency & Upgrade system disabled for now
        // [Header("Idle/Upgrade System")]
        // [SerializeField] private float currency;
        // [SerializeField] private float currencyPerKill = 10f;

        public static GameManager Instance { get; private set; }

        public GameState CurrentState => currentState;
        public int EnemiesKilled => enemiesKilled;
        public int HighestWave => highestWave;
        // public float Currency => currency; // Disabled

        private void Awake()
        {
            // Singleton pattern
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        private void Start()
        {
            InitializeGame();
        }

        private void Update()
        {
            if (currentState == GameState.Playing)
            {
                playTime += Time.deltaTime;
            }
        }

    private void InitializeGame()
    {
        // Find tower if not assigned
        if (tower == null)
        {
            tower = Object.FindFirstObjectByType<TowerRuntime>();
        }

        // Find wave manager if not assigned
        if (waveManager == null)
        {
            waveManager = Object.FindFirstObjectByType<WaveManager>();
        }            // Subscribe to events
            if (waveManager != null)
            {
                waveManager.OnWaveStart += OnWaveStarted;
                waveManager.OnWaveComplete += OnWaveCompleted;
            }

            currentState = GameState.Playing;
            Debug.Log("Game Initialized!");
        }

        private void OnWaveStarted(int waveNumber)
        {
            Debug.Log($"GameManager: Wave {waveNumber} started");
            
            if (waveNumber > highestWave)
            {
                highestWave = waveNumber;
            }
        }

        private void OnWaveCompleted(int waveNumber)
        {
            Debug.Log($"GameManager: Wave {waveNumber} completed");
            
            // Currency system disabled
            // Give wave completion bonus
            // float waveBonus = waveNumber * 50f;
            // AddCurrency(waveBonus);
            // Debug.Log($"Wave {waveNumber} bonus: {waveBonus} currency");
        }

        public void OnEnemyKilled(Enemies.EnemyType enemyType)
        {
            enemiesKilled++;
            
            // Currency system disabled
            /*
            // Give currency based on enemy type
            float reward = currencyPerKill;
            switch (enemyType)
            {
                case Enemies.EnemyType.Melee:
                    reward = currencyPerKill;
                    break;
                case Enemies.EnemyType.Ranged:
                    reward = currencyPerKill * 1.5f;
                    break;
                case Enemies.EnemyType.Tank:
                    reward = currencyPerKill * 3f;
                    break;
            }

            AddCurrency(reward);
            Debug.Log($"Enemy killed! Reward: {reward}. Total kills: {enemiesKilled}");
            */
            
            Debug.Log($"{enemyType} enemy killed! Total kills: {enemiesKilled}");
        }

        public void OnTowerDestroyed()
        {
            currentState = GameState.GameOver;
            
            if (waveManager != null)
            {
                waveManager.StopWaves();
            }

            Debug.Log("=== GAME OVER ===");
            Debug.Log($"Survived {highestWave} waves");
            Debug.Log($"Killed {enemiesKilled} enemies");
            Debug.Log($"Play time: {playTime:F1}s");

            // You can trigger UI game over screen here
        }

        /* Currency & Upgrade System - DISABLED
        public void AddCurrency(float amount)
        {
            currency += amount;
            Debug.Log($"Currency earned: +{amount}. Total: {currency}");
        }

        public bool SpendCurrency(float amount)
        {
            if (currency >= amount)
            {
                currency -= amount;
                Debug.Log($"Currency spent: -{amount}. Remaining: {currency}");
                return true;
            }
            
            Debug.LogWarning($"Not enough currency! Need: {amount}, Have: {currency}");
            return false;
        }

        #region Tower Upgrades
        
        public void UpgradeTowerHealth(float cost, float amount)
        {
            if (SpendCurrency(cost) && tower != null)
            {
                tower.UpgradeMaxHealth(amount);
            }
        }

        public void UpgradeTowerDamage(float cost, float amount)
        {
            if (SpendCurrency(cost) && tower != null)
            {
                tower.UpgradeAttackDamage(amount);
            }
        }

        public void UpgradeTowerAttackSpeed(float cost, float reductionPercent)
        {
            if (SpendCurrency(cost) && tower != null)
            {
                tower.UpgradeAttackSpeed(reductionPercent);
            }
        }

        public void UpgradeTowerRange(float cost, float amount)
        {
            if (SpendCurrency(cost) && tower != null)
            {
                tower.UpgradeRange(amount);
            }
        }

        #endregion
        */

        #region Game Control

        public void PauseGame()
        {
            if (currentState == GameState.Playing)
            {
                currentState = GameState.Paused;
                Time.timeScale = 0f;
                Debug.Log("Game Paused");
            }
        }

        public void ResumeGame()
        {
            if (currentState == GameState.Paused)
            {
                currentState = GameState.Playing;
                Time.timeScale = 1f;
                Debug.Log("Game Resumed");
            }
        }

        public void RestartGame()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void QuitGame()
        {
            Debug.Log("Quitting game...");
            Application.Quit();
        }

        #endregion

        private void OnDestroy()
        {
            // Unsubscribe from events
            if (waveManager != null)
            {
                waveManager.OnWaveStart -= OnWaveStarted;
                waveManager.OnWaveComplete -= OnWaveCompleted;
            }
        }
    }
}
