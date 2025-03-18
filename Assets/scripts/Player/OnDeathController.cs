using System.Collections;
using UnityEngine;

public class OnDeathController : MonoBehaviour
{
    public AnimationManager animationManager; // Animator reference
    public CharacterController characterController; // CharacterController reference
    private Rigidbody[] ragdollBodies; // Rigidbodies added at runtime
    private Collider[] ragdollColliders; // Colliders added at runtime
    private bool isRagdollActive = false; // Prevent multiple activations

    private void Start()
    {
        animationManager = GetComponent<AnimationManager>();
        characterController = GetComponent<CharacterController>();

        // Initialize arrays to empty; they will be populated on ragdoll activation
        ragdollBodies = new Rigidbody[0];
        ragdollColliders = new Collider[0];
    }

    private void SetRagdollState(bool isRagdoll)
    {
        foreach (Rigidbody rb in ragdollBodies)
        {
            rb.isKinematic = !isRagdoll;
            rb.detectCollisions = isRagdoll;
        }

        foreach (Collider col in ragdollColliders)
        {
            col.enabled = isRagdoll;
        }

        if (animationManager != null)
            animationManager.animator.enabled = !isRagdoll;

        if (characterController != null)
            characterController.enabled = !isRagdoll;
    }

    public void ActivateRagdoll(Vector3 forceDirection, float forceStrength)
    {
        if (isRagdollActive)
            return;

        isRagdollActive = true;

        // Add colliders and rigidbodies dynamically
        GenerateRagdollComponents();

        // Enable ragdoll physics
        SetRagdollState(true);

        // Apply force after physics update
        StartCoroutine(ApplyDeathForce(forceDirection, forceStrength));
    }

private void GenerateRagdollComponents()
{
    var rigidbodies = new System.Collections.Generic.List<Rigidbody>();
    var colliders = new System.Collections.Generic.List<Collider>();

    foreach (Transform bone in GetComponentsInChildren<Transform>())
    {
        if (bone == transform)
            continue;

        // Add Rigidbody if not present
        Rigidbody rb = bone.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = bone.gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true; // Initially kinematic
            rb.mass = 1f; // Adjust mass as needed
            rb.drag = 0.5f; // Add drag to stabilize
            rb.angularDrag = 0.5f;
        }
        rigidbodies.Add(rb);

        // Add Collider if not present
        Collider col = bone.GetComponent<Collider>();
        if (col == null)
        {
            if (bone.name.Contains("Arm") || bone.name.Contains("Leg"))
            {
                // Use CapsuleCollider for limbs
                CapsuleCollider capsule = bone.gameObject.AddComponent<CapsuleCollider>();
                capsule.direction = 2; // Z-axis direction
                capsule.height = 0.5f; // Adjust to match bone length
                capsule.radius = 0.1f; // Adjust to match bone thickness
                capsule.center = Vector3.zero; // Adjust to align with bone
                col = capsule;
            }
            else
            {
                // Use BoxCollider for torso, head, etc.
                BoxCollider box = bone.gameObject.AddComponent<BoxCollider>();
                box.size = new Vector3(0.2f, 0.2f, 0.2f); // Adjust size as needed
                box.center = Vector3.zero; // Adjust to align with bone
                col = box;
            }
        }

        col.enabled = false; // Disable until ragdoll activation
        colliders.Add(col);

        // Add Joint if not present
        if (bone.parent != null && bone.parent != transform)
        {
            Rigidbody parentRb = bone.parent.GetComponent<Rigidbody>();
            if (parentRb != null)
            {
                ConfigurableJoint joint = bone.GetComponent<ConfigurableJoint>();
                if (joint == null)
                {
                    joint = bone.gameObject.AddComponent<ConfigurableJoint>();
                    joint.connectedBody = parentRb;

                    // Lock linear motion
                    joint.xMotion = ConfigurableJointMotion.Locked;
                    joint.yMotion = ConfigurableJointMotion.Locked;
                    joint.zMotion = ConfigurableJointMotion.Locked;

                    // Configure angular motion
                    joint.angularXMotion = ConfigurableJointMotion.Limited;
                    joint.angularYMotion = ConfigurableJointMotion.Limited;
                    joint.angularZMotion = ConfigurableJointMotion.Limited;

                    // Set rotation limits
                    SoftJointLimit lowTwistLimit = new SoftJointLimit { limit = -20f };
                    SoftJointLimit highTwistLimit = new SoftJointLimit { limit = 20f };
                    SoftJointLimit swing1Limit = new SoftJointLimit { limit = 30f };
                    SoftJointLimit swing2Limit = new SoftJointLimit { limit = 30f };

                    joint.lowAngularXLimit = lowTwistLimit;
                    joint.highAngularXLimit = highTwistLimit;
                    joint.angularYLimit = swing1Limit;
                    joint.angularZLimit = swing2Limit;

                    joint.enablePreprocessing = false; // Helps prevent instability
                }
            }
        }
    }

    ragdollBodies = rigidbodies.ToArray();
    ragdollColliders = colliders.ToArray();
}

    private IEnumerator ApplyDeathForce(Vector3 forceDirection, float forceStrength)
    {
        yield return new WaitForSeconds(0.1f); // Wait for physics to stabilize

        if (ragdollBodies.Length > 0)
        {
            // Apply force to all rigidbodies for a more stable effect
            foreach (Rigidbody rb in ragdollBodies)
            {
                rb.AddForce(forceDirection.normalized * forceStrength, ForceMode.VelocityChange);
            }
        }
        else
        {
            Debug.LogError("Ragdoll activation failed: No rigidbodies detected.");
        }
    }
}
