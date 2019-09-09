﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateController : MonoBehaviour
{
    //THIS IS THE STATE MANAGER - Danish, Oliver

         /*
         This script is to change the player's active state 
         depending on the inputs received. 
         */


    [Header("Inputs")]
    // Movement variables
    [HideInInspector] public float horizontalInput = 0;
    [HideInInspector] public float verticalInput = 0;
    [HideInInspector] public Vector3 movementDir;
    [HideInInspector] public float moveAmount = 0;

    // Movement Variables
    [HideInInspector] public bool jumpInput = false;
    [HideInInspector] public bool longDodgeInput = false;
    [HideInInspector] public bool shortDodgeInput = false;

    // Attack Varaibles
    [HideInInspector] public bool quickAttackInput = false;
    [HideInInspector] public bool heavyAttackInput = false;

    //Camera
    [HideInInspector] public bool lockOnInput = false;

    // Power Use Inputs
    [HideInInspector] public int powerInput = 0;

    // Menu Inputs
    [HideInInspector] public bool pauseInput = false;
    [HideInInspector] public bool mapInput = false;

    // On Ground
    [HideInInspector] public bool OnGround = false;

    [Header("State Components")]
    private PlayerStateMachine stateMachine;
    public PlayerMovement _movementComponent { get; private set; } // Player's movement component, access this to move and jump
    // TODO LockOn Component // Player's lockon component changes player's movement aanimations
    [HideInInspector] public PlayerDodge _dodgeComponent; // Player's dodge component, access this to
    private PlayerLockOnScript _lockOnComponent;
    [HideInInspector] public PlayerPowerHandler _powerComponent;
    [HideInInspector] public PlayerRespawn _respawnComponent;

    [HideInInspector] public PlayerAttributes _playerAttributes;
    [HideInInspector] public AnimHandler _animHandler;

    [HideInInspector] public Rigidbody _rb;

    //enum States
    //{
    //    Normal, // Player's default state, able to move and can initiate attack
    //    LockedOn, // Payer is locked on to an enemy and can transition into any other state
    //    Dodging, // Player is currently in a dodge aninmation and cannot move or initiate an attack
    //    Attacking, // Player is currently in a attack animation, cannot move and cannot initiate another attack
    //    Stunned, // Player is stunned and cannot move
    //    Dead // Player doesn't receive anymore input
    //};
    
    //[SerializeField] private int state = (int) States.Normal;
    
    void Start()
    {
        _movementComponent = GetComponent<PlayerMovement>();
        _dodgeComponent = GetComponent<PlayerDodge>();
        _lockOnComponent = GetComponent<PlayerLockOnScript>();
        _powerComponent = GetComponent<PlayerPowerHandler>();

        _respawnComponent = GetComponent<PlayerRespawn>();
        _playerAttributes = GetComponent<PlayerAttributes>();
        _animHandler = GetComponentInChildren<AnimHandler>();

        stateMachine = GetComponent<PlayerStateMachine>();
        InitializeStateMachine();

        _rb = GetComponent<Rigidbody>();
    }

    void InitializeStateMachine()
    {
        var states = new Dictionary<Type, BaseState>()
        {
            {typeof(MovementState), new MovementState(controller:this) },
            {typeof(DodgeState), new DodgeState(controller:this) },
            {typeof(StunnedState), new StunnedState(controller:this) },
            {typeof(AttackState), new AttackState(controller:this) },
            {typeof(DeathState), new DeathState(controller:this) }
        };

        stateMachine.SetStates(states);
    }
}
