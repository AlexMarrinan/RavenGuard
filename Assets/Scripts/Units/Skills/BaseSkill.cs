using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;

public class BaseSkill : BaseItem {
    public string skillName;
    public string description;
    public UnitClass validClass;
    // public List<WeaponAttackMethod> validClasses = ((WeaponAttackMethod[])Enum.GetValues(typeof(WeaponAttackMethod))).ToList();
}


public enum SkillType {
    Active,
    UniversalPassive,
    ClassPassive,
    WeaponPassive,
}
