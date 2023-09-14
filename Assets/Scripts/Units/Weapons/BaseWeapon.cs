using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseWeapon : BaseItem {
    public string weaponName; 
    public string weaponDescription;
    public int damage;
}

public enum WeaponType{
    Melee,
    Ranged,
    Magic,
}