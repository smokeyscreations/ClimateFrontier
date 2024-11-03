using System;
using UnityEngine;
using UnityEngine.AI;

public abstract class BaseEnemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float pathUpdateInterval = 0.2f;
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float rotationSpeed = 8f;
    [SerializeField] private float baseAttackDamage = 10f;

    public NavMeshAgent navMeshAgent;
    public Animator animator;
    public Transform target;
    private float currentHealth;
    private float nextPathUpdateTime;

    // Static hash for the Idle animation trigger
    protected static readonly int IdleHash = Animator.StringToHash("Idle");

    // Properties (Getters and Setters)
    public float AttackRange
    {
        get => attackRange;
        set
        {
            attackRange = value;
            if (navMeshAgent != null)
            {
                navMeshAgent.stoppingDistance = attackRange;
            }
        }
    }

    public float PathUpdateInterval
    {
        get => pathUpdateInterval;
        set => pathUpdateInterval = value;
    }

    public float MaxHealth
    {
        get => maxHealth;
        set
        {
            maxHealth = value;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Ensure current health is within new max
        }
    }

    public float CurrentHealth
    {
        get => currentHealth;
        set => currentHealth = Mathf.Clamp(value, 0, maxHealth);
    }

    public float RotationSpeed
    {
        get => rotationSpeed;
        set => rotationSpeed = value;
    }

    public float BaseAttackDamage
    {
        get => baseAttackDamage;
        set => baseAttackDamage = value;
    }

    protected virtual void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;

        navMeshAgent.stoppingDistance = attackRange;
    }

    protected virtual void Start()
    {
        target = GameObject.FindWithTag("Player")?.transform;
    }

    protected virtual void Update()
    {
        if (target == null) return;

        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        bool inAttackRange = distanceToTarget <= attackRange;

        if (inAttackRange)
        {
            EnterAttackState();
        }
        else
        {
            ChaseTarget();
        }

        UpdateAnimationParameters(inAttackRange);
    }

    public virtual void UpdateAnimationParameters(bool inAttackRange)
    {
        animator.SetBool("Attack", inAttackRange);
        animator.SetFloat("speed", navMeshAgent.velocity.magnitude);
    }

    public virtual void EnterAttackState()
    {
        LookAtTarget();
        navMeshAgent.isStopped = true;
    }

    public virtual void ChaseTarget()
    {
        if (Time.time >= nextPathUpdateTime)
        {
            nextPathUpdateTime = Time.time + pathUpdateInterval;
            navMeshAgent.SetDestination(target.position);
        }
        navMeshAgent.isStopped = false;
    }

    public void LookAtTarget()
    {
        Vector3 lookDirection = target.position - transform.position;
        lookDirection.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    public virtual void TakeDamage(float amount)
    {
        CurrentHealth -= amount;
        if (currentHealth <= 0) Die();
    }

    public virtual void Die()
    {
        animator.SetTrigger("Die");
        navMeshAgent.isStopped = true;
        Destroy(gameObject, 2f);
    }

    public virtual void PerformAttack()
    {
        if (target != null && Vector3.Distance(transform.position, target.position) <= attackRange)
        {
            target.GetComponent<PlayerHealth>()?.TakeDamage(baseAttackDamage);
        }
    }

    public void StopMovement()
    {
        if (navMeshAgent != null)
        {
            navMeshAgent.isStopped = true;
            navMeshAgent.velocity = Vector3.zero;
        }
    }

    public void UpdateMovementSpeed(float speed)
    {
        if (navMeshAgent != null)
        {
            navMeshAgent.speed = speed;
        }
    }
}
