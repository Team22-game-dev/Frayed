using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(EnemyData))]
[RequireComponent(typeof(AnimationManager))]
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

    private AnimationManager animationManager;

    [SerializeField]
    private State _currentState;
    [SerializeField]
    private NavMeshAgent _navMeshAgent;
    [SerializeField]



    private float timeSinceAttack;
    private float timeStanding;
    private float timeStandingUntilWandering;

    public EnemyData enemyData;
    private GameObject chasingColliderGameObject;
    private SphereCollider chasingCollider;
    private GameObject attackingColliderGameObject;
    private SphereCollider attackingCollider;
    private CharacterController characterController;

    private void Start()
    {
        enemyData = GetComponent<EnemyData>();
        animationManager = GetComponent<AnimationManager>();

        if(animationManager == null)
            Debug.LogError("Amalgam animationManager is null");

        // Create child game object for chasing sphere collider since a game object can only have one collider.
        AddCollider(ref chasingColliderGameObject, ref chasingCollider, "Chasing Collider", enemyData.chaseRadius, ChasingColliderEnter, ChasingColliderExit, "PlayerDetect");

        //chasingColliderGameObject.layer = LayerMask.NameToLayer("PlayerDetection");
        // Create child game object for attacking sphere collider since a game object can only have one collider.
        AddCollider(ref attackingColliderGameObject, ref attackingCollider, "Attacking Collider", enemyData.attackRadius, AttackingColliderEnter, AttackingColliderExit, "Default");

        // Configure CharacterController
        characterController = GetComponent<CharacterController>();
        if (characterController == null)
        {
            characterController = this.gameObject.AddComponent<CharacterController>();

            characterController.height = 2.25f;
            characterController.center = new Vector3(0,1,0);
        }

        // Configure NavMeshAgent
        _navMeshAgent = GetComponent<NavMeshAgent>();
        if (_navMeshAgent == null)
        {
            _navMeshAgent = this.gameObject.AddComponent<NavMeshAgent>();
        }
        ChangeState(State.STANDING);

       
    }

    // Creates a new GameObject for colliderGameObject and a new SphereCollider for collider.
    void AddCollider(ref GameObject colliderGameObject, ref SphereCollider collider, string colliderName, 
    float colliderRadius, Action<Collider> colliderEnter, Action<Collider> colliderExit, string layer)
    {
        colliderGameObject = new GameObject(colliderName);
        colliderGameObject.layer = LayerMask.NameToLayer(layer);
        colliderGameObject.transform.SetParent(this.transform);
        colliderGameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
        collider = colliderGameObject.AddComponent<SphereCollider>();
        ChildColliderScript colliderScript = collider.AddComponent<ChildColliderScript>();
        colliderScript.OnTriggerEnterAction += colliderEnter;
        colliderScript.OnTriggerExitAction += colliderExit;
        collider.isTrigger = true;
        collider.radius = colliderRadius;
        collider.enabled = true;
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
        UpdateAnimationToState();
        
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
        //_navMeshAgent.SetDestination(enemyData.mainCharacter.transform.position);
        TrySetDestination(enemyData.mainCharacter.transform.position);
        if (ReachedDestination())
        {
            MoveTowardPlayerWithoutNavAgent(enemyData.chaseSpeed);
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
        MoveTowardPlayerWithoutNavAgent(enemyData.chaseSpeed);

        // TODO: Will probably have to improve on this logic.
        timeSinceAttack += Time.deltaTime;
        if (timeSinceAttack >= enemyData.attackDuration)
        {
            ChangeState(State.READY_TO_ATTACK);
        }
    }

    private void TrySetDestination(Vector3 desiredDestination)
    {
        NavMeshPath path = new NavMeshPath();
        NavMesh.CalculatePath(transform.position, desiredDestination, NavMesh.AllAreas, path);
        if (path.corners.Length > 0)
        {
            // Go to the last location of the path to the desiredDestination (even if it's unreachable).
            //_navMeshAgent.SetDestination(path.corners.Last());
            _navMeshAgent.SetPath(path);
            //Debug.Log(path.corners.Last());
        }
    }

    private void MoveTowardPlayerWithoutNavAgent(float speed)
    {
        Vector3 playerHorizontalPosition = new Vector3(enemyData.mainCharacter.transform.position.x, 0f, enemyData.mainCharacter.transform.position.z);
        Vector3 enemyHorizontalPosition = new Vector3(transform.position.x, 0f, transform.position.z);

        Vector3 direction = playerHorizontalPosition - enemyHorizontalPosition;
        // Make sure to look toward the player if not handled by the navAgent.
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, enemyData.angularSpeed * Time.deltaTime);

        float horizontalDistance = (playerHorizontalPosition - enemyHorizontalPosition).magnitude;
        if (horizontalDistance >= enemyData.attackRadius)
        {
            navMeshAgent.ResetPath();
            navMeshAgent.velocity = direction.normalized * speed;
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
                TrySetDestination(randomPosition);
                _navMeshAgent.isStopped = false;
                _navMeshAgent.speed = enemyData.wanderSpeed;
                _navMeshAgent.stoppingDistance = 1f;
                _navMeshAgent.angularSpeed = enemyData.angularSpeed;
                break;
            case State.CHASING:
                _navMeshAgent.isStopped = false;
                _navMeshAgent.speed = enemyData.chaseSpeed;
                _navMeshAgent.stoppingDistance = enemyData.attackRadius;
                _navMeshAgent.angularSpeed = enemyData.angularSpeed;
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

    private void AttackingColliderEnter(Collider other)
    {
        if (other.gameObject == enemyData.mainCharacter)
        {
            ChangeState(State.READY_TO_ATTACK);
        }
    }

    private void AttackingColliderExit(Collider other)
    {
        if (other.gameObject == enemyData.mainCharacter)
        {
            ChangeState(State.CHASING);
        }
    }

    private void UpdateAnimationToState()
    {
        animationManager.SetFloat("speed", _navMeshAgent.velocity.magnitude);
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