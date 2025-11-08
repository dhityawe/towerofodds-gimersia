using UnityEngine;

namespace TowerOfOdds.Scriptables
{
    /// <summary>
    /// ScriptableObject untuk menyimpan data stats enemy.
    /// Bisa di-edit langsung di Inspector dan reusable untuk variant enemy.
    /// </summary>
    [CreateAssetMenu(fileName = "NewEnemyData", menuName = "Tower of Odds/Enemy Data")]
    public class EnemyData : ScriptableObject
    {
        [Header("Basic Info")]
        public Enemies.EnemyType enemyType;
        public string enemyName;
        [TextArea(2, 4)]
        public string description;

        [Header("Combat Stats")]
        [Tooltip("Total HP - berapa banyak damage yang bisa diterima")]
        public float maxHealth = 50f;

        [Tooltip("Damage yang diberikan ke tower per hit")]
        public float attackDamage = 10f;

        [Tooltip("Jarak maksimal untuk attack tower")]
        public float attackRange = 2f;

        [Tooltip("Cooldown antar attack (dalam detik)")]
        public float attackCooldown = 1f;

        [Header("Movement")]
        [Tooltip("Kecepatan bergerak (units per second)")]
        public float moveSpeed = 3f;

        [Header("Special Behaviors")]
        [Tooltip("Ranged enemy: jarak ideal dari tower (kiting distance)")]
        public float preferredDistance = 8f;

        [Tooltip("Tank enemy: persentase damage reduction (0-1, dimana 0.2 = 20% reduction)")]
        [Range(0f, 1f)]
        public float damageReduction = 0f;

        [Header("Projectile (Ranged Enemy)")]
        [Tooltip("Prefab projectile untuk ranged attack (optional)")]
        public GameObject projectilePrefab;

        [Tooltip("Kecepatan projectile")]
        public float projectileSpeed = 15f;

        [Header("Visual")]
        [Tooltip("Warna untuk sprite enemy (bila menggunakan default sprite)")]
        public Color enemyColor = Color.white;

        /// <summary>
        /// Validasi data untuk memastikan tidak ada nilai invalid
        /// </summary>
        private void OnValidate()
        {
            maxHealth = Mathf.Max(1f, maxHealth);
            attackDamage = Mathf.Max(0f, attackDamage);
            attackRange = Mathf.Max(0.1f, attackRange);
            attackCooldown = Mathf.Max(0.1f, attackCooldown);
            moveSpeed = Mathf.Max(0.1f, moveSpeed);
            preferredDistance = Mathf.Max(0.1f, preferredDistance);
            projectileSpeed = Mathf.Max(1f, projectileSpeed);
        }
    }
}
