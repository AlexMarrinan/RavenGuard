using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseWeapon : BaseItem {
    public string weaponName; 
    public string weaponDescription;
    public int damage;
    [HideInInspector] public WeaponClass weaponClass;
}
public enum WeaponAttackMethod {
    Melee,
    Ranged,
}
public enum WeaponClass {
    Any,
    LongArms,
    SideArms,
    Archer,
    Magic,
}

public enum WeaponMeleeClass {
    LongArms,
    SideArms
}
public enum WeaponRangedClasss {
    Archer,
    Magic,
}