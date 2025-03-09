using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyData : MonoBehaviour
{
    public float chaseRadius { get { return _chaseRadius; } private set { _chaseRadius = value; } }
    public float chaseSpeed { get { return _chaseSpeed; } private set { _chaseSpeed = value; } }
    public float attackRadius { get { return _attackRadius; } private set { _attackRadius = value; } }
    public float attackDuration { get { return _attackDuration; } private set { _attackDuration = value; } }
    public float height { get { return _height; } private set { _height = value; } }
    public GameObject mainCharacter { get { return _mainCharacter; } private set { _mainCharacter = value; } }

    [SerializeField]
    private float _chaseRadius = 20f;
    [SerializeField]
    private float _chaseSpeed = 2.5f;
    [SerializeField]
    private float _attackRadius = 2f;
    [SerializeField]
    [Tooltip("In seconds.")]
    private float _attackDuration = 1f;
    [SerializeField]
    private float _height = 2f;
    [SerializeField]
    private GameObject _mainCharacter;

    private void Start()
    {
        if (_mainCharacter == null)
        {
            _mainCharacter = GameObject.FindGameObjectWithTag("Player");
        }
        Debug.Assert(_mainCharacter != null);
    }
}
