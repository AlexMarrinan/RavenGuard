using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class PoisonBuff : Buff{
    public int amount;
    public PoisonBuff(BaseUnit applier, BaseUnit appliedTo, int amount) : base(applier, appliedTo) {
        this.cooldown = 2;
        this.positive = false;
        this.buffType = BuffType.OnTurn;
        this.amount = amount;
    }
    public override void ApplyEffect(){
        //Debug.Log(appliedTo.occupiedTile.coordiantes);
        appliedTo.ReceiveDamage(amount);
    }
}