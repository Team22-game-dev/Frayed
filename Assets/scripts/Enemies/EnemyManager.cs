using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(EnemyData))]
public class EnemyManager : MonoBehaviour
{
    public enum State
    {
        STANDING, // Doing nothing
        WANDERING, // Wandering around
        CHASING, // Chasing player
        READY_TO_ATTACK, // Ready to attack a player
        ATTACKING, // Attacking player
    }

    public State currentState { get { return _currentState; } private set { _currentState = value; } }
    public NavMeshAgent navMeshAgent { get { return _navMeshAgent; } private set { _navMeshAgent = value; } }

    [SerializeField]
    private State _currentState;
    [SerializeField]
    private NavMeshAgent _navMeshAgent;

    private float timeSinceAttack;
    private float timeStanding;
    private float timeStandingUntilWandering;

    private EnemyData enemyData;
    private GameObject chasingColliderGameObject;
    private SphereCollider chasingCollider;
    private CharacterController characterController;

    private void Start()
    {
        enemyData = GetComponent<EnemyData>();

        // Create child game object for chasing sphere collider since a game object can only have one collider.
        chasingColliderGameObject = new GameObject("Chasing Collider");
        chasingColliderGameObject.transform.SetParent(this.transform);
        chasingCollider = chasingColliderGameObject.AddComponent<SphereCollider>();
        ChildColliderScript chasingColliderScript = chasingCollider.AddComponent<ChildColliderScript>();
        chasingColliderScript.OnTriggerEnterAction += ChasingColliderEnter;
        chasingColliderScript.OnTriggerExitAction += ChasingColliderExit;
        chasingCollider.isTrigger = true;
        chasingCollider.radius = enemyData.chaseRadius;
        chasingCollider.enabled = true;

        // Configure CharacterController
        characterController = GetComponent<CharacterController>();
        if (characterController == null)
        {
            characterController = this.gameObject.AddComponent<CharacterController>();
        }

        // Configure NavMeshAgent
        _navMeshAgent = GetComponent<NavMeshAgent>();
        if (_navMeshAgent == null)
        {
            _navMeshAgent = this.gameObject.AddComponent<NavMeshAgent>();
        }
        _navMeshAgent.baseOffset = enemyData.height / 2;
        ChangeState(State.STANDING);
    }

    private void Update()
    {
        switch (_currentState)
        {
            case State.STANDING:
                HandleStandingState();
                break;
            case State.WANDERING:
                HandleWanderingState();
                break;
            case State.CHASING:
                HandleChasingState();
                break;
            case State.READY_TO_ATTACK:
                HandleReadyToAttackState();
                break;
            case State.ATTACKING:
                HandleAttackingState();
                break;
            default:
                Debug.Assert(false);
                break;

        }
        
    }

    private void HandleStandingState()
    {
        timeStanding += Time.deltaTime;
        if (enemyData.enableWanderBehavior)
        {
            if (timeStanding >= timeStandingUntilWandering)
            {
                ChangeState(State.WANDERING);
            }
        }
    }

    private void HandleWanderingState()
    {
        if (ReachedDestination())
        {
            ChangeState(State.STANDING);
        }
    }

    private void HandleChasingState()
    {
        _navMeshAgent.SetDestination(enemyData.mainCharacter.transform.position);
        if (ReachedDestination())
        {
            ChangeState(State.READY_TO_ATTACK);
        }
    }

    private void HandleReadyToAttackState()
    {
        timeSinceAttack = 0f;
        //Debug.Log("TODO: Attacking!");
        ChangeState(State.ATTACKING);
    }

    private void HandleAttackingState()
    {
        // TODO: Will probably have to improve on this logic.
        timeSinceAttack += Time.deltaTime;
        if (timeSinceAttack >= enemyData.attackDuration)
        {
            ChangeState(State.CHASING);
        }
    }

    private bool ReachedDestination()
    {
        return _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance;
    }

    private void ChangeState(State newState)
    {
        switch (newState)
        {
            case State.STANDING:
                _navMeshAgent.SetDestination(this.transform.position);
                _navMeshAgent.isStopped = true;
                _navMeshAgent.speed = 0f;
                timeStanding = 0f;
                if (enemyData.enableWanderBehavior)
                {
                    timeStandingUntilWandering = UnityEngine.Random.Range(enemyData.minimumWanderPause, enemyData.maximumWanderPause);
                }
                break;
            case State.WANDERING:
                float wanderingRadius = UnityEngine.Random.Range(0f, enemyData.maximumWanderRadius);
                Vector3 wanderingDirection = UnityEngine.Random.insideUnitSphere * wanderingRadius;
                wanderingDirection.y = 0f;
                Vector3 randomPosition = enemyData.spawnPosition + wanderingDirection;

                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomPosition, out hit, enemyData.height * 2, NavMesh.AllAreas))
                {
                    _navMeshAgent.SetDestination(hit.position);
                    _navMeshAgent.isStopped = false;
                    _navMeshAgent.speed = enemyData.wanderSpeed;
                    _navMeshAgent.stoppingDistance = 2f;
                }
                else
                {
                    _navMeshAgent.SetDestination(randomPosition);
                    _navMeshAgent.isStopped = false;
                    _navMeshAgent.speed = enemyData.wanderSpeed;
                    _navMeshAgent.stoppingDistance = 2f;
                    Debug.Log("Using random position.");
                }
                break;
            case State.CHASING:
                _navMeshAgent.isStopped = false;
                _navMeshAgent.speed = enemyData.chaseSpeed;
                _navMeshAgent.stoppingDistance = enemyData.attackRadius;
                break;
            case State.READY_TO_ATTACK:
                break;
            case State.ATTACKING:
                break;
            default:
                Debug.Assert(false);
                break;

        }
        _currentState = newState;
        //Debug.Log($"Change enemy to state {newState.ToString()}");
    }

    private void ChasingColliderEnter(Collider other)
    {
        if (other.gameObject == enemyData.mainCharacter)
        {
            ChangeState(State.CHASING);
        }
    }

    private void ChasingColliderExit(Collider other)
    {
        if (other.gameObject == enemyData.mainCharacter)
        {
            ChangeState(State.STANDING);
        }
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