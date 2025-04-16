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

        [Header("Character Input Values")]
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

        [Header("Movement Settings")]
        public bool movementLocked = false;

        [Header("Mouse Cursor Settings")]
        public bool cursorLocked = true;
        public bool cursorInputForLook = true;

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
            if (!movementLocked)
            {
                MoveInput(value.Get<Vector2>());
            }
        }

        public void OnLook(InputValue value)
        {
            if (cursorInputForLook)
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

        public void OnToggleOptionsMenu(InputValue value)
        {
            ToggleOptionsMenuInput(value.isPressed);
        }

        public void OnToggleInventory(InputValue value)
        {
            ToggleInventoryInput(value.isPressed);
        }

        public void OnInventoryNextItem(InputValue value)
        {
            InventoryNextItemInput(value.isPressed);
        }

        public void OnInventoryPrevItem(InputValue value)
        {
            InventoryPrevItemInput(value.isPressed);
        }

        public void OnInventoryDropWeapon(InputValue value)
        {
            InventoryDropWeaponInput(value.isPressed);
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
            // Always be true due to Action being button.
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

        private void OnApplicationFocus(bool hasFocus)
        {
            SetCursorState(cursorLocked);
        }

        private void SetCursorState(bool newState)
        {
            Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
        }

        public void LockMovement()
        {
            movementLocked = true;
            // Stop all current movement.
            _move = new Vector2(0f, 0f);
        }

        public void UnlockMovement()
        {
            movementLocked = false;
        }

        public void LockMouse()
        {
            cursorLocked = true;
            cursorInputForLook = true;
            SetCursorState(true);
        }

        public void UnlockMouse()
        {
            cursorLocked = false;
            cursorInputForLook = false;
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
