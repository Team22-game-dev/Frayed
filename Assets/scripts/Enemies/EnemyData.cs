using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyData : MonoBehaviour, IAttackData
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
    public float angularSpeed { get { return _angularSpeed; } private set { _angularSpeed = value; } }
    public float height { get { return _height; } private set { _height = value; } }
    public float GetAttackPower () => _baseAttackPower;
    public GameObject mainCharacter { get { return _mainCharacter; } private set { _mainCharacter = value; } }
    public Rigidbody rb { get { return _rb; } private set { _rb = value; } }

    public Vector3 spawnPosition { get { return _spawnPosition; } private set { _spawnPosition = value; } }

    public float baseHealth { get { return _baseHealth; } }
    public float currentHealth { get {return _currentHealth; } }

    public void TakeDamage(float damage) => _currentHealth = Mathf.Max(0.0f, _currentHealth - damage);

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

    [SerializeField]
    [Tooltip("In deg/s.")]
    private float _angularSpeed = 90f;


    [Header("Enemy Character Information")]
    [SerializeField]
    private float _height = 2f;

    [Header("Enemy Stats")]
    [SerializeField]
    private float _baseHealth = 100f;
    [SerializeField]
    private float _startingHealth;
    [SerializeField]
    private float _currentHealth;

    [SerializeField]
    private float _baseAttackPower = 1f;

    [Header("Game Information")]
    [SerializeField]
    private GameObject _mainCharacter;
    [SerializeField]
    private GameObject mainCamera;

    private PlayerStats playerStats;
    private Rigidbody _rb;


    [Header("Health Slider Fields")]
    GameObject healthBarSliderGameObject;
    private Slider healthBarSlider;
    [SerializeField]
    private float healthBarSliderZ = 0.75f;
    [SerializeField]
    private float healthBarSliderY = 2.25f;


    private Vector3 _spawnPosition;

    public float GetHealthRatio()
    {
        return _currentHealth / _startingHealth;
    }

    private void Start()
    {
        if (_mainCharacter == null)
        {
            _mainCharacter = GameObject.FindGameObjectWithTag("Player");
        }
        Debug.Assert(_mainCharacter != null);

        if (mainCamera == null)
        {
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }
        Debug.Assert(mainCamera != null);

        _spawnPosition = this.transform.position;
        if (playerStats == null)
        {
            playerStats = _mainCharacter.GetComponent<PlayerStats>();
        }

        healthBarSliderGameObject = transform.Find("EnemyHealthBar/Slider").gameObject;
        Debug.Assert(healthBarSliderGameObject != null);
        healthBarSliderGameObject.transform.localPosition = new Vector3(0, healthBarSliderY, healthBarSliderZ);
        healthBarSlider = healthBarSliderGameObject.GetComponent<Slider>();
        Debug.Assert(healthBarSlider != null);

        _rb = GetComponent<Rigidbody>();
        Debug.Assert(_rb != null);

        // Temp starting health.
        _startingHealth = _baseHealth;
        _currentHealth = _startingHealth;

        // Assign the value to startingHealth only once
       // _currentHealth = _startingHealth = _baseHealth * playerStats._skill;
//        currentAttackPower = _baseAttackPower * playerStats._skill;
    }

    private void Update()
    {
        healthBarSlider.value = GetHealthRatio();
        Vector3 sliderDirection = (mainCamera.transform.position - healthBarSliderGameObject.transform.position).normalized;
        healthBarSliderGameObject.transform.rotation = Quaternion.LookRotation(sliderDirection);
        if (Mathf.Approximately(healthBarSlider.value, 1f))
        {
            healthBarSliderGameObject.SetActive(false);
        } 
        else
        {
            healthBarSliderGameObject.SetActive(true);
        }

        // TODO: Temp dummy logic to kill enemies for now.
        if (Mathf.Approximately(GetHealthRatio(), 0.0f))
        {
            gameObject.SetActive(false);
        }
    }

}
