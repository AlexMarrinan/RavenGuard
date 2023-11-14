using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

public class CrippledBuff : Buff{
    public int amount;
    public CrippledBuff(BaseUnit applier, BaseUnit appliedTo, int amount=2) : base(applier, appliedTo)
    {
        this.buffType = BuffType.OnApply;
        this.amount = amount;
        cooldown = 2;
    }

    public override void ApplyEffect(){
        appliedTo.ReduceNextMovemnt(amount);
    }

    public override void RemoveEffect(){
        appliedTo.ReduceNextMovemnt(0);
    }
}