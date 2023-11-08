using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BattlePrediction
{
    public BaseUnit attacker;
    public BaseUnit defender;
    public int atkHealth;
    public int defHealth;
    public bool defenderCounterAttack = false;
    public bool attackerSecondAttack = false;
    public BattlePrediction(BaseUnit attacker, BaseUnit defender){
        this.attacker = attacker;
        this.defender = defender;
        attackerSecondAttack = attacker.GetAgility().total >= defender.GetAgility().total + 5;
        defHealth = defender.health - attacker.GetDamage(defender);
        if (defHealth <= 0){
            return;
        }
        defenderCounterAttack = (attacker is RangedUnit && defender is RangedUnit) || 
            (attacker is MeleeUnit && defender is MeleeUnit);
        if (defenderCounterAttack){
            atkHealth = attacker.health - defender.GetDamage(attacker);
            if (atkHealth <= 0){
                return;
            }
        }
        if (defenderCounterAttack){
            defHealth = defHealth - attacker.GetDamage(defender);
            if (defHealth <= 0){
                return;
            }
        }
    }
}