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

    protected PlayerHealth healthSystem;
    protected internal Animator animator; // Changed to protected internal
    protected Rigidbody rb;

    // New MovementInput property
    public Vector3 MovementInput { get; private set; }

    // State Machine and States
    protected StateMachine stateMachine;
    protected IState idleState;
    protected IState runState;
    protected IState attackState;

    // Properties for other attributes
    public float MaxHealth { get => maxHealth; set => maxHealth = value; }
    public float BaseAttackDamage { get => baseAttackDamage; set => baseAttackDamage = value; }
    public float MovementSpeed { get => movementSpeed; set => movementSpeed = value; }
    public float AbilityCooldown { get => abilityCooldown; set => abilityCooldown = value; }

    protected virtual void InitializeStateMachine()
    {
        stateMachine = new StateMachine();
        idleState = new PlayerIdleState(this);
        runState = new PlayerRunState(this);
        attackState = new PlayerAttackState(this);

        // Set up transitions
        float inputThreshold = 0.1f; // Threshold for movement input
        stateMachine.AddTransition(idleState, runState, () => playerIsMoving());
        stateMachine.AddTransition(runState, idleState, () => !playerIsMoving());
        stateMachine.AddTransition(idleState, attackState, IsAttacking);
        stateMachine.AddTransition(runState, attackState, IsAttacking);
        stateMachine.AddTransition(attackState, idleState, () => !IsAttacking());

        stateMachine.SetState(idleState);

        bool playerIsMoving() => MovementInput.sqrMagnitude > inputThreshold * inputThreshold;
    }


    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        healthSystem = GetComponent<PlayerHealth>();
        healthSystem.Initialize(maxHealth);

        InitializeStateMachine();
    }

    protected virtual void FixedUpdate()
    {
        Move();
    }



    protected virtual void Update()
    {
        GatherInput();
        stateMachine.Tick();
    }

    /// <summary>
    /// Gathers input for movement and abilities.
    /// </summary>
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
            // Move the player
            Vector3 movement = MovementInput.normalized * movementSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + movement);

            // Smoothly rotate player to face movement direction
            Quaternion targetRotation = Quaternion.LookRotation(MovementInput);
            float rotationSpeed = 10f; // Adjust this value to control rotation smoothness

            // Use FixedDeltaTime for consistent rotation in FixedUpdate
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }



    protected virtual bool IsAttacking()
    {
        return Input.GetButtonDown("Fire1"); // Adjust the input based on your setup
    }

    public virtual void TakeDamage(float amount)
    {
        healthSystem.TakeDamage(amount);
    }

    public abstract void BaseAttack();
    public abstract void UseAbility(int abilityIndex);

    public void ScaleHealth(float healthIncrease)
    {
        MaxHealth += healthIncrease;
        healthSystem.Initialize(MaxHealth);
    }
}
