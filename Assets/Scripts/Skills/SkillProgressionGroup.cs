using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// SkillProgressionGroup is a way to keep track of what skills are upgraded into.
/// </summary>
[CreateAssetMenu(fileName = "Skill Progression", menuName = "Skill/Progression Group", order = 0)]
public class SkillProgressionGroup : ScriptableObject
{
    public List<SkillCost> skillProgression;

    public void SetReferences()
    {
        foreach (SkillCost skillCost in skillProgression)
        {
            skillCost.skill.progressionGroup = this;
        }
    }
}

[Serializable]
public struct SkillCost
{
    public BaseSkill skill;
    public int cost;
}
