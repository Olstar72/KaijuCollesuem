﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitbox : MonoBehaviour
{
    [HideInInspector] public float attackMult = 1;
    
    private int _damage;
    private Vector3 _knockback;

    private int _powerRecivedOnHit = 15;

    public GameObject attacker;

    private PlayerAttributes _playerAttributes;
    private IAttributes _playerIAttributes;

    private List<IAttributes> hitAttributes = new List<IAttributes>();
    [SerializeField] private ParticleSystem _HitParticle;

    private void OnEnable()
    {
        // Clear list
        hitAttributes = new List<IAttributes>();
    }

    private void Start()
    {
        _playerAttributes = GetComponentInParent<PlayerAttributes>();
        _playerIAttributes = _playerAttributes.GetComponent<IAttributes>();
    }

    private void OnTriggerEnter (Collider other)
    {
        //Check if collided with an Attributes Script
        IAttributes otherAttributes = other.GetComponent<IAttributes>();
        if (otherAttributes == null)
            otherAttributes = other.GetComponentInParent<IAttributes>();

        // Don't hit the same thing twice
        if (hitAttributes.Contains(otherAttributes) || other.CompareTag("Fireball"))
            return;

        // Add to list so we can't hit it twice
        hitAttributes.Add(otherAttributes);

        if (otherAttributes != null && otherAttributes.IsDead() == false && otherAttributes != _playerIAttributes)
        {
            // Damage other
            otherAttributes.TakeDamage(Mathf.FloorToInt(_damage * attackMult), _knockback, attacker, "Player");

            // Recieve Power
            _playerAttributes.modifyAbility(_powerRecivedOnHit);

            // Hit Effect
            if (_HitParticle != null)
                _HitParticle.Play();
        }
    }

    public void SetDamage(int pDamage, Vector3 pKnockback)
    {
        _damage = pDamage;
        _knockback = pKnockback;
    }
}
