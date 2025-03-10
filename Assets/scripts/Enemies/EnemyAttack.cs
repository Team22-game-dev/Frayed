using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyAttack : AttackBase  // Now abstract
{
    protected EnemyManager enemyManager;  // Made protected for subclasses

    protected void Start()
    {
        enemyManager = GetComponent<EnemyManager>();

        if (enemyManager == null)
            Debug.LogError("EnemyManager component not found on " + gameObject.name);
    }

    public override bool AttackTrigger()
    {

        return enemyManager != null && enemyManager.currentState == EnemyManager.State.READY_TO_ATTACK;
    }

}
