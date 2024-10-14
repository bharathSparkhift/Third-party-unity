using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    WeaponManager rifle;
    Rifle ak47;


    private void Start()
    {
        rifle = new Rifle();
        ak47 = new AK47();
    }

    [ContextMenu("Spawn Weapon")]
    public void SpawnWeapon()
    {
        if(rifle == null) {
            rifle = new Rifle();
        }
        rifle.Spawn_Weapon();
    }

    [ContextMenu("Equip Weapon")]
    public void EquipWeapon()
    {
        if(ak47 == null)
        {
            ak47 = new AK47();
        }
        ak47.Equip();
    }
    
    [ContextMenu("Drop Weapon")]
    public void DropWeapon()
    {
        if(ak47 == null)
        {
            ak47 = new AK47();
        }
        ak47.Drop();
    }
}
