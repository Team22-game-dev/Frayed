using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Frayed.Input;
using UnityEngine.Windows;
using Cinemachine;

namespace Frayed.Camera
{    
    public class CameraManager : MonoBehaviour
    {

        // Singleton Design
        private static CameraManager _instance;
        public static CameraManager Instance => _instance;

        public enum Mode
        {
            None,
            ThirdPersonStandard,
            ThirdPersonCombat
        }

        public Mode mode = Mode.ThirdPersonStandard;

        private InputManager inputManager;

        public GameObject mainCamera { get { return _mainCamera; } private set { _mainCamera = value; } }

        [SerializeField]
        private GameObject _mainCamera;
        [Tooltip("For locking the camera position on all axis")]
        [SerializeField]
        private bool _lockCameraPosition = false;

        [SerializeField]
        private float lookThreshold = 0.01f;

        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        [SerializeField]
        private GameObject cinemachineCameraTarget;

        [Tooltip("How far in degrees can you move the camera up")]
        private float topClamp = 70.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        private float bottomClamp = -15.0f;

        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        [SerializeField]
        private float cameraAngleOverride = 0.0f;

        // cinemachine
        private float cinemachineTargetYaw;
        private float cinemachineTargetPitch;
        private GameObject cinemachineVirtualCameraGameObject;
        private CinemachineVirtualCamera cinemachineVirtualCamera;

        private void Awake()
        {
            // Singleton pattern with explicit null check
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            // get a reference to our main camera
            if (mainCamera == null)
            {
                mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            inputManager = InputManager.Instance;
            Debug.Assert(inputManager != null);

            mode = Mode.None;
        }

        private void FixedUpdate()
        {
            cinemachineVirtualCameraGameObject = GetComponent<CinemachineBrain>().ActiveVirtualCamera.VirtualCameraGameObject;
            Debug.Assert(cinemachineVirtualCameraGameObject != null);

            cinemachineVirtualCamera = cinemachineVirtualCameraGameObject.GetComponent<CinemachineVirtualCamera>();
            Debug.Assert(cinemachineVirtualCamera != null);

            if (mode == Mode.None)
            {
                ChangeMode(Mode.ThirdPersonStandard);
            }
        }

        private void Update()
        {
            if (inputManager.switchCameraView)
            {
                switch (mode)
                {
                    case Mode.ThirdPersonStandard:
                        ChangeMode(Mode.ThirdPersonCombat);
                        break;
                    case Mode.ThirdPersonCombat:
                        ChangeMode(Mode.ThirdPersonStandard);
                        break;
                    default:
                        Debug.Assert(false);
                        break;
                }
            }

            // Reset camera input buttons.
            inputManager.switchCameraView = false;
        }

        public void ChangeMode(Mode newMode)
        {
            mode = newMode;
            Cinemachine3rdPersonFollow cinemachine3rdPersonFollow = cinemachineVirtualCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
            Debug.Assert(cinemachine3rdPersonFollow != null);
            switch (mode)
            {
                case Mode.ThirdPersonStandard:
                    topClamp = 70.0f;
                    bottomClamp = -15.0f;
                    cinemachine3rdPersonFollow.ShoulderOffset.y = 0.0f;
                    cinemachine3rdPersonFollow.CameraSide = 0.6f;
                    cinemachine3rdPersonFollow.CameraDistance = 4.0f;
                    break;
                case Mode.ThirdPersonCombat:
                    topClamp = 10.0f;
                    bottomClamp = -15.0f;
                    cinemachine3rdPersonFollow.ShoulderOffset.y = 0.25f;
                    cinemachine3rdPersonFollow.CameraSide = 0.8f;
                    cinemachine3rdPersonFollow.CameraDistance = 2.0f;
                    break;
                default:
                    Debug.Assert(false);
                    break;
            }
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        private void CameraRotation()
        {
            // if there is an input and camera position is not fixed
            if (inputManager.look.sqrMagnitude >= lookThreshold && !_lockCameraPosition)
            {
                //Don't multiply mouse input by Time.deltaTime;
                float deltaTimeMultiplier = inputManager.IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                cinemachineTargetYaw += inputManager.look.x * deltaTimeMultiplier;
                cinemachineTargetPitch += inputManager.look.y * deltaTimeMultiplier;
            }

            // clamp our rotations so our values are limited 360 degrees
            cinemachineTargetYaw = ClampAngle(cinemachineTargetYaw, float.MinValue, float.MaxValue);
            cinemachineTargetPitch = ClampAngle(cinemachineTargetPitch, bottomClamp, topClamp);

            // Cinemachine will follow this target
            cinemachineCameraTarget.transform.rotation = Quaternion.Euler(cinemachineTargetPitch + cameraAngleOverride,
                cinemachineTargetYaw, 0.0f);
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

    }

}
