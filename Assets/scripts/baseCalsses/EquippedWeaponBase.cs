using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EquippedWeaponBase : MonoBehaviour, IWeaponUser
{
    [SerializeField] 
    protected WeaponData weaponData; // local copy of weapon data
    [SerializeField] 
    protected GameObject equippedWeapon; // Add equippedWeapon GameObject
    [SerializeField]
    protected GameObject weaponSheath;
    // to be filled with tranforms of bones the weapon will target
    [SerializeField] protected Transform WeaponHandFowardR;
    [SerializeField] protected Transform WeaponHandReverseR;
    [SerializeField] protected Transform WeaponHandFowardL;
    [SerializeField] protected Transform WeaponHandReverseL;
    [SerializeField] protected Transform SheathBoneHip;
    [SerializeField] protected Transform SheathBoneBack;

    protected WeaponRotationData rotationComponent;  // Reference to the WeaponRotationComponent

    protected AnimationManager animationManager;
    public enum WeaponState
    {
        None,
        Sheathed,
        Drawn
    }

    public WeaponState currentWeaponState;


    public void Awake()
    {
        Debug.Log("EquipWeaponBase Awake function");
        // Get the AnimationManager component on the same GameObject
        animationManager = GetComponent<AnimationManager>();

        if (animationManager == null)
        {
            Debug.LogError("AnimationManager component not found on this GameObject.");
        }
    }

    protected void Start()
    {
        SetBoneData();

        if(hasWeaponEquipped())
            currentWeaponState = WeaponState.Sheathed;
        else
        {
            currentWeaponState = WeaponState.None;
            rotationComponent = GetComponent<WeaponRotationData>();
        }

    }

    

    private void SetBoneData()
    {
        Transform rootBone = GetComponentInChildren<Transform>();

        if(rootBone == null)
            Debug.LogError("root bone null");
        
        // Use GetComponentsInChildren to search through all descendants
        WeaponHandFowardR = FindDeepBoneByName(rootBone, "weapon-fwrd-ctrl.R");
        WeaponHandReverseR = FindDeepBoneByName(rootBone, "weapon-bkwrd-ctrl.R");
        WeaponHandFowardL = FindDeepBoneByName(rootBone, "weapon-fwrd-ctrl.L");
        WeaponHandReverseL = FindDeepBoneByName(rootBone, "weapon-bkwrd-ctrl.L");
        SheathBoneHip = FindDeepBoneByName(rootBone, "DEF-hip-Sheath");
        SheathBoneBack = FindDeepBoneByName(rootBone, "DEF-back-Sheath");

        Debug.Log("Bone: " + WeaponHandFowardR?.name + " " + WeaponHandReverseR?.name + " " + WeaponHandFowardL?.name + 
            " " + WeaponHandReverseL?.name + " " + SheathBoneHip?.name + " " + SheathBoneBack?.name);
    }

    private Transform FindDeepBoneByName(Transform root, string boneName)
    {
        // Iterate over all child transforms and their children recursively
        foreach (Transform child in root.GetComponentsInChildren<Transform>())
        {
            if (child.name == boneName)
            {
                return child;
            }
        }
        return null; // Return null if the bone is not found
    }


    public abstract bool WillPickupWeapon();

    // function to pass the bone data to the weapon
    public Dictionary<string, Transform> GetWeaponBoneData
    {
        get
        {
            Dictionary<string, Transform> boneData = new Dictionary<string, Transform>();
            boneData.Add("WeaponHandFowardR", WeaponHandFowardR);
            boneData.Add("WeaponHandReverseR", WeaponHandReverseR);
            boneData.Add("WeaponHandFowardL", WeaponHandFowardL);
            boneData.Add("WeaponHandReverseL", WeaponHandReverseL);
            boneData.Add("WeaponSheathHip", SheathBoneHip); // Example, add if needed
            boneData.Add("WeaponSheathBack", SheathBoneBack); // Example, add if needed
            return boneData;
        }
    }

    public IEnumerator EquipWeapon()
    {
        if (weaponData == null || weaponData.Weapon == null)
        {
            Debug.LogWarning("weaponData or weaponData.Weapon is null.");
            yield break;
        }

        // Ensure weaponData.Weapon is an instance in the scene
        if (!weaponData.Weapon.scene.IsValid())
        {
            Debug.LogError("weaponData.Weapon is not an instance in the scene. It might be a prefab asset.");
            yield break;
        }

        GameObject weaponToEquip = weaponData.Weapon;
        Debug.Log("Attempting to equip weapon: " + weaponToEquip.name);

        if (animationManager == null)
        {
            Debug.LogError("AnimationManager is null");
            yield break;
        }

        // Wait for the animation to finish
        string animationName = animationManager.GetCurrentAnimationName(); ;
        float startTime = Time.time;
        float weaponSwitchTimeLimit = 1.0f;

        while (animationName == "transition" || animationName.StartsWith("attack_"))
        {
            if (Time.time - startTime > weaponSwitchTimeLimit)
            {
                Debug.LogWarning($"Weapon equip timed out while waiting for animation to finish. Last animation: {animationName}");
                yield break;
            }
            yield return new WaitForSeconds(0.033f);
            animationName = animationManager.GetCurrentAnimationName();
        }
        // do
        // {
        //     animationName = animationManager.GetCurrentAnimationName();
        //     if (animationName == "transition")
        //     {
        //         Debug.Log("Animation in transition!");
        //         yield return new WaitForSeconds(0.033f);
        //     }

        //     if (Time.time - startTime > weaponSwitchTimeLimit)
        //     {
        //         Debug.LogWarning($"Weapon equip timed out while waiting for animation to finish. Last animation: {animationName}");
        //         yield break;
        //     }
        // } while (animationName == "transition");

        if (!(animationName == "transition" || animationName.StartsWith("attack_")))
        {
            Debug.Log("All good to equip weapon");

            equippedWeapon = weaponToEquip;

            if (weaponData != null)
            {
                weaponData.SetUserData(this);

                // Properly set the parent
                if (weaponData.SheathedBone != null)
                {
                    // needs to instantiate a new instance of the sheath model.
                    weaponSheath = Instantiate(weaponData.Sheath);

                    

                    weaponSheath.transform.SetParent(weaponData.SheathedBone);
                    weaponSheath.transform.localPosition = Vector3.zero;
                    weaponSheath.transform.localRotation = Quaternion.identity;

                    //ApplySheathRotation();

                    equippedWeapon.transform.SetParent(weaponData.SheathedBone);
                    equippedWeapon.transform.localPosition = Vector3.zero;
                    equippedWeapon.transform.localRotation = Quaternion.identity;
                    currentWeaponState = WeaponState.Sheathed;
                }
                else
                {
                    Debug.LogWarning("weaponData.SheathedBone is null!");
                    // some enemies dont have sheathes try draw weapon
                    DrawWeapon(false);
                }
            }
            else
            {
                Debug.LogError("WeaponData component not found on the equipped weapon.");
            }
        }
        else
        {
            Debug.LogError("Was NOT all good to equip weapon. Oops!");
        }
    }

    // local function to start the equip weapon coroutine
    public IEnumerator StartEquipWeaponCoroutine(GameObject pickedUpWeapon)
    {
        Debug.Log("StartEquipWeaponCoroutine with: " + pickedUpWeapon.name + " by " + gameObject.name);
        if (pickedUpWeapon == null)
        {
            Debug.LogWarning("pickedUpWeapon is null");
            yield break;
        }

        // Get the WeaponData component from the picked-up weapon
        WeaponData newWeaponData = pickedUpWeapon.GetComponent<WeaponData>();

        if (newWeaponData == null)
        {
            Debug.LogError("newWeaponData is null");
            yield break;
        }

        newWeaponData.Weapon = pickedUpWeapon;

        Debug.Log("newWeaponData.Weapon: " + newWeaponData.Weapon);

        // Assign the new weapon data
        weaponData = newWeaponData;

        

        // Destroy the old equipped weapon if any
        if (equippedWeapon != null)
        {
            Destroy(equippedWeapon);
        }

        Debug.Log("Starting equip weapon coroutine...");
        yield return StartCoroutine(EquipWeapon());
    }



    public GameObject GetEquippedWeapon()
    {
        return equippedWeapon; // Return equipped weapon
    }

    public WeaponData GetWeaponData()
    {
        return weaponData;
    }

    public bool UnEquipWeapon()
    {
        // removes the Weapon datas references to the users bones changes weapon into "rag-doll state"
        if(hasWeaponEquipped())
        {
            // remove this users transform data from weapon
            equippedWeapon.transform.SetParent(null);
            weaponData.ActionBoneL = null;
            weaponData.ActionBoneR = null;
            weaponData.SheathedBone = null;

            // remove weapon and its data from this user
            weaponData = null;
            equippedWeapon = null;
            currentWeaponState = WeaponState.None;
            return false;
        }
        else
        {
            Debug.Log("Tried do unequip null weapon");
            return true;
        }


    }


    public void SheathAndDrawWeapon()
    {
        Debug.Log("SheathAndDraw method state: " + currentWeaponState);
        bool isDrawn = animationManager.GetBool("WeaponDrawn");
        switch (currentWeaponState)
        {
            case WeaponState.None:
            case WeaponState.Sheathed:
                if(!weaponData.twoHanded) 
                {
                    
                    Debug.Log("Parent weapon to hand bone");
                    equippedWeapon.transform.SetParent(weaponData.ActionBoneR);
                    equippedWeapon.transform.localPosition = Vector3.zero;
                    
                }
                else
                {
                    // Two Handed Equip Logic (stretch goal)
                }
                currentWeaponState = WeaponState.Drawn;
                Debug.Log("Weapon Drawn");
                
                if (isDrawn != true)
                {
                    animationManager.SetBool("WeaponDrawn", true);
                }

                ApplyWeaponRotation();
                break;
            case WeaponState.Drawn:
                Debug.Log("Weapon sheathed");
                currentWeaponState = WeaponState.Sheathed;
                equippedWeapon.transform.SetParent(weaponData.SheathedBone);
                equippedWeapon.transform.localPosition = Vector3.zero;
                //ApplySheathRotation();
                if (isDrawn == true)
                {
                    animationManager.SetBool("WeaponDrawn", false);
                }
                break;
        }
        

    }

    public void DrawWeapon(bool playAnimation)
    {
        string currentAnimation = animationManager.GetCurrentAnimationName();

        if(playAnimation && currentAnimation == "Idle_rest")
        {
            // start animation and wait for animation event to parent to hand on OnDrawAnimation
     
   
            if (weaponData != null && currentWeaponState == WeaponState.Sheathed)
            {
                Debug.Log("Playing draw animation");
                animationManager.SetTrigger(weaponData.DrawAnimation);
            }
            else
            {
                Debug.Log("Weapon was null or in wrong state");
            }
        } 
        else 
        {
            // invoke the method to reparent now
            Debug.Log("invoke directly");
            animationManager.SetTrigger("DrawDagger");
            SheathAndDrawWeapon();
        }
    }

    protected void ApplyWeaponRotation()
    {
        // Get the rotation for the weapon from the user's WeaponRotationComponent
        Quaternion weaponRotation;

        switch(currentWeaponState)
        {
            case WeaponState.Drawn:
                weaponRotation = rotationComponent.GetDrawnWeaponRotation(weaponData.WeaponName);
                break;
            case WeaponState.Sheathed:
                weaponRotation = rotationComponent.GetSheathedWeaponRotation(weaponData.WeaponName);
                break;
            default:
                Debug.LogError("weapon in wrong state to get rotation");
                return;

        }

        // Apply the rotation to the equipped weapon
        equippedWeapon.transform.localRotation = weaponRotation;
    }
    public void ApplySheathRotation()
    {
        // Get the sheath rotation first
        Quaternion sheathRotation = rotationComponent.GetSheathedWeaponRotation(weaponData.WeaponName);

        // Then log it
        Debug.Log($"Applying sheath rotation: {sheathRotation}");

        // Finally, apply the rotation
        weaponSheath.transform.localRotation = sheathRotation;
    }


    public void DropWeapon()
    {
        if(hasWeaponEquipped())
        {
            // make refrence to rb before losing refrence to weapon 
            //Rigidbody rb = equippedWeapon.GetComponent<Rigidbody>();
            WeaponPickup weaponPickup = equippedWeapon.GetComponent<WeaponPickup>();
            if(weaponPickup != null)
            {
                if(!UnEquipWeapon())
                {
                    weaponPickup.DropWeapon();
                    //rb.isKinematic = false;
                    //rb.useGravity = true;
                }
                else
                {
                    Debug.Log("UnEquip method failed! Can not drop weapon");
                    return;
                }
            }
            else
            {
                Debug.Log("Equipped Weapon" + weaponData.WeaponName + " Rigid Body is null");
            }

        }
    }
    public bool hasWeaponEquipped()
    {
        //Debug.Log("Checking if Weapon is equipped!" + (equippedWeapon != null));
        return equippedWeapon != null;
    }
    public bool isDrawn()
    {
        return currentWeaponState == WeaponState.Drawn;
    }


}
