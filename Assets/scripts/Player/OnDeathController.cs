using System.Collections;
using UnityEngine;

public class OnDeathController : MonoBehaviour
{
    public AnimationManager animationManager;
    public CharacterController characterController;
    private Rigidbody[] ragdollBodies;
    private Collider[] ragdollColliders;
    private bool isRagdollActive = false;

    private void Start()
    {
        animationManager = GetComponent<AnimationManager>();
        characterController = GetComponent<CharacterController>();
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

    public IEnumerator ActivateRagdoll(Vector3 forceDirection, float forceStrength)
    {
        if (isRagdollActive)
            yield return null;

        isRagdollActive = true;
        GenerateRagdollComponents();
        SetRagdollState(true);
        yield return StartCoroutine(ApplyDeathForce(forceDirection, forceStrength));
        SetRagdollState(false);
        isRagdollActive = false;
    }

    private void GenerateRagdollComponents()
    {
        var rigidbodies = new System.Collections.Generic.List<Rigidbody>();
        var colliders = new System.Collections.Generic.List<Collider>();

        foreach (Transform bone in GetComponentsInChildren<Transform>())
        {
            if (bone == transform)
                continue;

            string boneName = bone.name.ToLower();

            // Ensure bone name contains "DEF" and matches the specified keywords
            if (boneName.Contains("DEF") &&
                (boneName.Contains("spine") || boneName.Contains("arm") ||
                boneName.Contains("thigh") || boneName.Contains("shin") || boneName.Contains("head")))
            {
                // Add Rigidbody if not present
                Rigidbody rb = bone.GetComponent<Rigidbody>();
                if (rb == null)
                {
                    rb = bone.gameObject.AddComponent<Rigidbody>();
                    rb.isKinematic = true; // Initially kinematic
                    rb.mass = 1f;
                    rb.drag = 0.5f;
                    rb.angularDrag = 0.5f;
                }
                rigidbodies.Add(rb);

                // Add Collider if not present
                Collider col = bone.GetComponent<Collider>();
                if (col == null)
                {
                    if (boneName.Contains("arm") || boneName.Contains("thigh") || boneName.Contains("shin") || boneName.Contains("head"))
                    {
                        // Use CapsuleCollider for limbs
                        CapsuleCollider capsule = bone.gameObject.AddComponent<CapsuleCollider>();
                        capsule.direction = 2; // Z-axis
                        capsule.height = 0.5f;
                        capsule.radius = 0.1f;
                        capsule.center = Vector3.zero;
                        col = capsule;
                    }
                    else
                    {
                        // Use BoxCollider for the spine
                        BoxCollider box = bone.gameObject.AddComponent<BoxCollider>();
                        box.size = new Vector3(0.2f, 0.2f, 0.2f);
                        box.center = Vector3.zero;
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
        }

        ragdollBodies = rigidbodies.ToArray();
        ragdollColliders = colliders.ToArray();
    }


    private IEnumerator ApplyDeathForce(Vector3 forceDirection, float forceStrength)
    {
        yield return new WaitForSeconds(0.1f);

        if (ragdollBodies.Length > 0)
        {
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
