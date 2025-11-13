using UnityEngine;

namespace TowerOfOdds.Enemies
{
    public enum EnemyType
    {
        Melee,
        Ranged,
        Tank
    }

    public abstract class BaseEnemy : MonoBehaviour
    {
        [Header("Enemy Data")]
        [SerializeField] protected Scriptables.EnemyData enemyData;

        [Header("Runtime Stats (Read-only)")]
        [SerializeField] protected float maxHealth;
        [SerializeField] protected float currentHealth;
        [SerializeField] protected float moveSpeed;
        [SerializeField] protected float attackDamage;
        [SerializeField] protected float attackRange;
        [SerializeField] protected float attackCooldown;
        [SerializeField] protected EnemyType enemyType;

        [Header("References")]
        protected Transform tower;
        protected float lastAttackTime;
        protected bool isAlive = true;

        public EnemyType Type => enemyType;
        public bool IsAlive => isAlive;

        protected virtual void Start()
        {
            LoadStatsFromData();
            currentHealth = maxHealth;
            FindTower();
            ApplyVisuals();
        }

        /// <summary>
        /// Load stats dari EnemyData ScriptableObject.
        /// Bisa di-override untuk custom behavior.
        /// </summary>
        protected virtual void LoadStatsFromData()
        {
            if (enemyData == null)
            {
                Debug.LogError($"EnemyData not assigned on {gameObject.name}! Using default values.");
                return;
            }

            maxHealth = enemyData.maxHealth;
            attackDamage = enemyData.attackDamage;
            attackRange = enemyData.attackRange;
            attackCooldown = enemyData.attackCooldown;
            moveSpeed = enemyData.moveSpeed;
            enemyType = enemyData.enemyType;
        }

        /// <summary>
        /// Apply visual dari EnemyData (warna sprite, dll)
        /// </summary>
        protected virtual void ApplyVisuals()
        {
            if (enemyData == null) return;

            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.color = enemyData.enemyColor;
            }
        }

        protected virtual void Update()
        {
            if (!isAlive || tower == null) return;

            float distanceToTower = Vector2.Distance(
                new Vector2(transform.position.x, transform.position.y),
                new Vector2(tower.position.x, tower.position.y)
            );

            if (distanceToTower <= attackRange)
            {
                AttackBehavior();
            }
            else
            {
                MovementBehavior();
            }
        }

        protected virtual void MovementBehavior()
        {
            if (tower != null)
            {
                Vector2 currentPos = new Vector2(transform.position.x, transform.position.y);
                Vector2 towerPos = new Vector2(tower.position.x, tower.position.y);
                Vector2 direction = (towerPos - currentPos).normalized;
                
                Vector2 movement = direction * moveSpeed * Time.deltaTime;
                transform.position = new Vector3(
                    transform.position.x + movement.x,
                    transform.position.y + movement.y,
                    0f
                );
            }
        }

        protected virtual void AttackBehavior()
        {
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                Attack();
                lastAttackTime = Time.time;
            }
        }

    protected virtual void Attack()
    {
        TowerRuntime towerComponent = tower.GetComponent<TowerRuntime>();
        if (towerComponent != null)
        {
            towerComponent.ApplyDamage(attackDamage);
            Debug.Log($"{enemyType} enemy attacked tower for {attackDamage} damage!");
        }
    }        public virtual void TakeDamage(float damage)
        {
            if (!isAlive) return;

            currentHealth -= damage;

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        protected virtual void Die()
        {
            isAlive = false;
            
            Manager.GameManager gameManager = Object.FindFirstObjectByType<Manager.GameManager>();
            if (gameManager != null)
            {
                gameManager.OnEnemyKilled(enemyType);
            }
            
            // Notify WaveManager that an enemy died
            TowerOfOdds.Manager.WaveManager waveManager = Object.FindFirstObjectByType<TowerOfOdds.Manager.WaveManager>();
            if (waveManager != null)
            {
                waveManager.OnEnemyDied();
            }
            
            // Increment kill counter in PlayerDataManager
            PlayerDataManager playerData = PlayerDataManager.Instance;
            if (playerData != null)
            {
                playerData.IncrementKills();
            }
            
            Destroy(gameObject, 0.1f);
        }

        protected void FindTower()
        {
            GameObject towerObj = GameObject.FindGameObjectWithTag("Tower");
            if (towerObj != null)
            {
                tower = towerObj.transform;
            }
            else
            {
                Debug.LogWarning("Tower not found! Make sure tower has 'Tower' tag.");
            }
        }

        public float GetCurrentHealth() => currentHealth;
        public float GetMaxHealth() => maxHealth;
        public Vector3 GetPosition() => transform.position;
    }
}
