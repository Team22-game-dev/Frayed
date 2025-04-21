using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeaponUser
{
    bool WillPickupWeapon(); // Function where an entity decides if ot will pickup a weapon.

    Dictionary<string, Transform> GetWeaponBoneData { get; } // returns key value pairs of bones that weapons can be parented to.

    bool UnEquipWeapon(Transform parent);

    bool hasWeaponEquipped();

    GameObject GetEquippedWeapon();

}
