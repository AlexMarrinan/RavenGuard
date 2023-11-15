using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class ConfusionBuff : Buff{
    public ConfusionBuff(BaseUnit applier, BaseUnit appliedTo) : base(applier, appliedTo)
    {
        this.positive = false;
        this.buffType = BuffType.OnApply;
    }

    public override void ApplyEffect(){
        appliedTo.attackEffect = AttackEffect.Confusion;
    }

    public override void RemoveEffect(){
        appliedTo.attackEffect = AttackEffect.None;
    }
}