using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MC_Locomotion))]
public class MC_AudioManager : MonoBehaviour
{

    [SerializeField]
    private AudioClip[] footstepAudioClips;

    [SerializeField]
    private AudioClip landingAudioClip;

    [SerializeField]
    private AudioClip environmentPainSound;

    [Range(0, 1)]
    [SerializeField]
    private float footstepAudioVolume = 0.33f;

    [Range(0, 1)]
    [SerializeField]
    private float landingAudioVolume = 0.5f;

    [Range(0, 1)]
    [SerializeField]
    private float environmentPainSoundVolume = 0.8f;

    private MC_Locomotion locomotion;
    public void Start()
    {
        locomotion = GetComponent<MC_Locomotion>();
        Debug.Assert(locomotion != null);

        Debug.Assert(landingAudioClip != null);
        Debug.Assert(footstepAudioClips.Length > 0);
        Debug.Assert(environmentPainSound != null);
    }

    public void PlayFootStepAudio()
    {
        if (footstepAudioClips.Length > 0 && locomotion.grounded)
        {
            var index = Random.Range(0, footstepAudioClips.Length);
            AudioSource.PlayClipAtPoint(footstepAudioClips[index], transform.TransformPoint(locomotion.position), footstepAudioVolume);
        }
    }

    public void PlayLandingAudio()
    {
        if (landingAudioClip != null)
        {
            AudioSource.PlayClipAtPoint(landingAudioClip, transform.TransformPoint(locomotion.position), landingAudioVolume);
        }
    }

    public void PlayEnvironmentPainAudio()
    {
        if (environmentPainSound != null)
        {
            AudioSource.PlayClipAtPoint(environmentPainSound, transform.TransformPoint(locomotion.position), environmentPainSoundVolume);
        }
    }
}
