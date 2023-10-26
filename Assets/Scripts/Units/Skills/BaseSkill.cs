using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;
using System.Reflection;

public class BaseSkill : BaseItem {
    public string skillName;
    public string description;
    public UnitClass unitClass;
    public WeaponClass weaponClass;
    protected MethodInfo methodInfo;
    public void OnUse(BaseUnit user){
        var mng = SkillManager.instance;
        var param = new object[1];
        param[0] = user;
        methodInfo.Invoke(mng, param);
    }
    public virtual void SetMethod(){

    }
}