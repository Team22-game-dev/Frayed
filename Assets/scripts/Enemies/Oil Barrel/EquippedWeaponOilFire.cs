using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquippedWeaponOilFire : EquippedWeaponBase
{
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {

    }

    override public bool WillPickupWeapon()
    {
        return false;
    }
}
