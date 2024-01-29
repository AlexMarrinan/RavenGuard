using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// SkillProgressionGroup is a way to keep track of what skills are upgraded into.
/// </summary>
[CreateAssetMenu(fileName = "Skill Progression", menuName = "Skill/Progression Group", order = 0)]
public class SkillProgressionGroup : ScriptableObject
{
    public List<SkillCost> skillProgression;
    public int index { get; private set; }

    public void SetReferences()
    {
        foreach (SkillCost skillCost in skillProgression)
        {
            skillCost.skill.progressionGroup = this;
        }
    }

    public bool ContainsSkill(BaseSkill skill)
    {
        foreach (SkillCost skillCost in skillProgression)
        {
            if (skill == skillCost.skill)
            {
                return true;
            }
        }

        return false;
    }
}

[Serializable]
public class SkillCost
{
    public BaseSkill skill;
    public int cost;
}
