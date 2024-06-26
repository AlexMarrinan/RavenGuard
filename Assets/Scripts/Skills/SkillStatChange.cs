using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


public class SkillStatChange {
    
    public UnitStatType statType;
    public int currentAmount;
    public int minAmount;
    public int maxAmount;
    public int cooldown;
    public SkillStatChange (UnitStatType type, int currentAmount, int minAmount, int maxAmount, int cooldown)
    {
        this.statType = type;
        this.currentAmount = currentAmount;
        this.minAmount = minAmount;
        this.maxAmount = maxAmount;
        this.cooldown = cooldown;
    }
}