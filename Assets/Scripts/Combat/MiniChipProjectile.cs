using UnityEngine;

namespace TowerOfOdds.Combat
{
    /// <summary>
    /// Mini-chip projectile spawned from ChipProjectile split.
    /// Can either target a specific enemy or fly in a random direction.
    /// </summary>
    public class MiniChipProjectile : MonoBehaviour
    {
        private Transform target;
        private Vector3 direction;
        private float speed;
        private float damage;
        private bool hasTarget;
        private bool hasHit;
        private float lifetime;
        private float maxLifetime;

        /// <summary>
        /// Initialize mini-chip with a specific target.
        /// </summary>
        public void Init(Transform targetTransform, float damageAmount, float travelSpeed)
        {
            target = targetTransform;
            damage = damageAmount;
            speed = travelSpeed;
            hasTarget = true;
            maxLifetime = 3f; // 3 seconds max lifetime
        }

        /// <summary>
        /// Initialize mini-chip to fly in a random direction (no target).
        /// </summary>
        public void InitRandom(Vector3 flyDirection, float damageAmount, float travelSpeed, float lifeTime)
        {
            direction = flyDirection.normalized;
            damage = damageAmount;
            speed = travelSpeed;
            hasTarget = false;
            maxLifetime = lifeTime;
        }

        private void Update()
        {
            lifetime += Time.deltaTime;

            // Destroy after max lifetime
            if (lifetime >= maxLifetime)
            {
                Destroy(gameObject);
                return;
            }

            if (hasHit) return;

            if (hasTarget)
            {
                UpdateTargeted();
            }
            else
            {
                UpdateRandom();
            }
        }

        private void UpdateTargeted()
        {
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

        private void UpdateRandom()
        {
            // Fly in direction
            transform.position += direction * speed * Time.deltaTime;
        }

        private void OnHitEnemy()
        {
            if (hasHit) return;
            hasHit = true;

            Enemies.BaseEnemy enemy = target.GetComponent<Enemies.BaseEnemy>();
            if (enemy != null && enemy.IsAlive)
            {
                enemy.TakeDamage(damage);
                Debug.Log($"[MiniChip] Hit {enemy.name} for {damage:F1} damage");
            }

            Destroy(gameObject);
        }

        // Optional: Visual feedback for mini-chips
        private void OnDrawGizmos()
        {
            if (hasTarget && target != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, target.position);
            }
            else if (!hasTarget)
            {
                Gizmos.color = Color.gray;
                Gizmos.DrawRay(transform.position, direction);
            }
        }
    }
}
