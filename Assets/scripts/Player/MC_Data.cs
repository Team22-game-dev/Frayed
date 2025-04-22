using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MC_Data : MonoBehaviour, IAttackData
{

    public float GetAttackPower() => _attackPower;

    [Header("Power Stats")]
    [SerializeField]
    private float _attackPower = 1.0f;

    public int enemiesKilled = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _attackPower = Mathf.Pow((enemiesKilled + 1) * 2.0f, 1.0f / 2.0f);
    }


}
