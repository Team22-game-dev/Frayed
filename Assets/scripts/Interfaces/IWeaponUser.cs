using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeaponUser
{
    bool willPickupWeapon(); // Function where an entity decides if ot will pickup a weapon.

    Dictionary<string, Transform> GetWeaponBoneData { get; } // returns key value pairs of bones that weapons can be parented to.

    bool UnEquipWeapon();

    bool hasWeaponEquipped();

    GameObject GetEquippedWeapon();

}
