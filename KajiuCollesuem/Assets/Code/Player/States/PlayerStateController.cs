﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerStateController : MonoBehaviour
{
    //THIS IS THE STATE MANAGER - Danish, Oliver

         /*
         This script is to change the player's active state 
         depending on the inputs received. 
         */

    // On Ground
    [HideInInspector] public bool onGround = false;
    [HideInInspector] public int groundMaterial = -1;
    [HideInInspector] public float AttackStateReturnDelay = 0;
    [HideInInspector] public float AbilityStateReturnDelay = 0;

    // Inputs
    [HideInInspector] public float LastInputTime = 0;
    [HideInInspector] public Vector2 mouseInput;
    [HideInInspector] public Vector3 moveInput = new Vector3(0, 0, 0);
    [HideInInspector] public Vector2 moveRawInput = new Vector2(0, 0);
    [HideInInspector] public Vector2 LastMoveDirection = new Vector2(0, 0);
    // 0 - Tapped, 1 - Held
    [HideInInspector] public float dodgeInput = -1.0f;
    [HideInInspector] public float abilityinput = -1.0f;
    [HideInInspector] public float lightAttackinput = -1.0f;
    // 0 - Released, 1 - Pressed
    [HideInInspector] public float heavyAttackinput = -1.0f;

    [Header("State Components")]
    [HideInInspector] public PlayerStateMachine _stateMachine;
    public MovementComponent _movementComponent { get; private set; } // Player's movement component, access this to move and jump
    [HideInInspector] public DodgeComponent _dodgeComponent; // Player's dodge component, access this to
    [HideInInspector] public PlayerLockOnScript _lockOnComponent;
    [HideInInspector] public PlayerAbilitySelector _powerComponent;
    [HideInInspector] public PlayerHitbox _hitboxComponent;

    [HideInInspector] public PlayerAttributes _playerAttributes;
    [HideInInspector] public ModelController _modelController;
    [HideInInspector] public PlayerCamera _playerCamera;

    [HideInInspector] public PlayerSFX _Sound;
    [HideInInspector] public CameraShakeCont _CameraShake;

    [HideInInspector] public Rigidbody _Rb;
    public Transform _Camera;
    public PlayerHitbox[] hitboxes = new PlayerHitbox[0];

    [HideInInspector] public LocomotionParticles _Particles;

    // Abilities
    [HideInInspector] public IAbility _AbilityScript;

    [HideInInspector] public bool usingAbility = false;

    [HideInInspector] public bool IgnoreNextHeavyRelease = false;
    [HideInInspector] public float IgnoreJumpInputTime = 0.0f;

    void Awake()
    {
        _movementComponent = GetComponent<MovementComponent>();
        _dodgeComponent = GetComponent<DodgeComponent>();
        _powerComponent = GetComponent<PlayerAbilitySelector>();
        _lockOnComponent = GetComponent<PlayerLockOnScript>();
        _hitboxComponent = GetComponentInChildren<PlayerHitbox>();

        _playerAttributes = GetComponent<PlayerAttributes>();
        _modelController = GetComponentInChildren<ModelController>();

        _CameraShake = GetComponent<CameraShakeCont>();
        _Sound = GetComponentInChildren<PlayerSFX>();

        _stateMachine = GetComponent<PlayerStateMachine>();
        InitializeStateMachine();

        _Rb = GetComponent<Rigidbody>();
        _playerCamera = _Camera.GetComponentInParent<PlayerCamera>();

        _Particles = GetComponentInChildren<LocomotionParticles>();

        LastInputTime = Time.time;
    }

    #region Inputs
    // List for inputs
    public void OnCamera(InputValue ctx) => mouseInput = ctx.Get<Vector2>();
    public void OnMovement(InputValue ctx) => moveRawInput = ctx.Get<Vector2>();
    public void OnDodge(InputValue ctx) => dodgeInput = ctx.Get<float>();
    public void OnAbility(InputValue ctx)
    {
        // AbilityState is on cooldown
        if (AbilityStateReturnDelay > Time.time)
            return;

        // Not enough power
        if (_playerAttributes.getAbility() < _modelController.abilitySO.abilityRequired)
            return;

        abilityinput = ctx.Get<float>();
    }
    public void OnLightAttack(InputValue ctx)
    {
        // AttackState is on cooldown
        if (AttackStateReturnDelay > Time.time)
            return;

        lightAttackinput = ctx.Get<float>();
    }
    public void OnHeavyAttack(InputValue ctx)
    {
        // Auto released already so ignore next release
        if (IgnoreNextHeavyRelease)
        {
            IgnoreNextHeavyRelease = false;
            return;
        }

        // AttackState is on cooldown
        if (AttackStateReturnDelay > Time.time)
            return;

        heavyAttackinput = ctx.Get<float>();
    }
    public void OnJump()
    {
        if (Time.time >= IgnoreJumpInputTime)
        {
            _movementComponent.OnJump();
        }
    }
    public void OnAnyInput() => LastInputTime = Time.time;
    #endregion

    #region StateChecks
    public Type DeadCheck()
    {
        //Dead
        if (_playerAttributes.getHealth() <= 0)
        {
            return typeof(DeathState);
        }

        return null;
    }

    public Type attackOrAbilityCheck()
    {
        // Attack
        if (heavyAttackinput == 1.0f || lightAttackinput == 1.0f)
        {
            return typeof(AttackState);
        }

        // Ability
        if (abilityinput == 1.0f)
        {
            return typeof(AbilityState);
        }

        return null;
    }
    #endregion

    private void Update()
    {
        RotateMoveInputToCamera();
    }

    private void FixedUpdate()
    {
        if (moveInput.magnitude != 0 || mouseInput.magnitude != 0)
        {
            LastInputTime = Time.time;
        }
    }

    public void RotateMoveInputToCamera()
    {
        moveInput = new Vector3(moveRawInput.x, 0, moveRawInput.y);
        moveInput = _Camera.TransformDirection(moveInput);
        moveInput.y = 0;
        moveInput.Normalize();
    }

    void InitializeStateMachine()
    {
        var states = new Dictionary<Type, BaseState>()
        {
            {typeof(IdleState), new IdleState(controller:this) },
            {typeof(MovementState), new MovementState(controller:this) },
            {typeof(DodgeState), new DodgeState(controller:this) },
            {typeof(AttackState), new AttackState(controller:this) },
            {typeof(AbilityState), new AbilityState(controller:this) },
            {typeof(DeathState), new DeathState(controller:this) }
        };

        _stateMachine.SetStates(states);
    }

    
}
