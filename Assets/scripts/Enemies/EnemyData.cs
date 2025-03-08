using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyData : MonoBehaviour
{
    public float chaseRadius { get { return _chaseRadius; } private set { _chaseRadius = value; } }
    public GameObject mainCharacter { get { return _mainCharacter; } private set { _mainCharacter = value; } }

    [SerializeField]
    private float _chaseRadius = 20f;

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
