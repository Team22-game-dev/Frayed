using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EquippedWeaponBase : MonoBehaviour, IWeaponUser
{
    [SerializeField] private WeaponData weaponData; // local copy of weapon data
    [SerializeField] private GameObject equippedWeapon; // Add equippedWeapon GameObject

    // to be filled with tranforms of bones the weapon will target
    [SerializeField] private Transform WeaponHandFowardR;
    [SerializeField] private Transform WeaponHandReverseR;
    [SerializeField] private Transform WeaponHandFowardL;
    [SerializeField] private Transform WeaponHandReverseL;
    [SerializeField] private Transform SheathBoneHip;
    [SerializeField] private Transform SheathBoneBack;

    private AnimationManager animationManager;

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

    void Start()
    {
        SetBoneData();
    }

    

    private void SetBoneData()
    {
        Transform rootBone = GetComponentInChildren<Transform>(); 
        
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


    public abstract bool willPickupWeapon();

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

    // private void ParentToUser()
    // {
    //     equippedWeapon.transform.SetParent()
    // }

    // use enumerator for yield if animation in transisiton period
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
    string animationName;
    float startTime = Time.time;
    float weaponSwitchTimeLimit = 0.5f;

    do
    {
        animationName = animationManager.GetCurrentAnimationName();
        if (animationName == "transition")
        {
            Debug.Log("Animation in transition!");
            yield return new WaitForSeconds(0.033f);
        }

        if (Time.time - startTime > weaponSwitchTimeLimit)
        {
            Debug.LogWarning($"Weapon equip timed out while waiting for animation to finish. Last animation: {animationName}");
            yield break;
        }
    } while (animationName == "transition");

    if (!animationName.StartsWith("attack_"))
    {
        Debug.Log("All good to equip weapon");

        equippedWeapon = weaponToEquip;

        if (weaponData != null)
        {
            weaponData.SetUserData(this);

            // Properly set the parent
            if (weaponData.SheathedBone != null)
            {
                equippedWeapon.transform.SetParent(weaponData.SheathedBone);
                equippedWeapon.transform.localPosition = Vector3.zero;
                equippedWeapon.transform.localRotation = Quaternion.identity;
            }
            else
            {
                Debug.LogWarning("weaponData.SheathedBone is null!");
            }
        }
        else
        {
            Debug.LogError("WeaponData component not found on the equipped weapon.");
        }
    }
}




    // local function to start the equip weapon coroutine
public void StartEquipWeaponCoroutine(GameObject pickedUpWeapon)
{
    if (pickedUpWeapon == null)
    {
        Debug.LogWarning("pickedUpWeapon is null");
        return;
    }

    // Get the WeaponData component from the picked-up weapon
    WeaponData newWeaponData = pickedUpWeapon.GetComponent<WeaponData>();

    if (newWeaponData == null)
    {
        Debug.LogError("newWeaponData is null");
        return;
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
    StartCoroutine(EquipWeapon());
}



    public GameObject GetEquippedWeapon()
    {
        return equippedWeapon; // Return equipped weapon
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
            return false;
        }
        else
        {
            Debug.Log("Tried do unequip null weapon");
            return true;
        }


    }

    public void DrawWeapon(bool playAnimation)
    {
        if(playAnimation)
        {
            // start animation and wait for animation event to parent to hand on OnDrawAnimation
            if (weaponData != null && weaponData.GetWeaponState() == WeaponData.State.Equipped)
            {
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
            OnDrawWeaponEvent();
        }
    }

    public void OnDrawWeaponEvent()
    {
        if(!weaponData.twoHanded) 
        {
            Debug.Log("Parent weapon to hand bone");
            equippedWeapon.transform.SetParent(weaponData.ActionBoneR);
            equippedWeapon.transform.localPosition = Vector3.zero;
            equippedWeapon.transform.localRotation = Quaternion.identity;
        }
        else
        {
            // Two Handed Equip Logic (stretch goal)
        }
    }

    public void DropWeapon()
    {
        if(hasWeaponEquipped())
        {
            // make refrence to rb before losing refrence to weapon 
            Rigidbody rb = equippedWeapon.GetComponent<Rigidbody>();
            if(rb != null)
            {
                if(!UnEquipWeapon())
                {
                    rb.isKinematic = false;
                    rb.useGravity = true;
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
        Debug.Log("Checking if Weapon is equipped!");
        return equippedWeapon != null;
    }


}
