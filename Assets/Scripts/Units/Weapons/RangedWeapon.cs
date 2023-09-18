using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ranged Weapon", menuName = "Weapon/Ranged", order = 0)]
public class RangedWeapon : BaseWeapon
{
    public int minRange;
    public int maxRange;
    [SerializeField] private WeaponRangedClasss rangedWeaponClass;

    public void Awake(){
        if (rangedWeaponClass == WeaponRangedClasss.Archer){
            base.weaponClass = WeaponClass.Archer;
        }else{
            base.weaponClass = WeaponClass.Magic;
        }
    }
}
