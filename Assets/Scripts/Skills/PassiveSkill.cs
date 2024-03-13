using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


[CreateAssetMenu(fileName = "[Name]PS", menuName = "Skill/Passive", order = 0)]
public class PassiveSkill : BaseSkill {
    public PassiveSkillType passiveSkillType;
    /// <summary>
    /// Sets the backend method for the given skill based on its name
    /// </summary>
    public override void SetMethod(){
        var mng = SkillManager.instance;
        methodInfo = mng.GetType().GetMethod(base.skillName + "PS");
    }
    /// <summary>
    /// If the passive conditions are met, calls the active skills backend method to perform the skill
    /// </summary>
    /// <param name="user">unit that is using the skill</param>
    public override void OnUse(BaseUnit user){
        var mng = SkillManager.instance;
        var param = new object[1];
        param[0] = user;
//        Debug.Log(methodInfo);
        methodInfo.Invoke(mng, param);
    }
}

public enum PassiveSkillType {
    OnTurnStart,
    BeforeCombat,
    DuringCombat,
    AfterCombat,
    OnMovement,
    OnTurnEnd,
}