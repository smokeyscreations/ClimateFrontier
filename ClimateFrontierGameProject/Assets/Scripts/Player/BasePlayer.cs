using System;
using UnityEngine;
using PlayerStates;

public abstract class BasePlayer : MonoBehaviour
{
    [Header("Character Data")]
    public CharacterData characterData;

    [Header("Player Attributes")]
    [SerializeField] protected float maxHealth = 100f;
    [SerializeField] protected float baseAttackDamage = 50f;


    [SerializeField] private float baseWalkingSpeed = 2.5f; // Adjust as needed
    [SerializeField] private float baseRunningSpeed = 5f;    // Adjust as needed
    private bool isRunning = false;
    private float movementSpeedValue = 0f;
    private float movementDirection = 0f;

    [SerializeField] protected float abilityCooldown = 1f;
    [SerializeField] private float attackRange = 10f;
    [SerializeField] private LayerMask enemyLayerMask;


    [SerializeField] public Transform projectileSpawnPoint;
    private Collider[] hitEnemies = new Collider[20];
    private float attackCooldown = 0.5f;
    private float lastAttackTime;
    public float MovementSpeed
    {
        get => isRunning ? baseRunningSpeed : baseWalkingSpeed;
    }
    protected PlayerHealth healthSystem;
    protected internal Animator animator;
    protected Rigidbody rb;
    protected SpellManager spellManager;

    public Vector3 MovementInput { get; private set; }

    protected StateMachine stateMachine;
    protected IState idleState;
    protected IState runState;
    protected IState attackState;

    private float originalAttackDamage;



    public float AttackRange { get => attackRange; set => attackRange = value; }

    public Transform Target { get; protected set; } // Use in Awake or Start to find and assign the target

    public float MaxHealth { get => maxHealth; set => maxHealth = value; }
    public float BaseAttackDamage { get => baseAttackDamage; set => baseAttackDamage = value; }
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
        Target = GameObject.FindWithTag("Enemy")?.transform;

        // Removed InitializeCharacter() and InitializeStateMachine() from Awake()
    }

    // New method to initialize the player after characterData is assigned
    public virtual void InitializePlayer()
    {

        if (characterData == null)
        {
            Debug.LogError("CharacterData is not assigned.");
            return;
        }

        // Initialize attributes from characterData
        maxHealth = characterData.maxHealth;
        baseAttackDamage = characterData.baseAttackDamage;
        baseWalkingSpeed = characterData.baseWalkingSpeed;
        baseRunningSpeed = characterData.baseRunningSpeed;
        abilityCooldown = characterData.abilityCooldown;
        attackRange = characterData.attackRange;
        enemyLayerMask = characterData.enemyLayerMask;


        originalAttackDamage = baseAttackDamage;


        // Initialize health
        healthSystem.Initialize(maxHealth);

        // Initialize state machine
        InitializeStateMachine();

        spellManager = GetComponent<SpellManager>();
        if (spellManager != null)
        {
            spellManager.spells = characterData.abilities;
            spellManager.InitializeCooldownTimers(); // Ensure cooldown timers are initialized after spells are assigned
        }
        else
        {
            Debug.LogError("SpellManager component not found on player.");
        }
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
        Vector3 inputDirection = new Vector3(horizontal, 0, vertical).normalized;

        // Check if the player is moving
        bool isMoving = inputDirection.magnitude > 0.1f;

        // Check if the player is running
        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        // Calculate speed value
        float speed = 0f;
        if (isMoving)
        {
            speed = isRunning ? 1f : 0.5f; // Run = 1, Walk = 0.5
        }

        float dampTime = 0.1f;
        animator.SetFloat("Speed", speed, dampTime, Time.deltaTime);
        animator.SetBool("IsRunning", isRunning);

        // Store movement input for movement logic
        MovementInput = inputDirection;
        this.isRunning = isRunning; // Update the isRunning field if needed
    }







    public virtual void Move()
    {

        if (MovementInput != Vector3.zero)
        {
            float movementSpeed = isRunning ? baseRunningSpeed : baseWalkingSpeed;
            Vector3 movement = MovementInput * movementSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + movement);

            // Rotate character to face movement direction
            Quaternion targetRotation = Quaternion.LookRotation(MovementInput);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, 10f * Time.fixedDeltaTime);
        }
    }






    protected virtual bool IsAttacking() => Input.GetButtonDown("Fire1");

    public virtual void TakeDamage(float amount) => healthSystem.TakeDamage(amount);
    public void ScaleHealth(float healthIncrease)
    {
        MaxHealth += healthIncrease;
        healthSystem.Initialize(MaxHealth);
    }

    private void UpdateAnimatorParameters()
    {
        animator.SetFloat("Speed", movementSpeedValue);
        animator.SetFloat("Direction", movementDirection);
        animator.SetBool("IsRunning", isRunning);
    }

    public virtual void IncreaseAttackDamage(int amount)
    {
        baseAttackDamage += amount;
        Debug.Log($"Attack Damage increased by {amount}. New Attack Damage: {baseAttackDamage}");
    }


    public virtual void ResetStats()
    {
        baseAttackDamage = originalAttackDamage;
        Debug.Log($"Attack Damage reset to original value: {baseAttackDamage}");
        
    }
}
