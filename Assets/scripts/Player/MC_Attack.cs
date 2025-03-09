using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MC_Attack : AttackBase
{
    public enum AttackState
    {
        Idle,
        Attacking,
        Combo
    };

    public AttackState currentState { get; private set; }

    [SerializeField] private float comboWindowTime = 0.5f; // Time to trigger a combo attack
    [SerializeField] private float clickBuffer = 0.25f; // Prevent button mashing
    private float lastAttackTime;
    private bool attackQueued;

    void Start()
    {
        currentState = AttackState.Idle;
        lastAttackTime = -comboWindowTime; // Ensure first attack can trigger
        attackQueued = false;
    }

    override public bool AttackTrigger()
    {
        return Input.GetMouseButtonDown(0);
    }

    override public void Attack()
    {
        float currentTime = Time.time;

        // Ignore attacks if clicking too fast
        if (currentTime - lastAttackTime < clickBuffer)
        {
            Debug.Log("Clicked too soon!");
            return;
        }

        if (equippedWeaponController.isDrawn())
        {
            if (currentState == AttackState.Idle)
            {
                StartAttack();
            }
            else if (currentState == AttackState.Attacking && (currentTime - lastAttackTime <= comboWindowTime))
            {
                attackQueued = true;
                Debug.Log("Attack queued for combo!");
            }
        }
        else
        {
            Debug.Log("Weapon not Drawn");
            equippedWeaponController.DrawWeapon(true);
        }
    }

    private void StartAttack()
    {
        Debug.Log("Starting attack!");
        lastAttackTime = Time.time;
        animationManager.SetBool("isAttacking", true);
        currentState = AttackState.Attacking;
    }

    // Called by an Animation Event at the end of an attack animation
    public void AttackEnd()
    {
        Debug.Log("Attack ended. Current state: " + currentState);

        if (attackQueued)
        {
            Debug.Log("Executing queued attack!");
            attackQueued = false;
            currentState = AttackState.Combo;
            StartAttack();
        }
        else
        {
            Debug.Log("Returning to idle.");
            currentState = AttackState.Idle;
            animationManager.SetBool("isAttacking", false); // Reset the attacking flag
        }
    }
}