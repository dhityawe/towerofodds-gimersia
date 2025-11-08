using UnityEngine;

namespace TowerOfOdds.Utilities
{
    /// <summary>
    /// Helper class untuk visualisasi debugging
    /// Attach ke enemy/tower untuk melihat stats di scene view
    /// </summary>
    public class DebugVisualizer : MonoBehaviour
    {
        [SerializeField] private bool showHealthBar = true;
        [SerializeField] private bool showRange = true;
        [SerializeField] private Color healthBarColor = Color.green;
        [SerializeField] private Color rangeColor = Color.red;

        private Enemies.BaseEnemy enemy;
        private Tower.Tower tower;

        private void Start()
        {
            enemy = GetComponent<Enemies.BaseEnemy>();
            tower = GetComponent<Tower.Tower>();
        }

        private void OnDrawGizmos()
        {
            if (showHealthBar)
            {
                DrawHealthBar();
            }
        }

        private void DrawHealthBar()
        {
            float currentHealth = 0;
            float maxHealth = 1;

            if (enemy != null)
            {
                currentHealth = enemy.GetCurrentHealth();
                maxHealth = enemy.GetMaxHealth();
            }
            else if (tower != null)
            {
                currentHealth = tower.CurrentHealth;
                maxHealth = tower.MaxHealth;
            }

            if (maxHealth <= 0) return;

            float healthPercent = currentHealth / maxHealth;
            Vector3 position = transform.position + Vector3.up * 2f;

            // Background (2D - draw on XY plane)
            Gizmos.color = Color.black;
            Gizmos.DrawCube(position, new Vector3(1.1f, 0.2f, 0.01f));

            // Health bar (2D)
            Color color = Color.Lerp(Color.red, Color.green, healthPercent);
            Gizmos.color = color;
            Vector3 healthBarSize = new Vector3(healthPercent, 0.15f, 0.01f);
            Vector3 healthBarPosition = position - Vector3.right * (0.5f - healthPercent * 0.5f);
            Gizmos.DrawCube(healthBarPosition, healthBarSize);
        }
    }
}
