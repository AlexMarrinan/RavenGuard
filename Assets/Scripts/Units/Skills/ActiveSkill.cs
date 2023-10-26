using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


[CreateAssetMenu(fileName = "[Name]AS", menuName = "Skill/Active", order = 0)]
public class ActiveSkill : BaseSkill {
    public int cooldown = 0;
    public ActiveSkillType activeSkillType;
    public override void SetMethod(){
        var mng = SkillManager.instance;
        var type = mng.GetType();
        Debug.Log(type);
        methodInfo = type.GetMethod(base.skillName + "AS");
        Debug.Log(methodInfo);
    }
}

public enum ActiveSkillType {
    OnSelf
}
