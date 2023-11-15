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
        methodInfo = type.GetMethod(base.skillName + "AS");
    }
     public override void OnSelect(BaseUnit user){
        MenuManager.instance.ToggleUnitActionMenu();
        SkillManager.instance.currentSkill = this;
        SkillManager.instance.user = user;
        SkillManager.instance.ShowSkillPreview();
    }
    public override void OnUse(BaseUnit user){
        var mng = SkillManager.instance;
        var param = new object[1];
        param[0] = user;
        methodInfo.Invoke(mng, param);
        user.FinishTurn();
        SkillManager.instance.OnSkilEnd();
    }
}

public enum ActiveSkillType {
    OnSelf,
    OnUnit,
    OnHero,
    OnEnemy,
    OnTile,
}
