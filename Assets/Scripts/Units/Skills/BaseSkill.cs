using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;

public class BaseSkill : BaseItem {
    public string skillName;
    public string description;
    public List<UnitClassType> validClasses = ((UnitClassType[])Enum.GetValues(typeof(UnitClassType))).ToList();
}


public enum SkillType {
    Active,
    Passive,
}