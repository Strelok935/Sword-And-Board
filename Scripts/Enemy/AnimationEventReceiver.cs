using UnityEngine;
using UnityEngine.AI;

public class AnimationEventReceiver : MonoBehaviour
{
private EnemyHealth enemyHealth;
private NavMeshAgent agent;

private void Awake()
{
// Find the EnemyHealth component on the parent GameObject
enemyHealth = GetComponentInParent<EnemyHealth>();
if (enemyHealth == null)
{
Debug.LogError("EnemyHealth component not found on parent GameObject!");
}

// Find the NavMeshAgent component on the parent GameObject
agent = GetComponentInParent<NavMeshAgent>();
if (agent == null)
{
Debug.LogError("NavMeshAgent component not found on parent GameObject!");
}
}

// Method to reset the DamageTrigger parameter
public void ResetDamageTrigger()
{
if (enemyHealth != null)
{
enemyHealth.ResetDamageTrigger();
}

// Restore the enemy's movement speed
if (agent != null)
{
agent.speed = 3.5f; // Restore the original speed
Debug.Log("Enemy movement restored.");
}
}

// Method to stop the enemy's movement
public void StopMovement()
{
if (agent != null)
{
agent.speed = 0; // Stop the enemy's movement
Debug.Log("Enemy movement stopped during hit animation.");
}
}
}