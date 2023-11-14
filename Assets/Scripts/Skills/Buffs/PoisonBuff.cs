using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

public class PoisonBuff : Buff{

    public PoisonBuff(BaseUnit applier, BaseUnit appliedTo) : base(applier, appliedTo) {
        this.cooldown = 2;
        this.buffType = BuffType.OnTurn;
    }
    public override void ApplyEffect(){
        appliedTo.ReceiveDamage(-1);
    }
}