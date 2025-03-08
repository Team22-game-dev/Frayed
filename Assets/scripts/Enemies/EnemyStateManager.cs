using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(EnemyData))]
public class EnemyStateManager : MonoBehaviour
{
    public enum State
    {
        STANDING, // Doing nothing
        WANDERING, // Wandering around
        CHASING, // Chasing player
        ATTACKING, // Attacking player
    }

    public State currentState { get { return _currentState; } private set { _currentState = value; } }

    [SerializeField]
    private State _currentState;

    private EnemyData enemyData;
    private GameObject chasingColliderGameObject;
    private SphereCollider chasingCollider;

    private void Start()
    {
        enemyData = GetComponent<EnemyData>();
        _currentState = State.STANDING;

        // Create child game object for chasing sphere collider since a game object can only have one collider.
        chasingColliderGameObject = new GameObject("Chasing Collider");
        chasingColliderGameObject.transform.SetParent(this.gameObject.transform);
        chasingCollider = chasingColliderGameObject.AddComponent<SphereCollider>();
        ChildColliderScript chasingColliderScript = chasingCollider.AddComponent<ChildColliderScript>();
        chasingColliderScript.OnTriggerEnterAction += ChasingColliderEnter;
        chasingColliderScript.OnTriggerExitAction += ChasingColliderExit;
        chasingCollider.isTrigger = true;
        chasingCollider.radius = enemyData.chaseRadius;
        chasingCollider.enabled = true;
    }

    private void ChasingColliderEnter(Collider other)
    {
        if (other.gameObject == enemyData.mainCharacter)
        {
            _currentState = State.CHASING;
            Debug.Log("Start chasing main character");
        }
    }

    private void ChasingColliderExit(Collider other)
    {
        if (other.gameObject == enemyData.mainCharacter)
        {
            _currentState = State.STANDING;
            Debug.Log("Stop chasing main character");
        }
    }

    private void Update()
    {
        
    }

    private void HandleStandingState()
    {

    }

    private void HandleWanderingState()
    {

    }

    private void HandleChasingState()
    {

    }

    private void HandleAttackingState()
    {

    }
}

class ChildColliderScript : MonoBehaviour
{
    public Action<Collider> OnTriggerEnterAction;
    public Action<Collider> OnTriggerExitAction;

    private void OnTriggerEnter(Collider other)
    {
        OnTriggerEnterAction?.Invoke(other);
    }

    private void OnTriggerExit(Collider other)
    {
        OnTriggerExitAction?.Invoke(other);
    }
}