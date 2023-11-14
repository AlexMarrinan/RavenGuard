using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

public class DualityBuff : Buff{
    public DualityBuff(BaseUnit applier, BaseUnit appliedTo) : base(applier, appliedTo)
    {
        this.positive = true;
        this.buffType = BuffType.OnApply;
    }

    public override void ApplyEffect(){
        appliedTo.attackEffect = AttackEffect.Duality;
    }

    public override void RemoveEffect(){
        appliedTo.attackEffect = AttackEffect.None;
    }
}

public enum AttackEffect {
    None,
    Duality,
    Confusion
}