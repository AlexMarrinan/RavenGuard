using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class JelousBuff : Buff{
    public JelousBuff(BaseUnit applier, BaseUnit appliedTo) : base(applier, appliedTo)
    {
        this.positive = false;
        this.buffType = BuffType.OnApply;
    }

    public override void ApplyEffect(){
        if (applier.GetAttack().total < appliedTo.GetAttack().total){
            appliedTo.AddStatsChange("JealousAtk", UnitStatType.Attack, -4, -4, 0);
        }
        if (applier.GetDefense().total < appliedTo.GetDefense().total){
            appliedTo.AddStatsChange("JealousDef", UnitStatType.Defense, -4, -4, 0);
        }
        if (applier.GetAgility().total < appliedTo.GetAgility().total){
            appliedTo.AddStatsChange("JealousAgl", UnitStatType.Agility, -4, -4, 0);
        }
        if (applier.GetAttuenment().total < appliedTo.GetAttuenment().total){
            appliedTo.AddStatsChange("JealousAtu", UnitStatType.Attunment, -4, -4, 0);
        }
        if (applier.GetForesight().total < appliedTo.GetForesight().total){
            appliedTo.AddStatsChange("JealousFor", UnitStatType.Foresight, -4, -4, 0);
        }
        if (applier.GetLuck().total < appliedTo.GetLuck().total){
            appliedTo.AddStatsChange("JealousLck", UnitStatType.Luck, -4, -4, 0);
        }
    }

    public override void RemoveEffect(){
        Debug.Log("removing effect...");        
        appliedTo.SetStatChange("JealousAtk", 0);
        appliedTo.SetStatChange("JealousDef", 0);
        appliedTo.SetStatChange("JealousAgl", 0);
        appliedTo.SetStatChange("JealousAtu", 0);
        appliedTo.SetStatChange("JealousFor", 0);
        appliedTo.SetStatChange("JealousLck", 0);

        appliedTo.RemoveStatChange("JealousAtk");
        appliedTo.RemoveStatChange("JealousDef");
        appliedTo.RemoveStatChange("JealousAgl");
        appliedTo.RemoveStatChange("JealousAtu");
        appliedTo.RemoveStatChange("JealousFor");
        appliedTo.RemoveStatChange("JealousLck");
    }
}