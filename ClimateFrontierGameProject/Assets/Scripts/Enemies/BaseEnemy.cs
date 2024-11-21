using System;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using EnemyStates;
public abstract class BaseEnemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float baseAttackDamage = 10f;
    [SerializeField] private float pathUpdateInterval = 0.2f;

    public int experienceAmount = 10;

    public NavMeshAgent navMeshAgent;
    public Animator animator;
    public Transform Target { get; protected set; }
    private float currentHealth;
    private PlayerHealth playerHealth;

    private float pathUpdateTimer;
    private bool isDead = false;

    protected StateMachine stateMachine;
    protected IState chaseState;
    protected IState attackState;

    public event Action<BaseEnemy> OnEnemyDeath;

    protected virtual void OnEnable()
    {
        // Reset health and state
        ResetEnemy();
    }

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

    protected virtual void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;

        navMeshAgent.stoppingDistance = attackRange;

        InitializeStateMachine();
    }

    protected virtual void Start()
    {
        Target = GameObject.FindWithTag("Player")?.transform;
        playerHealth = Target?.GetComponent<PlayerHealth>();

        if (Target == null)
        {
            Debug.LogError($"{gameObject.name}: Player target not found. Ensure the player GameObject has the tag 'Player'.");
        }
    }

    protected virtual void Update()
    {
        if (isDead) return;
        stateMachine.Tick();
    }

    protected virtual void InitializeStateMachine()
    {
        stateMachine = new StateMachine();

        chaseState = new EnemyChaseState(this, navMeshAgent, animator);
        attackState = new EnemyAttackState(this, animator);

        stateMachine.AddTransition(chaseState, attackState, PlayerInAttackRange);
        stateMachine.AddTransition(attackState, chaseState, PlayerOutOfAttackRange);

        stateMachine.SetState(chaseState);
    }

    protected Func<bool> PlayerInAttackRange => () => Target != null && Vector3.Distance(transform.position, Target.position) <= AttackRange;
    protected Func<bool> PlayerOutOfAttackRange => () => Target != null && Vector3.Distance(transform.position, Target.position) > AttackRange;

    public void LookAtTarget()
    {
        if (Target == null) return;

        Vector3 lookDirection = Target.position - transform.position;
        lookDirection.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * navMeshAgent.angularSpeed);
    }

    public virtual void PerformAttack()
    {
        if (Target != null && Vector3.Distance(transform.position, Target.position) <= attackRange)
        {
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(baseAttackDamage);
                Debug.Log($"{gameObject.name} attacks the player for {baseAttackDamage} damage.");
            }
        }
    }

    public virtual void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        if (isDead) return;
        isDead = true;

        // Notify listeners that this enemy has died
        OnEnemyDeath?.Invoke(this);

        // Play death animation
        animator.SetTrigger("Die");

        // Disable components to prevent further interaction
        if (navMeshAgent != null)
        {
            navMeshAgent.enabled = false;
        }

        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = false;
        }

        // Spawn experience orb
        SpawnExperienceOrb();

        // Return to pool after a short delay to allow the death animation to play
        StartCoroutine(ReturnToPoolAfterDelay(1f));
    }

    private IEnumerator ReturnToPoolAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        EnemyPool.Instance.ReturnToPool(this);
    }

    private void SpawnExperienceOrb()
    {
        GameObject orb = ExperienceOrbPool.Instance.GetOrb();
        orb.transform.position = transform.position;

        ExperienceOrb expOrb = orb.GetComponent<ExperienceOrb>();
        if (expOrb != null)
        {
            expOrb.experienceAmount = experienceAmount;
        }
    }

    public void MoveToTarget()
    {
        if (Time.time >= pathUpdateTimer)
        {
            pathUpdateTimer = Time.time + pathUpdateInterval;
            if (Target != null)
                navMeshAgent.SetDestination(Target.position);
        }
        navMeshAgent.isStopped = false;
    }

    public bool IsDead => isDead;

    public void ResetEnemy()
    {
        // Reset health and state
        isDead = false;
        currentHealth = maxHealth;

        // Reset components
        if (navMeshAgent != null)
        {
            navMeshAgent.enabled = true;

            // Place the NavMeshAgent on the NavMesh
            if (!navMeshAgent.Warp(transform.position))
            {
                Debug.LogError($"{gameObject.name}: Failed to warp NavMeshAgent to position {transform.position}");
            }

            navMeshAgent.isStopped = false;
            navMeshAgent.stoppingDistance = attackRange;
        }

        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = true;
        }

        // Reset animator triggers and state
        animator.ResetTrigger("Die");
        animator.ResetTrigger("Attack");

        // Re-initialize state machine
        InitializeStateMachine();
    }

}
