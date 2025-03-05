using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Frayed.Input
{

#if ENABLE_INPUT_SYSTEM
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class InputManager : MonoBehaviour
    {
        // Singleton Design
        private static InputManager _instance;
        public static InputManager Instance => _instance;

        public Vector2 move { get { return _move;  } private set { _move = value; } }
        public Vector2 look { get { return _look; } private set { _look = value; } }
        public bool jump { get { return _jump; } set { _jump = value; } }
        public bool sprint { get { return _sprint; } private set { _sprint = value; } }
        public bool cursorLocked { get { return _cursorLocked; } private set { _cursorLocked = value; } }
        public bool cursorInputForLook { get { return _cursorInputForLook; } private set { _cursorInputForLook = value; } }

        [Header("Character Input Values")]
        [SerializeField]
        private Vector2 _move;
        [SerializeField]
        private Vector2 _look;
        [SerializeField]
        private bool _jump;
        [SerializeField]
        private bool _sprint;

        [Header("Mouse Cursor Settings")]
        [SerializeField]
        private bool _cursorLocked  = true;
        [SerializeField]
        private bool _cursorInputForLook = true;

#if ENABLE_INPUT_SYSTEM
        private PlayerInput playerInput;
#endif

        private void Awake()
        {
            // Singleton pattern with explicit null check
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            playerInput = GetComponent<PlayerInput>();
        }


#if ENABLE_INPUT_SYSTEM

        public void OnMove(InputValue value)
        {
            MoveInput(value.Get<Vector2>());
        }

        public void OnLook(InputValue value)
        {
            if (_cursorInputForLook)
            {
                LookInput(value.Get<Vector2>());
            }
        }

        public void OnJump(InputValue value)
        {
            JumpInput(value.isPressed);
        }

        public void OnSprint(InputValue value)
        {
            SprintInput(value.isPressed);
        }

#endif

        public void MoveInput(Vector2 newMoveDirection)
        {
            _move = newMoveDirection;
        }

        public void LookInput(Vector2 newLookDirection)
        {
            _look = newLookDirection;
        }

        public void JumpInput(bool newJumpState)
        {
            _jump = newJumpState;
        }

        public void SprintInput(bool newSprintState)
        {
            _sprint = newSprintState;
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            SetCursorState(_cursorLocked);
        }

        private void SetCursorState(bool newState)
        {
            Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
        }

        public bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return playerInput.currentControlScheme == "KeyboardMouse";
#else
				    return false;
#endif
            }
        }
    }
}
