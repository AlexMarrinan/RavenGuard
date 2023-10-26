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
        Debug.Log(methodInfo);
    }
}

public enum PassiveSkillType {
    StatChange
}