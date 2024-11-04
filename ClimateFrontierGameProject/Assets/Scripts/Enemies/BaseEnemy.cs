using System;
using UnityEngine;
using UnityEngine.AI;
using EnemyStates;
using static UnityEngine.GraphicsBuffer;

public abstract class BaseEnemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float baseAttackDamage = 10f;
    [SerializeField] private float pathUpdateInterval = 0.2f; // Set a default interval for path updates

    public NavMeshAgent navMeshAgent;
    public Animator animator;
    public Transform Target { get; protected set; }
    private float currentHealth;
    private PlayerHealth playerHealth;

    private float pathUpdateTimer; // Timer to control path update delay

    protected StateMachine stateMachine;
    protected IState chaseState;
    protected IState attackState;

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

    protected Func<bool> PlayerInAttackRange => () => Vector3.Distance(transform.position, Target.position) <= AttackRange;
    protected Func<bool> PlayerOutOfAttackRange => () => Vector3.Distance(transform.position, Target.position) > AttackRange;
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
        //// Define a detection radius around the enemy's attack range
        //Collider[] hitObjects = Physics.OverlapSphere(transform.position + transform.forward * attackRange, attackRange);

        //foreach (Collider hit in hitObjects)
        //{
        //    // Check if the collider has the "Player" tag
        //    if (hit.CompareTag("Player"))
        //    {
        //        if (hit.TryGetComponent(out PlayerHealth playerHealth))
        //        {
        //            playerHealth.TakeDamage(baseAttackDamage);
        //            Debug.Log($"{gameObject.name} dealt {baseAttackDamage} damage to player.");
        //            break; // Stop after hitting the player once
        //        }
        //    }
        //}

        // Ensure this method is called via the animation event at the correct time
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
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        animator.SetTrigger("Die");
        navMeshAgent.isStopped = true;
        Destroy(gameObject, 2f);
    }

    //public bool ShouldUpdatePath() // Helper function to check if path should update
    //{
    //    if (Time.time >= pathUpdateTimer)
    //    {
    //        pathUpdateTimer = Time.time + pathUpdateInterval;
    //        return true;
    //    }
    //    return false;
    //}

    public void MoveToTarget()
    {
        if (Time.time >= pathUpdateTimer)
        {
            pathUpdateTimer = Time.time + pathUpdateInterval;
            navMeshAgent.SetDestination(Target.position);
        }
        navMeshAgent.isStopped = false;
    }
}
