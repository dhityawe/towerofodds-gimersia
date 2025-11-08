using UnityEngine;

namespace TowerOfOdds.Utilities
{
    /// <summary>
    /// Debug helper untuk testing gameplay
    /// Attach ke GameManager atau object lain di scene
    /// Gunakan keyboard shortcuts untuk testing
    /// </summary>
    public class DebugCheatCodes : MonoBehaviour
    {
    [Header("Cheat Settings")]
    [SerializeField] private bool enableCheats = true;
    // [SerializeField] private float currencyCheatAmount = 1000f; // Disabled
    [SerializeField] private float healAmount = 500f;

    private Manager.GameManager gameManager;
    private Manager.WaveManager waveManager;
    private TowerRuntime tower;

    private void Start()
    {
        gameManager = Manager.GameManager.Instance;
        waveManager = Object.FindFirstObjectByType<Manager.WaveManager>();
        tower = Object.FindFirstObjectByType<TowerRuntime>();            if (enableCheats)
            {
                Debug.Log("=== CHEAT CODES ENABLED ===");
                // Debug.Log("M - Add Currency (+1000)"); // Disabled
                Debug.Log("H - Heal Tower (+500 HP)");
                Debug.Log("K - Kill All Enemies");
                Debug.Log("N - Skip to Next Wave");
                Debug.Log("T - Spawn Tank Enemy");
                // Debug.Log("1 - Upgrade Tower Damage"); // Disabled
                // Debug.Log("2 - Upgrade Tower Health"); // Disabled
                // Debug.Log("3 - Upgrade Tower Attack Speed"); // Disabled
                // Debug.Log("4 - Upgrade Tower Range"); // Disabled
                Debug.Log("========================");
            }
        }

        private void Update()
        {
            if (!enableCheats) return;

            // Currency disabled
            /*
            // Add Currency
            if (Input.GetKeyDown(KeyCode.M))
            {
                gameManager?.AddCurrency(currencyCheatAmount);
                Debug.Log($"CHEAT: Added {currencyCheatAmount} currency");
            }
            */

            // Heal Tower
            if (Input.GetKeyDown(KeyCode.H))
            {
                tower?.Heal(healAmount);
                Debug.Log($"CHEAT: Healed tower for {healAmount} HP");
            }

            // Kill All Enemies
            if (Input.GetKeyDown(KeyCode.K))
            {
                KillAllEnemies();
                Debug.Log("CHEAT: Killed all enemies");
            }

            // Skip to Next Wave
            if (Input.GetKeyDown(KeyCode.N))
            {
                if (waveManager != null && !waveManager.IsWaveActive)
                {
                    waveManager.StartNextWave();
                    Debug.Log("CHEAT: Skipped to next wave");
                }
            }

            // Spawn Tank Enemy for testing
            if (Input.GetKeyDown(KeyCode.T))
            {
                SpawnTestEnemy(Enemies.EnemyType.Tank);
                Debug.Log("CHEAT: Spawned Tank enemy");
            }

            // Upgrades disabled
            /*
            // Quick Upgrades
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                tower?.UpgradeAttackDamage(10f);
                Debug.Log("CHEAT: Tower damage +10");
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                tower?.UpgradeMaxHealth(100f);
                Debug.Log("CHEAT: Tower health +100");
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                tower?.UpgradeAttackSpeed(0.1f);
                Debug.Log("CHEAT: Tower attack speed +10%");
            }

            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                tower?.UpgradeRange(2f);
                Debug.Log("CHEAT: Tower range +2");
            }
            */

            // God Mode Toggle
            if (Input.GetKeyDown(KeyCode.G))
            {
                ToggleGodMode();
            }
        }

        private void KillAllEnemies()
        {
            Enemies.BaseEnemy[] enemies = Object.FindObjectsByType<Enemies.BaseEnemy>(FindObjectsSortMode.None);
            foreach (var enemy in enemies)
            {
                if (enemy.IsAlive)
                {
                    enemy.TakeDamage(999999f);
                }
            }
        }

        private void SpawnTestEnemy(Enemies.EnemyType type)
        {
            Manager.EnemySpawner spawner = Object.FindFirstObjectByType<Manager.EnemySpawner>();
            if (spawner == null) return;

            // Spawn at random position around tower
            Vector3 spawnPos = tower != null ? 
                tower.transform.position + Random.insideUnitSphere * 10f : 
                Vector3.zero;
            spawnPos.y = 0;

            // Note: Prefab spawning requires public access to EnemySpawner prefab fields
            // or reflection. For now, just log the spawn request.
            switch (type)
            {
                case Enemies.EnemyType.Melee:
                    Debug.Log($"Spawn Test Melee Enemy requested at {spawnPos}");
                    break;
                case Enemies.EnemyType.Tank:
                    Debug.Log($"Spawn Test Tank Enemy requested at {spawnPos}");
                    break;
            }
        }

        private void ToggleGodMode()
        {
            // Make tower invincible
            if (tower != null)
            {
                tower.AddMaxHp(999999f);
                tower.Heal(999999f);
                Debug.Log("CHEAT: God Mode - Tower health set to max");
            }
        }

        // Display cheats on screen
        private void OnGUI()
        {
            if (!enableCheats) return;

            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.fontSize = 12;
            style.normal.textColor = Color.yellow;

            string cheatInfo = "CHEAT CODES ACTIVE\n" +
                             // $"Currency: {gameManager?.Currency:F0}\n" + // Disabled
                             $"Wave: {waveManager?.CurrentWave}\n" +
                             $"Kills: {gameManager?.EnemiesKilled}\n" +
                             $"Tower HP: {tower?.GetCurrentHp():F0}/{tower?.GetStats().MaxHp:F0}";

            GUI.Label(new Rect(10, 10, 300, 100), cheatInfo, style);
        }
    }
}
