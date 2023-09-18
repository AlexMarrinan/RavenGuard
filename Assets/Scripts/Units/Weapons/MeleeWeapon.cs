using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Melee Weapon", menuName = "Weapon/Melee", order = 0)]
public class MeleeWeapon : BaseWeapon
{
    [SerializeField] private WeaponMeleeClass meleeWeaponClass;

    public void Awake(){
        if (meleeWeaponClass == WeaponMeleeClass.SideArms){
            base.weaponClass = WeaponClass.SideArms;
        }else{
            base.weaponClass = WeaponClass.LongArms;
        }
    }
}
