﻿using System;
using UnityEngine;

public class AttackState : BaseState
{
    PlayerStateController _stateController;

    private float _exitStateTime = 0;

    private float _enableHitboxTime = 0;
    private float _disableHitboxTime = 0;
    private PlayerHitbox _hitbox = null;

    private float _attackStateReturnDelayLength = 0.2f;
    private float _minChargeTime = 0.0f;
    private float _maxCharge = 4.0f;

    private bool _onHolding = false;
    private bool _alreadyReleased = false;
    private float _chargeTime = 0f;

    public AttackState(PlayerStateController controller) : base(controller.gameObject)
    {
        _stateController = controller;
    }

    public override void Enter()
    {
        //Debug.Log("AttackState: Enter");
        _exitStateTime = 0;
        _onHolding = false;
        _alreadyReleased = false;
        _stateController._modelController.SetInputDirection(_stateController.moveInput);
        CheckForAttack();
    }

    public override void Exit()
    {
        //Debug.Log("AttackState: Exit");
        _stateController.AttackStateReturnDelay = Time.time + _attackStateReturnDelayLength;

        // If leaving state before disabling hitbox, disable hitbox
        if (_hitbox != null)
        {
            _hitbox.gameObject.SetActive(false);
            _hitbox = null;
        }

        _stateController._modelController.SetInputDirection(Vector3.zero);
        ClearInputs();
    }

    public override Type Tick()
    {
        // Stunned Or Dead
        Type stunnedOrDead = _stateController.stunnedOrDeadCheck();
        if (stunnedOrDead != null)
            return stunnedOrDead;

        ChargingAttack();

        // Leave state after attack
        if (Time.time > _exitStateTime && _onHolding == false)
        {
            return typeof(MovementState);
        }

        // Hitbox enable & disable
        if (_hitbox != null)
        {
            if (Time.time >= _disableHitboxTime)
            {
                _hitbox.gameObject.SetActive(false);
                _hitbox = null;
            }
            else if (Time.time >= _enableHitboxTime)
            {
                _hitbox.gameObject.SetActive(true);
            }
        }

        return null;
    }

    private void ChargingAttack()
    {
        // If holding don't listen for more attacks, listen for release else run holding code.
        if (_onHolding == true)
        {
            // Delay before starting charging
            if (Time.time >= _minChargeTime)
            {
                // ON RELEASE HEAVY (Called Once)
                if (_stateController.heavyAttackinput == 0 || _alreadyReleased)
                {
                    //Debug.Log("AttackState: CheckForAttack - HEAVY RELEASED");
                    ReleaseHeavyAttack();
                }
                // If Reached max charge release heavy attack
                else if (Time.time >= _chargeTime)
                {
                    //Debug.Log("AttackState: CheckForAttack - HEAVY RELEASED Maxed Charge");
                    // Automatically did a release input so ignore the actual input
                    _stateController.IgnoreNextHeavyRelease = true;
                    ReleaseHeavyAttack();
                }
            }
        }
    }

    private void CheckForAttack()
    {
        // ON RELEASED HEAVY BEFORE CHARGING STARTED (Called Once)
        if (_stateController.heavyAttackinput == 0)
        {
            //Debug.Log("AttackState: CheckForAttack - HEAVY RELEASED Before Charging");
            PressedHeavyAttack();
            _alreadyReleased = true;
        }
        // ON PRESSED HEAVY (Called Once)
        else if (_stateController.heavyAttackinput == 1)
        {
            //Debug.Log("AttackState: CheckForAttack - HEAVY PRESSED");
            PressedHeavyAttack();
        }
        // ON PRESSED LIGHT (Called Once)
        else if (_stateController.lightAttackinput == 1)
        {
            //Debug.Log("AttackState: CheckForAttack - LIGHT PRESSED");
            PressedLightAttack();
        }
    }

    #region Pressed & Release
    private void PressedLightAttack()
    {
        //Debug.Log("AttackState: PressedLightAttack");
        SOAttack curAttack = _stateController._modelController.attacks[0];
        float PreAttackTime = curAttack.holdStartPosTime;
        SetAttackValues(curAttack, PreAttackTime);

        _stateController._modelController.PlayAttack(0, false);
    }

    private void PressedHeavyAttack()
    {
        //Debug.Log("AttackState: PressedHeavyAttack");
        _stateController._modelController.PlayAttack(1, true);

        _minChargeTime = Time.time + _stateController._modelController.attacks[1].holdStartPosTime;
        _chargeTime = Time.time + _maxCharge + _minChargeTime;
        _onHolding = true;
    }

    private void ReleaseHeavyAttack()
    {
        //Debug.Log("AttackState: ReleaseHeavyAttack");
        SOAttack curAttack = _stateController._modelController.attacks[1];
        SetAttackValues(curAttack); // TODO Add charging mult to hitbox

        // TODO send through how long attack was charged for and use that to know how fast the attack should move.
        _stateController._modelController.DoneChargingAttack();

        _onHolding = false;
    }
    #endregion

    #region Set & Clear
    private void SetAttackValues(SOAttack pAttackVars, float pPreAttackTime = 0)
    {
        // Timings
        _exitStateTime = Time.time + pAttackVars.attackTime + pPreAttackTime + pAttackVars.holdEndPosTime;

        // Hitbox
        _hitbox = _stateController.hitboxes[pAttackVars.hitboxIndex];
        Vector3 knockVector = _stateController._modelController.transform.forward * pAttackVars.HitboxKnockback + Vector3.up * pAttackVars.HitboxKnockup;
        _hitbox.SetDamage(pAttackVars.HitboxDamage, knockVector);

        _enableHitboxTime = Time.time + pAttackVars.enableHitboxTime + pPreAttackTime;
        _disableHitboxTime = Time.time + pAttackVars.disableHitboxTime + pPreAttackTime;
    }

    private void ClearInputs()
    {
        _stateController.lightAttackinput = -1.0f;
        _stateController.heavyAttackinput = -1.0f;
        _stateController.ability1input = -1.0f;
        _stateController.ability2input = -1.0f;
    }
    #endregion
}