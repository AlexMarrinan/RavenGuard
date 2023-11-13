using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class BattlePrediction
{
    public BaseUnit attacker;
    public BaseUnit defender;
    public BaseUnit tempAttacker = null;
    public bool swappedAttackers = false;
    public int atkHealth;
    public int defHealth;
    public bool defenderCounterAttack = false;
    private bool defenderCAttackLocked = false;
    
    public bool defenderSecondAttack = false;
    private bool defenderSAttackLocked = false;

    public bool attackerSecondAttack = false;
    private bool attackerSAttackLocked = false;

    public BattlePrediction(BaseUnit start, BaseUnit def){
        this.attacker = start;
        this.defender = def;

        do {
            //attacker.UsePassiveSkills(PassiveSkillType.BeforeCombat);
            var atkBattleSkills = this.attacker.GetBattleSkills();
            foreach (CombatPassiveSkill battleSkill in atkBattleSkills){
                CheckBattleSkill(battleSkill, attacker);
            }

            //defender.UsePassiveSkills(PassiveSkillType.BeforeCombat);
            var defBattleSkills = this.defender.GetBattleSkills();
            foreach (CombatPassiveSkill battleSkill in defBattleSkills){
                CheckBattleSkill(battleSkill, defender);
            }

            Debug.Log(this.attacker.unitClass.ToString() + " " + this.defender.unitClass.ToString());
            if (tempAttacker == null || tempAttacker == attacker || swappedAttackers){
                break;
            }
            swappedAttackers = true;
            Debug.Log("Attacker/defender swapped!");

            //swap def and atk units
            (this.defender, this.attacker) = (this.attacker, this.defender);

            attackerSAttackLocked = false;
            defenderCAttackLocked = false;
            tempAttacker = null;
        }while (true);

        //var defBattleSkills = attacker.GetBattleSkills();

        attackerSecondAttack = !attackerSAttackLocked && (attacker.GetAgility().total >= defender.GetAgility().total + 5);

        defHealth = defender.health - attacker.GetDamage(defender);
        if (defHealth <= 0){
            return;
        }

        defenderCounterAttack = !defenderCAttackLocked && 
        (attacker is RangedUnit && defender is RangedUnit || attacker is MeleeUnit && defender is MeleeUnit);

        defenderSecondAttack = !defenderSAttackLocked && (defender.GetAgility().total >= attacker.GetAgility().total + 5) && defenderCounterAttack;

        if (defenderCounterAttack){
            atkHealth = attacker.health - defender.GetDamage(attacker);
            if (atkHealth <= 0){
                return;
            }
        }
        if (attackerSecondAttack){
            defHealth -= attacker.GetDamage(defender);
            if (defHealth <= 0){
                return;
            }
        }

        //TODO: ADD BACK ONCE AI IS FIGURED OUT BETTER OHNO !!!
        else if (defenderSecondAttack){
            atkHealth -= defender.GetDamage(attacker);
            if (atkHealth <= 0){
                return;
            }
        }
    }

    private void CheckBattleSkill(CombatPassiveSkill battleSkill, BaseUnit unit){
        foreach (var a in battleSkill.atributes){
            var cond = a.condition;
            var action = a.action;
            var verifyCond = CheckBattleCondition(cond, unit);
            // Debug.Log(cond.variable);
            // Debug.Log(verifyCond);
            if (verifyCond){
                RunCombatAction(action, unit);
            }
        }
    }
    private bool CheckBattleCondition(CombatPSCondition cond, BaseUnit unit)
    {
        int variableValue = 0;
        int compareVariableValue = 0;
        var otherUnit = attacker;
        if (unit == attacker){
            otherUnit = defender;
        }
        bool boolValue = cond.value == 1;
        switch(cond.variable){
            case CombatPSVariable.AlwaysTrue:
                return true;
            case CombatPSVariable.AlwaysFalse:
                return false;
            case CombatPSVariable.AttackFirst:
                return (unit == attacker) == boolValue;
            case CombatPSVariable.Health:
                variableValue = unit.health;
                compareVariableValue = unit.maxHealth;
                break;
            case CombatPSVariable.OppHealth:
                variableValue = otherUnit.health;
                compareVariableValue = otherUnit.maxHealth;
                break;

        }
        switch(cond.comparator){
            case Comparator.EqualTo:
                return variableValue == compareVariableValue*cond.value;
            case Comparator.LessThan:
                return variableValue < compareVariableValue*cond.value;
            case Comparator.LessThanEqualTo:
                Debug.Log("less than equal...");
                Debug.Log(variableValue + " " + compareVariableValue*cond.value);

                return variableValue <= compareVariableValue*cond.value;
            case Comparator.GreaterThan:
                return variableValue > compareVariableValue*cond.value;
            case Comparator.GreaterThanEqualTo:
                return variableValue >= compareVariableValue*cond.value;
        }
        return false;
    }

    private void RunCombatAction(CombatPSAction action, BaseUnit unit)
    {   
        var otherUnit = attacker;
        if (unit == attacker){
            otherUnit = defender;
        }
        bool boolValue = action.value == 1;
        switch(action.type){
            case CombatPSActionType.AttackFirst:
                if (tempAttacker == null){
                    tempAttacker = unit;
                }else{
                    tempAttacker = null;
                }
                break;
            case CombatPSActionType.OppAttackFirst:
                if (tempAttacker == null){
                    tempAttacker = otherUnit;
                }else{
                    tempAttacker = null;
                }
                break;
            case CombatPSActionType.CounterAttack:
                if (unit == defender){
                    defenderCAttackLocked = true;
                    defenderCounterAttack = boolValue;
                }
                break;
            case CombatPSActionType.OppCounterAttack:
                //Debug.Log("trying to stop opp cattack");
                if (otherUnit == defender){
                    //Debug.Log("stopped opp cattack");
                    defenderCAttackLocked = true;
                    defenderCounterAttack = boolValue;
                }
                break;
            case CombatPSActionType.FollowUpAttack:
                if (unit == attacker){
                    attackerSAttackLocked = true;
                    attackerSecondAttack = boolValue;
                }else{
                    defenderSAttackLocked = true;
                    defenderSecondAttack = boolValue;
                }
                break;
            case CombatPSActionType.OppFollowUpAttack:
                if (otherUnit == attacker){
                    attackerSAttackLocked = true;
                    attackerSecondAttack = boolValue;
                }else{
                    defenderSAttackLocked = true;
                    defenderSecondAttack = boolValue;
                }
                break;
            case CombatPSActionType.ReverseDebuffs:
                unit.ReverseBuffs();
                break;
            case CombatPSActionType.OppReverseDebuffs:
                otherUnit.ReverseBuffs();
                break;
        }
    }
}