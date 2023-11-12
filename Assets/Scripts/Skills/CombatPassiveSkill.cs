using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


[CreateAssetMenu(fileName = "[Name]PS", menuName = "Skill/CombatPassive", order = 0)]
public class CombatPassiveSkill : PassiveSkill {

    public List<CombatPSAtribute> atributes;
    // [HideInInspector];
    // public PassiveSkillType passiveSkillType = PassiveSkillType.BeforeCombat;
}
[Serializable]
public class CombatPSAtribute
{
    public CombatPSCondition condition;
    public CombatPSAction action;
}

[Serializable]
public class CombatPSCondition
{
    public CombatPSVariable conditions;
    public Comparator comparator;
    public float value;
}

[Serializable]
public class CombatPSAction
{
    public CombatPSActionType type;
    public float value;
}
public enum Comparator {
    EqualTo,
    LessThan,
    LessThanEqualTo,
    GreaterThan,
    GreaterThanEqualTo,
    AlwaysTrue,
    AlwaysFalse
}

//TODO: ADD UNIT OF TYPE IN RANGE SOMEHOW IDK??? !!!
public enum CombatPSVariable {
    Health,
    Attack,
    Defense,
    Agility,
    Attunment,
    Foresight,
    Luck,

    OppHealth,
    OppAttack,
    OppDefense,
    OppAgility,
    OppAttunment,
    OppForesight,
    OppLuck,

    AlwaysTrue,
    AlwaysFalse
}

public enum CombatPSActionType {
    AttackFirst,
    OppAttackFirst,
    
    CounterAttack,
    OppCounterAttack,

    FollowUpAttack,
    OppFollowUpAttack,

    // DamagePerDebuff,
    // OppDamagerPerDebuff

    ReverseDebuffs,
    OppReverseDebuffs,

    DamageMultiplier,
    OppDamageMultiplier,
}