using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    private Animator animator;

    protected void Start()
    {
        // Get Animator component
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError($"{nameof(Animator)} component not found on the GameObject.");
        }
    }

    // Set an integer parameter in the Animator
    public void SetInt(string parameterName, int value)
    {
        if (animator != null)
        {
            animator.SetInteger(parameterName, value);
        }
        else
        {
            Debug.LogWarning($"{nameof(Animator)} not set in {nameof(AnimationManager)}.");
        }
    }

    // Set a float parameter in the Animator
    public void SetFloat(string parameterName, float value)
    {
        if (animator != null)
        {
            animator.SetFloat(parameterName, value);
        }
        else
        {
            Debug.LogWarning($"{nameof(Animator)} not set in {nameof(AnimationManager)}.");
        }
    }

    // Set a boolean parameter in the Animator
    public void SetBool(string parameterName, bool value)
    {
        if (animator != null)
        {
            animator.SetBool(parameterName, value);
        }
        else
        {
            Debug.LogWarning($"{nameof(Animator)} not set in {nameof(AnimationManager)}.");
        }
    }

    // Trigger a parameter in the Animator
    public void SetTrigger(string parameterName)
    {
        if (animator != null)
        {
            animator.SetTrigger(parameterName);
        }
        else
        {
            Debug.LogWarning($"{nameof(Animator)} not set in {nameof(AnimationManager)}.");
        }
    }

    // Get the name of the currently playing animation asynchronously
    public string GetCurrentAnimationName()
    {
        Debug.Log("Getting Animation name from AnimationManager");
        int layerIndex = 0;
        if (animator == null)
        {
            Debug.LogWarning($"{nameof(Animator)} not set in {nameof(AnimationManager)}.");
            return null;
        }

        // Wait for the Animator to finish transitioning
        if (animator.IsInTransition(layerIndex))
        {
            return "transition";
        }

        // Get the current AnimatorStateInfo
        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(layerIndex);
        if (clipInfo.Length > 0)
        {
            return clipInfo[0].clip.name;
        }

        return null; // No animation found
    }
}
