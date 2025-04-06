using System.Collections;
using System.Collections.Generic;
using Frayed.Input;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

[RequireComponent(typeof(MC_AudioManager))]
public class MC_AnimationManager : AnimationManager
{

    public string animationParameterSpeed { get { return _animationParameterSpeed; } private set { _animationParameterSpeed = value; } }
    public string animationParameterSprintHeld { get { return _animationParameterSprintHeld; } private set { _animationParameterSprintHeld = value; } }

    [Header("Animation Parameters")]
    [Tooltip("The animation parameter for speed")]
    [SerializeField]
    private string _animationParameterSpeed = "speed";
    [Tooltip("The animation parameter for if sprint is held")]
    [SerializeField]
    private string _animationParameterSprintHeld = "sprintHeld";

    private InputManager inputManager;
    private MC_AudioManager audioManager;

    public void SetSpeed(float speed)
    {
        SetFloat(_animationParameterSpeed, speed);
    }
    
    new protected void Start()
    {
        base.Start();

        inputManager = InputManager.Instance;
        Debug.Assert(inputManager != null);

        audioManager = GetComponent<MC_AudioManager>();
        Debug.Assert(audioManager != null);
    }

    private void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            audioManager.PlayFootStepAudio();
        }
    }

    private void Update()
    {
        SetBool(_animationParameterSprintHeld, inputManager.sprint);
    }

}
