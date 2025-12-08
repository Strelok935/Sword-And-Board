using UnityEngine;

public class EnemyHealth : MonoBehaviour
{

    [SerializeField] public int startingHealth = 100;
    public int currentHealth = 100;

    void Awake()
    {
        currentHealth = startingHealth;

    }


    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log($"Enemy is Taking {amount} damage. Current health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Debug.Log("Enemy Died");
            Destroy(gameObject);
        }
    }



   
}
