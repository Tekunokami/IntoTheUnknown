using UnityEngine;

public class Hazard : MonoBehaviour
{
    public float damageAmount = 20f; 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
                Debug.Log("You step on a spike! Remaining Health: " + playerHealth.currentHealth);
            }
        }
    }
}