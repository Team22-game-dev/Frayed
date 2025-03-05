using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MC_Locomotion : MonoBehaviour
{
    // Multipliers for character speed
    [SerializeField] private float[] speed = { 2f, 4f };

    private AnimationManager animationManager;

    private void Awake()
    {
        // Get the AnimationManager component on the same GameObject
        animationManager = GetComponent<AnimationManager>();

        if (animationManager == null)
        {
            Debug.LogError("AnimationManager component not found on this GameObject.");
        }
    }



    // Start is called before the first frame update
    void Start()
    {
        // Get the Animator component attached to the GameObject
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the player is walking forward
        bool isWalking = Input.GetKey(KeyCode.W); // Walking forward

        // Check if the player is running (walking + holding Left Shift)
        bool isRunning = isWalking && Input.GetKey(KeyCode.LeftShift);

        if(isWalking) Debug.Log("Is Walinkg!");

        // Calculate movement direction and speed
        float moveInput = Input.GetAxis("Vertical"); // Get vertical input (W/S or Up/Down Arrow)
        float currentSpeed = isRunning ? speed[1] : speed[0]; // Choose speed based on running state
        Vector3 velocity = Vector3.forward * moveInput * currentSpeed;

        // Move the character
        transform.Translate(velocity * Time.deltaTime);


        //AnimationManager.SetFloat("speed", velocity.magnitude);
    }


}
