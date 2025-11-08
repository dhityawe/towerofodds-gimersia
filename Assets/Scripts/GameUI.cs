using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TowerOfOdds.UI
{
    /// <summary>
    /// Simple UI controller untuk menampilkan informasi game
    /// Attach ke Canvas dengan UI elements
    /// </summary>
    public class GameUI : MonoBehaviour
    {
        [Header("Tower Info")]
        [SerializeField] private TextMeshProUGUI towerHealthText;
        [SerializeField] private Slider towerHealthBar;

        [Header("Wave Info")]
        [SerializeField] private TextMeshProUGUI waveNumberText;
        [SerializeField] private TextMeshProUGUI waveProgressText;
        [SerializeField] private Slider waveProgressBar;

        [Header("Game Stats")]
        // [SerializeField] private TextMeshProUGUI currencyText; // Disabled
        [SerializeField] private TextMeshProUGUI killCountText;
        [SerializeField] private TextMeshProUGUI enemyCountText;

        [Header("Wave Timer")]
        [SerializeField] private TextMeshProUGUI nextWaveTimerText;

        private Manager.GameManager gameManager;
        private Manager.WaveManager waveManager;
        private Tower.Tower tower;

        private void Start()
        {
            // Find references
            gameManager = Manager.GameManager.Instance;
            waveManager = Object.FindFirstObjectByType<Manager.WaveManager>();
            tower = Object.FindFirstObjectByType<Tower.Tower>();

            if (gameManager == null)
                Debug.LogWarning("GameManager not found!");
            if (waveManager == null)
                Debug.LogWarning("WaveManager not found!");
            if (tower == null)
                Debug.LogWarning("Tower not found!");
        }

        private void Update()
        {
            UpdateTowerInfo();
            UpdateWaveInfo();
            UpdateGameStats();
            UpdateNextWaveTimer();
        }

        private void UpdateTowerInfo()
        {
            if (tower == null) return;

            if (towerHealthText != null)
            {
                towerHealthText.text = $"HP: {tower.CurrentHealth:F0} / {tower.MaxHealth:F0}";
            }

            if (towerHealthBar != null)
            {
                towerHealthBar.maxValue = tower.MaxHealth;
                towerHealthBar.value = tower.CurrentHealth;
            }
        }

        private void UpdateWaveInfo()
        {
            if (waveManager == null) return;

            if (waveNumberText != null)
            {
                waveNumberText.text = $"Wave {waveManager.CurrentWave}";
            }

            if (waveManager.IsWaveActive)
            {
                float progress = waveManager.GetWaveProgress();
                
                if (waveProgressText != null)
                {
                    waveProgressText.text = $"Progress: {progress * 100:F0}%";
                }

                if (waveProgressBar != null)
                {
                    waveProgressBar.value = progress;
                }
            }
        }

        private void UpdateGameStats()
        {
            if (gameManager == null) return;

            // Currency disabled
            /*
            if (currencyText != null)
            {
                currencyText.text = $"Currency: {gameManager.Currency:F0}";
            }
            */

            if (killCountText != null)
            {
                killCountText.text = $"Kills: {gameManager.EnemiesKilled}";
            }

            if (enemyCountText != null)
            {
                int enemyCount = Object.FindObjectsByType<Enemies.BaseEnemy>(FindObjectsSortMode.None).Length;
                enemyCountText.text = $"Enemies: {enemyCount}";
            }
        }

        private void UpdateNextWaveTimer()
        {
            if (waveManager == null || nextWaveTimerText == null) return;

            if (!waveManager.IsWaveActive)
            {
                float progress = waveManager.GetTimeBetweenWavesProgress();
                float timeRemaining = (1f - progress) * 10f; // timeBetweenWaves = 10s
                
                nextWaveTimerText.text = $"Next Wave in: {timeRemaining:F1}s";
                nextWaveTimerText.gameObject.SetActive(true);
            }
            else
            {
                nextWaveTimerText.gameObject.SetActive(false);
            }
        }
    }
}
