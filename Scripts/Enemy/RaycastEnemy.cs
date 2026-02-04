using UnityEngine;

public class RaycastEnemy : Enemy
{
    [Header("Shooting")]
    [SerializeField] private float shootRange = 12f;
    [SerializeField] private float shootCooldown = 1.25f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float inaccuracyAngle = 3f;
    [SerializeField] private Vector3 rayOriginOffset = new Vector3(0f, 1.5f, 0f);
    [SerializeField] private LayerMask hitMask = ~0;

    private float nextShotTime;

    protected override void Update()
    {
        if (isDying)
        {
            return;
        }

        if (target == null || enemyHealth == null)
        {
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, target.position);
        bool hasBeenHit = enemyHealth.currentHealth < enemyHealth.startingHealth;
        bool shouldEngage = hasBeenHit || distanceToPlayer <= detectionRange;

        if (shouldEngage)
        {
            SwitchState(EnemyState.Chase);
            TryShoot(distanceToPlayer, hasBeenHit);
        }
        else
        {
            SwitchState(EnemyState.Idle);
        }
    }

    protected override void SwitchState(EnemyState newState)
    {
        if (currentState == newState)
        {
            return;
        }

        currentState = newState;
        HandleIdleAudio();

        switch (currentState)
        {
            case EnemyState.Idle:
                animator.SetBool("Spotted", false);
                if (agent != null)
                {
                    agent.ResetPath();
                    agent.isStopped = true;
                }
                break;
            case EnemyState.Chase:
                animator.SetBool("Spotted", true);
                if (agent != null)
                {
                    agent.ResetPath();
                    agent.isStopped = true;
                }
                break;
        }
    }

    private void TryShoot(float distanceToPlayer, bool hasBeenHit)
    {
        if (Time.time < nextShotTime)
        {
            return;
        }

        if (!hasBeenHit && distanceToPlayer > shootRange)
        {
            return;
        }

        Vector3 origin = transform.position + rayOriginOffset;
        Vector3 toTarget = (target.position - origin).normalized;
        Vector3 direction = ApplyInaccuracy(toTarget);

        if (Physics.Raycast(origin, direction, out RaycastHit hit, shootRange, hitMask, QueryTriggerInteraction.Ignore))
        {
            if (hit.transform == target || hit.transform.IsChildOf(target))
            {
                PlayerStats playerStats = hit.transform.GetComponentInParent<PlayerStats>();
                if (playerStats != null)
                {
                    playerStats.TakeDamage(damage);
                }
            }
        }

        nextShotTime = Time.time + shootCooldown;
    }

    private Vector3 ApplyInaccuracy(Vector3 direction)
    {
        if (inaccuracyAngle <= 0f)
        {
            return direction;
        }

        Quaternion baseRotation = Quaternion.LookRotation(direction);
        Quaternion spread = Quaternion.Euler(
            Random.Range(-inaccuracyAngle, inaccuracyAngle),
            Random.Range(-inaccuracyAngle, inaccuracyAngle),
            0f);

        return baseRotation * spread * Vector3.forward;
    }
    
}
