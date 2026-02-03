using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] public Animator animator; // Reference to the Animator component
    [SerializeField] protected float detectionRange = 10f; // Range within which the player is detected
    [SerializeField] protected Transform target; // Reference to the player's transform
    protected NavMeshAgent agent;
    protected EnemyHealth enemyHealth; // Reference to the EnemyHealth component

    protected enum EnemyState
    {
        Idle,
        Chase
    }

    protected EnemyState currentState;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        enemyHealth = GetComponent<EnemyHealth>(); // Get the EnemyHealth component
    }

    protected virtual void Start()
    {
        currentState = EnemyState.Idle;
        animator.SetBool("Spotted", false); // Ensure the Spotted parameter is set to false initially
    }

    protected virtual void Update()
    {
        // Check the distance between the enemy and the player
        float distanceToPlayer = Vector3.Distance(transform.position, target.position);

        // Check if the enemy's health is below 100 or the player is within detection range
        if (enemyHealth.currentHealth < enemyHealth.startingHealth || distanceToPlayer <= detectionRange)
        {
            // Switch to chase state
            SwitchState(EnemyState.Chase);
        }
        else
        {
            // Player is outside detection range and enemy health is full, switch to idle state
            SwitchState(EnemyState.Idle);
        }

        // If in chase state, set the agent's destination to the player's position
        if (currentState == EnemyState.Chase)
        {
            agent.SetDestination(target.position);
        }
    }

    protected virtual void SwitchState(EnemyState newState)
    {
        if (currentState != newState)
        {
            currentState = newState;

            switch (currentState)
            {
                case EnemyState.Idle:
                    animator.SetBool("Spotted", false); // Set Spotted to false for idle animation
                    agent.ResetPath(); // Stop the agent from moving
                    break;
                case EnemyState.Chase:
                    animator.SetBool("Spotted", true); // Set Spotted to true for chase animation
                    agent.SetDestination(target.position); // Set the agent's destination to the player's position
                    break;
            }
        }
    }
}
