using System.Collections;
using System.Collections.Generic;
using Frayed.Input;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

[RequireComponent(typeof(MC_Locomotion))]
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


    [SerializeField]
    private AudioClip[] footstepAudioClips;

    [SerializeField]
    private AudioClip environmentPainSound;

    [Range(0, 1)]
    [SerializeField]
    private float footstepAudioVolume = 0.33f;

    [Range(0, 1)]
    [SerializeField]
    private float environmentPainSoundVolume = 0.8f;

    private MC_Locomotion locomotion;
    private InputManager inputManager;

    public void SetSpeed(float speed)
    {
        SetFloat(_animationParameterSpeed, speed);
    }
    
    new protected void Start()
    {
        base.Start();
        locomotion = GetComponent<MC_Locomotion>();
        Debug.Assert(locomotion != null);

        inputManager = InputManager.Instance;
        Debug.Assert(inputManager != null);
    }

    private void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f && locomotion.grounded)
        {
            if (footstepAudioClips.Length > 0)
            {
                var index = Random.Range(0, footstepAudioClips.Length);
                AudioSource.PlayClipAtPoint(footstepAudioClips[index], transform.TransformPoint(locomotion.position), footstepAudioVolume);
            }
        }
    }

    public void EnvironmentPain()
    {
        AudioSource.PlayClipAtPoint(environmentPainSound, transform.TransformPoint(locomotion.position), environmentPainSoundVolume);
    }

    private void Update()
    {
        SetBool(_animationParameterSprintHeld, inputManager.sprint);
    }

}
