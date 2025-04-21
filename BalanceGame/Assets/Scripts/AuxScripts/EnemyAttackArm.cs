using UnityEngine;

public class EnemyArm : MonoBehaviour
{
    public int damage=1;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();

            if (player != null)
            {
                player.TakeDamage(damage); // Player loses 1 HP
                Debug.Log("Player hit! Remaining HP: " + player.HP);

                // Optional: Push player a bit
                Vector2 pushDirection = (collision.transform.position - transform.parent.position).normalized;
                Rigidbody2D playerRb = collision.GetComponent<Rigidbody2D>();

                if (playerRb != null)
                {
                    playerRb.AddForce(pushDirection * 500f); // Adjust the force as needed
                }
            }
        }
    }
}
