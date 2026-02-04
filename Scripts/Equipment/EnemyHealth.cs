using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] public int startingHealth = 100;
    public int currentHealth = 100;

    private Enemy enemy; // Reference to the Enemy component
    private NavMeshAgent agent; // Reference to the NavMeshAgent component

    void Awake()
    {
        currentHealth = startingHealth;
        enemy = GetComponent<Enemy>(); // Get the Enemy component
        agent = GetComponent<NavMeshAgent>(); // Get the NavMeshAgent component
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log($"Enemy is Taking {amount} damage. Current health: {currentHealth}");

        // Trigger the damage animation using the animator reference from the Enemy script
        if (enemy != null && enemy.animator != null)
        {
            enemy.animator.SetTrigger("Hit");

            
        }

        if (currentHealth <= 0)
        {
            Debug.Log("Enemy Died");
            if (enemy != null)
            {
                enemy.HandleDeath();
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    // Method to reset the DamageTrigger parameter and restore movement
    public void ResetDamageTrigger()
    {
        if (enemy != null && enemy.animator != null)
        {
            enemy.animator.ResetTrigger("Hit");
            Debug.Log("Hit reset.");

            // Restore the enemy's movement speed
            if (agent != null)
            {
                agent.speed = 3.5f; // Restore the original speed
                Debug.Log("Enemy movement restored.");
            }
        }
    }
}
