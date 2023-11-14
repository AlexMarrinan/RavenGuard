using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


public class UnitStatMultiplier {
    
    public UnitStatType statType;
    public readonly float multiplier;
    private StatIncreaseType statIncreaseType;
    public UnitStatMultiplier (UnitStatType type, float multiplier){
        this.statType = type;
        this.multiplier = multiplier;
        if (multiplier > 1){
            statIncreaseType = StatIncreaseType.NEUTRAL;
        }else if (multiplier < 1){
            statIncreaseType = StatIncreaseType.DOWN;
        }else{
            statIncreaseType = StatIncreaseType.UP;
        }
    }

    public StatIncreaseType GetStatIncreaseType(){
        return statIncreaseType;
    }
}