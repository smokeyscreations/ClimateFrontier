using System;
using UnityEngine;
using PlayerStates;

public abstract class BasePlayer : MonoBehaviour
{
    [Header("Player Attributes")]
    [SerializeField] protected float maxHealth = 100f;
    [SerializeField] protected float baseAttackDamage = 10f;
    [SerializeField] protected float movementSpeed = 5f;
    [SerializeField] protected float abilityCooldown = 1f;
    [SerializeField] private float attackRange = 10f;
    [SerializeField] private LayerMask enemyLayerMask;
    private Collider[] hitEnemies = new Collider[20];
    private float attackCooldown = 0.5f;
    private float lastAttackTime;


    protected PlayerHealth healthSystem;
    protected internal Animator animator;
    protected Rigidbody rb;

    public Vector3 MovementInput { get; private set; }

    protected StateMachine stateMachine;
    protected IState idleState;
    protected IState runState;
    protected IState attackState;

    public float AttackRange { get => attackRange; set => attackRange = value; }

    public Transform Target { get; protected set; } // Use in Awake or Start to find and assign the target

    public float MaxHealth { get => maxHealth; set => maxHealth = value; }
    public float BaseAttackDamage { get => baseAttackDamage; set => baseAttackDamage = value; }
    public float MovementSpeed { get => movementSpeed; set => movementSpeed = value; }
    public float AbilityCooldown { get => abilityCooldown; set => abilityCooldown = value; }
    public LayerMask EnemyLayerMask { get => enemyLayerMask; set => enemyLayerMask = value; }
    public float AttackCooldown { get => attackCooldown; set => attackCooldown = value; }
    public float LastAttackTime { get => lastAttackTime; set => lastAttackTime = value; }
    public Collider[] HitEnemies { get => hitEnemies; set => hitEnemies = value; }

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        healthSystem = GetComponent<PlayerHealth>();
        healthSystem.Initialize(maxHealth);
        Target = GameObject.FindWithTag("Enemy")?.transform;
        InitializeStateMachine();  // Initialize the state machine here, only once
    }

    protected virtual void InitializeStateMachine()
    {
        stateMachine = new StateMachine();
        idleState = new PlayerIdleState(this);
        runState = new PlayerRunState(this);

        float inputThreshold = 0.1f;
        stateMachine.AddTransition(idleState, runState, () => MovementInput.sqrMagnitude > inputThreshold * inputThreshold);
        stateMachine.AddTransition(runState, idleState, () => MovementInput.sqrMagnitude <= inputThreshold * inputThreshold);

        stateMachine.SetState(idleState);
    }

    protected virtual void Update()
    {
        GatherInput();
        stateMachine.Tick();
    }

    protected virtual void FixedUpdate()
    {
        Move();
    }

    protected virtual void GatherInput()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        MovementInput = new Vector3(horizontal, 0, vertical).normalized;
    }

    public virtual void Move()
    {
        if (MovementInput != Vector3.zero)
        {
            Vector3 movement = MovementInput.normalized * movementSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + movement);

            Quaternion targetRotation = Quaternion.LookRotation(MovementInput);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, 10f * Time.fixedDeltaTime);
        }
    }

    protected virtual bool IsAttacking() => Input.GetButtonDown("Fire1");

    public virtual void TakeDamage(float amount) => healthSystem.TakeDamage(amount);

    public abstract void BaseAttack();
    public abstract void UseAbility(int abilityIndex);

    public void ScaleHealth(float healthIncrease)
    {
        MaxHealth += healthIncrease;
        healthSystem.Initialize(MaxHealth);
    }

   
}
