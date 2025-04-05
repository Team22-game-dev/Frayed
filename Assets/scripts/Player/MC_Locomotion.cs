using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Frayed.Camera;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
using Frayed.Input;


[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(MC_AnimationManager))]
[RequireComponent(typeof(MC_Attack))]
public class MC_Locomotion : MonoBehaviour
{

    [Header("Player")]
    [Tooltip("Move speed of the character in m/s")]
    [SerializeField]
    private float moveSpeed = 2.0f;

    [Tooltip("Sprint speed of the character in m/s")]
    [SerializeField]
    private float sprintSpeed = 5.335f;

    [Tooltip("Attack speed of the character in m/s")]
    [SerializeField]
    private float attackSpeed = 0.4f;

    [Tooltip("How fast the character turns to face movement direction")]
    [Range(0.0f, 0.3f)]
    [SerializeField]
    private float rotationSmoothTime = 0.12f;

    [Tooltip("Acceleration and deceleration")]
    [SerializeField]
    private float speedChangeRate = 10.0f;

    [Space(10)]
    [Tooltip("The height the player can jump")]
    [SerializeField]
    private float jumpHeight = 1.2f;

    [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
    [SerializeField]
    private float gravity = -15.0f;

    [Space(10)]
    [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
    [SerializeField]
    private float jumpTimeout = 0.50f;

    [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
    [SerializeField]
    private float fallTimeout = 0.15f;

    [Tooltip("Useful for rough ground")]
    [SerializeField]
    private float groundedOffset = -0.14f;

    [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
    [SerializeField]
    private float groundedRadius = 0.28f;

    [Tooltip("What layers the character uses as ground")]
    [SerializeField]
    private LayerMask groundLayers;

    public bool grounded { get { return _grounded; } set { _grounded = value; } }
    public float speed { get { return _speed; } set { _speed = value; } }
    public float animationBlend { get { return _animationBlend; } set { _animationBlend = value; } }
    public float targetRotation { get { return _targetRotation; } set { _targetRotation = value; } }
    public float rotationVelocity { get { return _rotationVelocity; } set { _rotationVelocity = value; } }
    public float verticalVelocity { get { return _verticalVelocity; } set { _verticalVelocity = value; } }
    public float terminalVelocity { get { return _terminalVelocity; } set { _terminalVelocity = value; } }
    public Vector3 position { get { return _position; } set { _position = value; } }


    // Player
    private bool _grounded = true;
    private float _speed;
    private float _animationBlend;
    private float _targetRotation = 0.0f;
    private float _rotationVelocity;
    private float _verticalVelocity;
    private float _terminalVelocity = 53.0f;
    private Vector3 _position;

    // Timeout deltatime
    private float jumpTimeoutDelta;
    private float fallTimeoutDelta;

    private CharacterController _controller;
    private MC_AnimationManager animationManager;
    private InputManager inputManager;
    private CameraManager cameraManager;
    private MC_Attack attack;


    private void Awake()
    {

    }

    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        // reset our timeouts on start
        jumpTimeoutDelta = jumpTimeout;
        fallTimeoutDelta = fallTimeout;

        inputManager = InputManager.Instance;
        Debug.Assert(inputManager != null);

        cameraManager = CameraManager.Instance;
        Debug.Assert(cameraManager != null);

        animationManager = GetComponent<MC_AnimationManager>();

        attack = GetComponent<MC_Attack>();
    }

    private void Update()
    {
        JumpAndGravity();
        GroundedCheck();
        animationManager.SetBool("Grounded", _grounded);
        Move();
    }

    private void GroundedCheck()
    {
        // set sphere position, with offset
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - groundedOffset,
            transform.position.z);

        _grounded = Physics.CheckSphere(spherePosition, groundedRadius, groundLayers,
            QueryTriggerInteraction.Ignore);

    }

    private void Move()
    {
        // Set target speed based on conditions.
        float targetSpeed;
        if (attack.currentState != MC_Attack.AttackState.Idle)
        {
            targetSpeed = attackSpeed;
        } else if (inputManager.sprint)
        {
            targetSpeed = sprintSpeed;
        } else
        {
            targetSpeed = moveSpeed;
        }

        // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

        // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is no input, set the target speed to 0
        if (inputManager.move == Vector2.zero) targetSpeed = 0.0f;

        // a reference to the players current horizontal velocity
        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

        float speedOffset = 0.1f;
        float inputMagnitude = 1f;

        // accelerate or decelerate to target speed
        if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            // creates curved result rather than a linear one giving a more organic speed change
            // note T in Lerp is clamped, so we don't need to clamp our speed
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                Time.deltaTime * speedChangeRate);

            // round speed to 3 decimal places
            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }

        _animationBlend = Mathf.Lerp(_animationBlend, _speed, Time.deltaTime * speedChangeRate);
        if (_animationBlend < 0.01f) _animationBlend = 0f;

        // normalise input direction
        Vector3 inputDirection = new Vector3(inputManager.move.x, 0.0f, inputManager.move.y).normalized;

        // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is a move input rotate player when the player is moving
        if (inputManager.move != Vector2.zero)
        {

            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                              cameraManager.mainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                rotationSmoothTime);

            // rotate to face input direction relative to camera position
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }

        Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

        // move the player
        if (_controller.enabled)
        {
            _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                            new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

        }

        _position = _controller.center;
        // Update animation
        animationManager.SetSpeed(_animationBlend);
    }

    private void JumpAndGravity()
    {
        if (_grounded)
        {
            // reset the fall timeout timer
            fallTimeoutDelta = fallTimeout;

            // stop our velocity dropping infinitely when grounded
            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2f;
            }
            // Jump
            if (inputManager.jump && jumpTimeoutDelta <= 0.0f)
            {
                if(_speed>0)
                {
                    animationManager.SetTrigger("JumpFoward");
                }
                else
                {
                    animationManager.SetTrigger("JumpInPlace");
                }
                
                // the square root of H * -2 * G = how much velocity needed to reach desired height
                _verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

            }

            // jump timeout
            if (jumpTimeoutDelta >= 0.0f)
            {
                jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            // reset the jump timeout timer
            jumpTimeoutDelta = jumpTimeout;

            // fall timeout
            if (fallTimeoutDelta >= 0.0f)
            {
                fallTimeoutDelta -= Time.deltaTime;
            }

            // if we are not grounded, do not jump
            inputManager.jump = false;
        }

        // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
        if (_verticalVelocity < _terminalVelocity)
        {
            _verticalVelocity += gravity * Time.deltaTime;
        }
    }
}
