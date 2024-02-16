using System;
using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

public class BaseWeapon : BaseItem {
    public string weaponName; 
    public string weaponDescription;
    public int damage;
    public List<WeaponType> weaponUpgrades;
    public List<Sprite> statIcons;
    [HideInInspector] public WeaponClass weaponClass;
}

[Serializable]
public struct WeaponType
{
    public BaseWeapon weapon;
    public float cost;
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