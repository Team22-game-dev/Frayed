using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyData : MonoBehaviour
{
    public bool enableWanderBehavior { get { return _enableWanderBehavior; } private set { _enableWanderBehavior = value; } }
    public float maximumWanderRadius { get { return _maximumWanderRadius; } private set { _maximumWanderRadius = value; } }
    public float wanderSpeed { get { return _wanderSpeed; } private set { _wanderSpeed = value; } }
    public float minimumWanderPause { get { return _minimumWanderPause; } private set { _minimumWanderPause = value; } }
    public float maximumWanderPause { get { return _maximumWanderPause; } private set { _maximumWanderPause = value; } }
    public float chaseRadius { get { return _chaseRadius; } private set { _chaseRadius = value; } }
    public float chaseSpeed { get { return _chaseSpeed; } private set { _chaseSpeed = value; } }
    public float attackRadius { get { return _attackRadius; } private set { _attackRadius = value; } }
    public float attackDuration { get { return _attackDuration; } private set { _attackDuration = value; } }
    public float height { get { return _height; } private set { _height = value; } }
    public GameObject mainCharacter { get { return _mainCharacter; } private set { _mainCharacter = value; } }

    public Vector3 spawnPosition { get { return _spawnPosition; } private set { _spawnPosition = value; } }

    public float getenemyhealth { get {return currentHealth; } }
    public float getAttackPower { get { return currentAttackPower; } }

    [Header("Wander Fields")]
    [SerializeField]
    private bool _enableWanderBehavior = true;
    [SerializeField]
    [Tooltip("Maximum radius the enemy can wander from its original spawn location.")]
    private float _maximumWanderRadius = 30f;
    [SerializeField]
    private float _wanderSpeed = 1.5f;
    [SerializeField]
    [Tooltip("In seconds.")]
    private float _minimumWanderPause = 1.5f;
    [SerializeField]
    [Tooltip("In seconds.")]
    private float _maximumWanderPause = 3f;


    [Header("Chase Fields")]
    [SerializeField]
    private float _chaseRadius = 20f;
    [SerializeField]
    private float _chaseSpeed = 2.5f;


    [Header("Attack Fields")]
    [SerializeField]
    private float _attackRadius = 2f;
    [SerializeField]
    [Tooltip("In seconds.")]
    private float _attackDuration = 1f;


    [Header("Enemy Character Information")]
    [SerializeField]
    private float _height = 2f;

    [Header("Enemy Stats")]
    [SerializeField]
    private float _baseHealth = 100f; 
    private float startingHealth;
    private float currentHealth;

    [SerializeField]
    private float _baseAttackPower = 1f;
    private float currentAttackPower;

    [Header("Game Information")]
    [SerializeField]
    private GameObject _mainCharacter;
    private PlayerStats playerStats;


    private Vector3 _spawnPosition;

    public float GetHealthRatio()
    {
        return currentHealth / startingHealth;
    }

    private void Start()
    {
        if (_mainCharacter == null)
        {
            _mainCharacter = GameObject.FindGameObjectWithTag("Player");
        }
        Debug.Assert(_mainCharacter != null);

        _spawnPosition = this.transform.position;
        if (playerStats == null)
        {
            playerStats = _mainCharacter.GetComponent<PlayerStats>();
        }

        // Assign the value to startingHealth only once
        currentHealth = startingHealth = _baseHealth * playerStats._skill;
        currentAttackPower = _baseAttackPower * playerStats._skill;
    }
    

}
