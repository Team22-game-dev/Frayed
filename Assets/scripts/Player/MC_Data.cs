using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MC_Data : MonoBehaviour, IAttackData
{

    public float GetAttackPower() => _attackPower;

    [Header("Power Stats")]
    [SerializeField]
    private float _attackPower = 2.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
