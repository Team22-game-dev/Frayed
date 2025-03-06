using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Frayed.Input;
using UnityEngine.Windows;

namespace Frayed.Camera
{    
    public class CameraManager : MonoBehaviour
    {

        // Singleton Design
        private static CameraManager _instance;
        public static CameraManager Instance => _instance;

        private InputManager inputManager;

        public GameObject mainCamera { get { return _mainCamera; } private set { _mainCamera = value; } }

        [SerializeField]
        private GameObject _mainCamera;
        [Tooltip("For locking the camera position on all axis")]
        [SerializeField]
        private bool _lockCameraPosition = false;

        [SerializeField]
        private float lookThreshold = 0.01f;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        [SerializeField]
        private GameObject cinemachineCameraTarget;

        [Tooltip("How far in degrees can you move the camera up")]
        [SerializeField]
        private float topClamp = 70.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        [SerializeField]
        private float bottomClamp = -30.0f;

        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        [SerializeField]
        private float cameraAngleOverride = 0.0f;

        // cinemachine
        private float cinemachineTargetYaw;
        private float cinemachineTargetPitch;

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
