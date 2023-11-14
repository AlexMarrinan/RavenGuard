using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


[CreateAssetMenu(fileName = "[Name]PS", menuName = "Skill/Passive", order = 0)]
public class PassiveSkill : BaseSkill {
    public PassiveSkillType passiveSkillType;
    public override void SetMethod(){
        var mng = SkillManager.instance;
        methodInfo = mng.GetType().GetMethod(base.skillName + "PS");
    }
    public override void OnUse(BaseUnit user){
        var mng = SkillManager.instance;
        var param = new object[1];
        param[0] = user;
        methodInfo.Invoke(mng, param);
    }
}

public enum PassiveSkillType {
    Buff,
    BeforeCombat,
    DuringCombat,
    AfterCombat,
    OnMovement,
    OnTurnEnd,
}