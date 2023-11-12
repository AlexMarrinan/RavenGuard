using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class CombatOrder {
    public bool canFollowUp;
    public bool canCounterAttack;

    public CombatOrder(bool canFollowUp, bool canCounterAttack){
        this.canFollowUp = canFollowUp;
        this.canCounterAttack = canCounterAttack;
    }
}