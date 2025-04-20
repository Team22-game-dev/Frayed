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

        public int disableInputCount { get { return _disableInputCount; } set { _disableInputCount = value; } }
        public bool inputDisabled { get { return (_disableInputCount > 0); } }
        public Vector2 move { get { return _move; } private set { _move = value; } }
        public Vector2 look { get { return _look; } private set { _look = value; } }
        // Public setter and getter.
        public bool jump { get { return _jump; } set { _jump = value; } }
        public bool sprint { get { return _sprint; } private set { _sprint = value; } }
        // Public setter and getter.
        public bool toggleOptionsMenu { get { return _toggleOptionsMenu; } set { _toggleOptionsMenu = value; } }
        public bool toggleInventory { get { return _toggleInventory; } set { _toggleInventory = value; } }
        public bool inventoryNextItem { get { return _inventoryNextItem; } private set { _inventoryNextItem = value; } }
        public bool inventoryPrevItem { get { return _inventoryPrevItem; } private set { _inventoryPrevItem = value; } }
        public bool inventoryDropWeapon { get { return _inventoryDropWeapon; } set { _inventoryDropWeapon = value; } }
        public bool switchCameraView { get { return _switchCameraView; } set { _switchCameraView = value; } }
        public bool movementLocked { get { return _movementLocked; } private set { _movementLocked = value; } }
        public bool cursorLocked { get { return _cursorLocked; } private set { _cursorLocked = value; } }
        public bool cursorInputForLook { get { return _cursorInputForLook; } private set { _cursorInputForLook = value; } }

        [Header("Input Manager settings")]
        [SerializeField]
        public int _disableInputCount = 0;


        [Header("Player Input Values")]
        [SerializeField]
        private Vector2 _move;
        [SerializeField]
        private Vector2 _look;
        [SerializeField]
        private bool _jump;
        [SerializeField]
        private bool _sprint;
        [SerializeField]
        private bool _toggleOptionsMenu;
        [SerializeField]
        private bool _toggleInventory;
        [SerializeField]
        private bool _inventoryNextItem;
        [SerializeField]
        private bool _inventoryPrevItem;
        [SerializeField]
        private bool _inventoryDropWeapon;
        [SerializeField]
        private bool _switchCameraView;

        [Header("Movement Settings")]
        private bool _movementLocked = false;

        [Header("Mouse Cursor Settings")]
        private bool _cursorLocked = true;
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
            _disableInputCount = 0;
        }

        private void Start()
        {
            playerInput = GetComponent<PlayerInput>();
        }

        private void Update()
        {
            if (inputDisabled)
            {
                _move = new Vector2(0.0f, 0.0f);
                _look = new Vector2(0.0f, 0.0f);
                _jump = false;
                _sprint = false;
                _toggleInventory = false;
                _inventoryNextItem = false;
                _inventoryPrevItem = false;
                _inventoryDropWeapon = false;
                _switchCameraView = false;
            }
        }

#if ENABLE_INPUT_SYSTEM

        public void OnMove(InputValue value)
        {
            if (inputDisabled || movementLocked)
            {
                return;
            }
            MoveInput(value.Get<Vector2>());
        }

        public void OnLook(InputValue value)
        {
            if (inputDisabled || !cursorInputForLook)
            {
                return;
            }
            LookInput(value.Get<Vector2>());
        }

        public void OnJump(InputValue value)
        {
            if (inputDisabled || movementLocked)
            {
                return;
            }
            JumpInput(value.isPressed);
        }

        public void OnSprint(InputValue value)
        {
            if (inputDisabled || movementLocked)
            {
                return;
            }
            SprintInput(value.isPressed);
        }

        public void OnToggleOptionsMenu(InputValue value)
        {
            ToggleOptionsMenuInput(value.isPressed);
        }

        public void OnToggleInventory(InputValue value)
        {
            if (inputDisabled)
            {
                return;
            }
            ToggleInventoryInput(value.isPressed);
        }

        public void OnInventoryNextItem(InputValue value)
        {
            if (inputDisabled)
            {
                return;
            }
            InventoryNextItemInput(value.isPressed);
        }

        public void OnInventoryPrevItem(InputValue value)
        {
            if (inputDisabled)
            {
                return;
            }
            InventoryPrevItemInput(value.isPressed);
        }

        public void OnInventoryDropWeapon(InputValue value)
        {
            if (inputDisabled)
            {
                return;
            }
            InventoryDropWeaponInput(value.isPressed);
        }

        public void OnSwitchCameraView(InputValue value)
        {
            if (inputDisabled)
            {
                return;
            }
            SwitchCameraViewInput(value.isPressed);
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
            // Always be true due to Action being button.
            _jump = newJumpState;
        }

        public void SprintInput(bool newSprintState)
        {
            _sprint = newSprintState;
        }

        public void ToggleOptionsMenuInput(bool newToggleOptionsMenuState)
        {
            // Always be true due to Action being button.
            _toggleOptionsMenu = newToggleOptionsMenuState;
        }

        public void ToggleInventoryInput(bool newToggleInventoryState)
        {
            _toggleInventory = newToggleInventoryState;
        }

        public void InventoryNextItemInput(bool newInventoryNextItemState)
        {
            _inventoryNextItem = newInventoryNextItemState;
        }

        public void InventoryPrevItemInput(bool newInventoryPrevItemState)
        {
            _inventoryPrevItem = newInventoryPrevItemState;
        }

        public void InventoryDropWeaponInput(bool newInventoryDropWeaponState)
        {
            // Always be true due to Action being button.
            _inventoryDropWeapon = newInventoryDropWeaponState;
        }

        public void SwitchCameraViewInput(bool newSwitchCameraViewState)
        {
            // Always be true due to Action being button.
            _switchCameraView = newSwitchCameraViewState;
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            SetCursorState(_cursorLocked);
        }

        private void SetCursorState(bool newState)
        {
            Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
        }

        public void LockMovement()
        {
            _movementLocked = true;
            // Stop all current movement.
            _move = new Vector2(0f, 0f);
        }

        public void UnlockMovement()
        {
            _movementLocked = false;
        }

        public void LockMouse()
        {
            _cursorLocked = true;
            _cursorInputForLook = true;
            SetCursorState(true);
        }

        public void UnlockMouse()
        {
            _cursorLocked = false;
            _cursorInputForLook = false;
            SetCursorState(false);
            // Stop all camera movement.
            _look = new Vector2(0f, 0f);
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
