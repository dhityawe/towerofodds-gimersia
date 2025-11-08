using UnityEngine;

namespace TowerOfOdds.Combat
{
    /// <summary>
    /// Simple projectile that moves towards a target transform (2D) and applies damage on arrival.
    /// This implementation does a distance check and does not rely on Unity physics so it's easy to wire up.
    /// </summary>
    public class Projectile : MonoBehaviour
    {
        private Transform target;
        private float speed = 10f;
        private float damage = 1f;
        private bool targetIsTower = false;

        public void Init(Transform targetTransform, float damageAmount, float travelSpeed, bool isTargetTower)
        {
            target = targetTransform;
            damage = damageAmount;
            speed = travelSpeed;
            targetIsTower = isTargetTower;
        }

        private void Update()
        {
            if (target == null)
            {
                Destroy(gameObject);
                return;
            }

            Vector3 currentPos = transform.position;
            Vector3 targetPos = new Vector3(target.position.x, target.position.y, 0f);

            // Move towards target
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(currentPos, targetPos, step);

            // If we reached the target (or close enough), apply damage
            if (Vector3.Distance(transform.position, targetPos) <= 0.15f)
            {
                if (targetIsTower)
                {
                    Tower.Tower tower = target.GetComponent<Tower.Tower>();
                    if (tower != null)
                    {
                        tower.TakeDamage(damage);
                    }
                }
                else
                {
                    Enemies.BaseEnemy enemy = target.GetComponent<Enemies.BaseEnemy>();
                    if (enemy != null)
                    {
                        enemy.TakeDamage(damage);
                    }
                }

                Destroy(gameObject);
            }
        }
    }
}
