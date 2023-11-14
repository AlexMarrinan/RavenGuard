using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

public class Buff {
    public int cooldown = 1;
    public BuffType buffType;
    public BaseUnit applier;
    public BaseUnit appliedTo;
    public Buff(BaseUnit applier, BaseUnit appliedTo){
        this.applier = applier;
        this.appliedTo = appliedTo;
    }
    public virtual void ApplyEffect(){
    }
    public virtual void RemoveEffect(){
    }
    public void ReduceCooldown(){
        cooldown -= 1;
    }
    public bool CooldownOver(){
        if (cooldown <= 0){
            Debug.Log("cooldown <= 0");
            this.RemoveEffect();
            return true;
        }
        return false;
    }
}

public enum BuffType
{
    OnApply,
    OnTurn,
}