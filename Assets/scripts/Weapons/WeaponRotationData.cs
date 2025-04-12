using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class WeaponRotationData : MonoBehaviour
{
    [Header("Weapon Rotation Data")]
    [SerializeField] private Quaternion drawnDaggerRotation = Quaternion.identity;
    [SerializeField] private Quaternion sheathedDaggerRotation = Quaternion.identity;
    [SerializeField] private Quaternion amalgamSwordRotation = Quaternion.identity;

    [SerializeField] private Quaternion axeRotation = Quaternion.identity;
    [SerializeField] private Quaternion bowRotation = Quaternion.identity;

    // Method to get the rotation for a specific weapon
    public Quaternion GetDrawnWeaponRotation(string weaponName)
    {
        switch (weaponName.ToLower())
        {
            case "dagger":
                return drawnDaggerRotation;
            case "amalgamsword":
                return axeRotation;
            case "DNE2":
                return bowRotation;
            default:
                Debug.LogWarning($"Rotation data for weapon '{weaponName}' not found.");
                return Quaternion.identity;  // Default if not found
        }
    }

    public Quaternion GetSheathedWeaponRotation(string weaponName)
    {
        switch (weaponName.ToLower())
        {
            case "dagger":
                return sheathedDaggerRotation;
            case "DNE1":
                return axeRotation;
            case "DNE2":
                return bowRotation;
            default:
                Debug.LogWarning($"Rotation data for weapon '{weaponName}' not found.");
                return Quaternion.identity;  // Default if not found
        }
    }
}
