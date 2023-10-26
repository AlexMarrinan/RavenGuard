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
    None,
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

public enum WeaponSubclass{
    Sword,
    Mace,
    Handaxe,
    Spear,
    Polaxe,
    Warpick,
    Bow,
    Crossbow,
}