using UnityEngine;

public class PlayerAttackArm : MonoBehaviour
{
    public float pushForce = 200f;
    public float damage = 1f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            // Try to get the EnemyController script from the enemy
            EnemyController enemy = collision.GetComponentInParent<EnemyController>();
            if (enemy != null)
            {
                // Deal damage to the enemy
                enemy.TakeDamage(damage);

                // Push the enemy away from the player
                Vector2 pushDirection = (collision.transform.position - transform.root.position).normalized;
                Rigidbody2D enemyRb = collision.GetComponentInParent<Rigidbody2D>();
                if (enemyRb != null)
                {
                    enemyRb.AddForce(pushDirection * pushForce, ForceMode2D.Impulse);
                }
            }
        }
    }
}
