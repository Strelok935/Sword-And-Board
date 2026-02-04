using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [Header("Idle Sound Randomness")]
    [SerializeField] private float minIdleDelay = 4f;
    [SerializeField] private float maxIdleDelay = 10f;

    private float idleTimer;
    private float nextIdleTime;

    [Header("Animation")]
    [SerializeField] private string deathAnimationState = "Death";
    [SerializeField] public Animator animator;

    [Header("Detection")]
    [SerializeField] protected float detectionRange = 10f;
    [SerializeField] protected Transform target;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip idleClip;
    [SerializeField] private AudioClip alertClip;
    [SerializeField] private AudioClip deathClip;

    protected NavMeshAgent agent;
    protected EnemyHealth enemyHealth;
    protected bool isDying;

    protected enum EnemyState { Idle, Chase }
    protected EnemyState currentState;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        enemyHealth = GetComponent<EnemyHealth>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        // Make audio 3D/local
        audioSource.spatialBlend = 1f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.minDistance = 2f;
        audioSource.maxDistance = 15f;
    }

    protected virtual void Start()
    {
        currentState = EnemyState.Idle;
        animator.SetBool("Spotted", false);
        ScheduleNextIdle();
    }

    protected virtual void Update()
    {
        if (isDying) return;

        float distanceToPlayer = Vector3.Distance(transform.position, target.position);

        if (enemyHealth.currentHealth < enemyHealth.startingHealth || distanceToPlayer <= detectionRange)
            SwitchState(EnemyState.Chase);
        else
            SwitchState(EnemyState.Idle);

        if (currentState == EnemyState.Chase)
            agent.SetDestination(target.position);

        HandleIdleAudio();
    }

    public void HandleIdleAudio()
    {
        if (currentState != EnemyState.Idle || audioSource.isPlaying) return;

        idleTimer += Time.deltaTime;

        if (idleTimer >= nextIdleTime)
        {
            if (idleClip != null)
                audioSource.PlayOneShot(idleClip);

            ScheduleNextIdle();
        }
    }

    void ScheduleNextIdle()
    {
        idleTimer = 0f;
        nextIdleTime = Random.Range(minIdleDelay, maxIdleDelay);
    }

    protected virtual void SwitchState(EnemyState newState)
    {
        if (currentState == newState) return;

        currentState = newState;

        switch (currentState)
        {
            case EnemyState.Idle:
                animator.SetBool("Spotted", false);
                agent.ResetPath();
                break;

            case EnemyState.Chase:
                animator.SetBool("Spotted", true);
                if (alertClip != null)
                    audioSource.PlayOneShot(alertClip);
                break;
        }
    }

    public virtual void HandleDeath()
    {
        if (isDying) return;
        isDying = true;

        agent.isStopped = true;
        agent.ResetPath();

        animator.ResetTrigger("Hit");
        animator.SetBool("Spotted", false);
        animator.SetTrigger("Die");

        if (deathClip != null)
            audioSource.PlayOneShot(deathClip);
        
        
    }

    
    public void OnDeathAnimationComplete()
{
    Destroy(gameObject);
}
}
