using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Active", menuName = "Skill/Active", order = 0)]
public class ActiveSkill : BaseSkill {
    public int cooldown = 0;    
    public delegate void UseDelegate();
    UseDelegate useDelegate;
    private void Awake(){
        var mng = SkillManager.instance;
        switch(base.skillName){
            case "EarthQuake":
                useDelegate = mng.EarthQuake;
                break;
        }
        Debug.Log(useDelegate);
    }
    public void OnUse(BaseUnit user){
        useDelegate();
    }
}
