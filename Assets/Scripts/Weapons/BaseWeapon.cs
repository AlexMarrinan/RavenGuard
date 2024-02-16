using System;
using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.Serialization;
using Weapons;

public class BaseWeapon : BaseItem {
    public string weaponName; 
    public string weaponDescription;
    public int damage;
    public float cost;
    public WeaponUpgradeGroup weaponUpgradeGroup;
    public List<Sprite> statIcons;
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