using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


public class UnitStat {
    
    public UnitStatType statType;
    private readonly int baseAmount;
    private readonly int change;
    public int total;
    private StatIncreaseType statIncreaseType;
    public UnitStat (UnitStatType type, int baseAmount, int change){
        this.statType = type;
        this.change = change;
        this.baseAmount = baseAmount;
        if (change == 0){
            statIncreaseType = StatIncreaseType.NEUTRAL;
        }else if (change < 0){
            statIncreaseType = StatIncreaseType.DOWN;
        }else{
            statIncreaseType = StatIncreaseType.UP;
        }
        total = baseAmount + change;
    }

    public StatIncreaseType GetStatIncreaseType(){
        return statIncreaseType;
    }
}

public enum StatIncreaseType{
    UP,
    DOWN,
    NEUTRAL
}