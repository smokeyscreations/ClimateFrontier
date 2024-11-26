using System;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using EnemyStates;
using TMPro;

public abstract class BaseEnemy : MonoBehaviour, IDamageable
{
    [Header("Enemy Settings")]
    [SerializeField] protected float attackRange = 2f;
    [SerializeField] protected float maxHealth = 100f;
    [SerializeField] private float baseAttackDamage = 10f;
    [SerializeField] private float pathUpdateInterval = 0.2f; // Set a default interval for path updates

    public int experienceAmount = 10;

    public GameObject floatingTextPrefab;

    public NavMeshAgent navMeshAgent;
    public Animator animator;
    public Transform Target { get; protected set; }
    protected float currentHealth;
    private PlayerHealth playerHealth;

    private float pathUpdateTimer;
    protected bool isDead = false;

    protected StateMachine stateMachine;
    protected IState chaseState;
    protected IState attackState;

    public event Action<BaseEnemy> OnEnemyDeath;

    protected EnemyFlashEffect3D flashEffect;
    protected TextMeshPro textMesh;

    protected virtual void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;

        navMeshAgent.stoppingDistance = attackRange;

        InitializeStateMachine();

        // Reference to the flash effect script
        flashEffect = GetComponent<EnemyFlashEffect3D>();
        if (flashEffect == null)
        {
            Debug.LogWarning($"{gameObject.name}: No EnemyFlashEffect3D component found.");
        }
    }

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
        if (isDead || !gameObject.activeInHierarchy) return;
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

    public virtual void LookAtTarget()
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
        if (!gameObject.activeInHierarchy)
        {
            Debug.LogWarning($"{gameObject.name}: Attempted to take damage while inactive.");
            return;
        }

        if (isDead) return;

        currentHealth -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage. Remaining health: {currentHealth}");

        // Trigger on-hit effects
        TriggerOnHitEffects(damage);

        // Show floating damage text
        ShowFloatingDamage(damage);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Spawns a floating damage number using the FloatingText prefab.
    /// </summary>
    /// <param name="damage">The amount of damage taken.</param>
    protected virtual void ShowFloatingDamage(float damage)
    {
        if (floatingTextPrefab == null)
        {
            Debug.LogError($"{gameObject.name}: FloatingTextPrefab is not assigned.");
            return;
        }

        // Define spawn position slightly above the enemy to prevent overlap
        Vector3 spawnPosition = transform.position + new Vector3(UnityEngine.Random.Range(-0.5f, 0.5f), 1.5f, UnityEngine.Random.Range(-0.5f, 0.5f));

        // Spawn a FloatingText instance from the pool
        GameObject floatingTextGO = ObjectPooler.Instance.SpawnFromPool("FloatingText", spawnPosition, Quaternion.identity);
        textMesh = floatingTextGO.GetComponent<TextMeshPro>();

        if (floatingTextGO != null)
        {
            FloatingText floatingText = floatingTextGO.GetComponent<FloatingText>();
            if (floatingText != null)
            {
                // Set the damage number
                floatingText.SetText(Mathf.RoundToInt(damage).ToString());
            }
            else
            {
                Debug.LogError($"{gameObject.name}: FloatingText component not found on the prefab.");
            }
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

    protected virtual void SpawnExperienceOrb()
    {
        GameObject orb = ExperienceOrbPool.Instance.GetOrb();
        orb.transform.position = transform.position;

        ExperienceOrb expOrb = orb.GetComponent<ExperienceOrb>();
        if (expOrb != null)
        {
            expOrb.experienceAmount = experienceAmount;
        }
    }

    public virtual void MoveToTarget()
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

    public virtual void ResetEnemy()
    {
        // Reset health and state
        isDead = false;
        currentHealth = maxHealth;

        // Reset components
        if (navMeshAgent != null)
        {
            navMeshAgent.enabled = true;
            navMeshAgent.isStopped = false;
            navMeshAgent.stoppingDistance = attackRange;

            // Place the NavMeshAgent on the NavMesh
            if (!navMeshAgent.Warp(transform.position))
            {
                Debug.LogError($"{gameObject.name}: Failed to warp NavMeshAgent to position {transform.position}");
            }
        }

        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = true;
        }

        // Reset animator triggers and state
        animator.ResetTrigger("Die");
        animator.ResetTrigger("Attack");

        // Reset flash effect
        if (flashEffect != null)
        {
            flashEffect.ResetFlash();
        }

        // Re-initialize state machine
        InitializeStateMachine();
    }

    // Method to trigger on-hit effects
    private void TriggerOnHitEffects(float damage)
    {
        // Trigger white flash
        if (flashEffect != null)
        {
            flashEffect.TriggerFlash();
        }

        // Add other on-hit effects here (e.g., sound, particles)
    }
}
