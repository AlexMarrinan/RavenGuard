using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Hub.Blacksmith
{
    /// <summary>
    /// The Model is strictly a data container that holds values. It does not perform gameplay logic or run calculations.
    /// </summary>
    public class BlacksmithStoreModel:MonoBehaviour
    {
        public int playerBalance;
        public List<SkillProgressionGroup> skillProgressionGroups;
        public List<UpgradableSkill> upgradableSkills;
        
        public List<UpgradableSkill> GetUpgradableSkills()
        {
            List<UpgradableSkill> skills = new List<UpgradableSkill>();
            foreach (SkillProgressionGroup skillGroup in skillProgressionGroups)
            {
                UpgradableSkill skill = GetUpgradableSkill(skillGroup);
                if(skill!=null) skills.Add(skill);
            }

            upgradableSkills = skills;
            return skills;
        }

        private UpgradableSkill GetUpgradableSkill(SkillProgressionGroup skillGroup)
        {
            List<SkillCost> skillCosts = skillGroup.skillProgression;
            //TODO: Integrate with save system to get the current skill index
            int currentSkillIndex = 0;
                
            // Avoids putting skills in when the skill is maxed out
            if (currentSkillIndex < skillGroup.skillProgression.Count)
            {
                BaseSkill currentSkill= skillCosts[currentSkillIndex].skill;
                SkillCost newSkill = skillGroup.skillProgression[currentSkillIndex + 1];
                return new UpgradableSkill(newSkill.cost,currentSkill,newSkill.skill);
            }

            return null;
        }

        private SkillProgressionGroup GetSkillProgressionGroup(UpgradableSkill skill)
        {
            foreach (SkillProgressionGroup group in skillProgressionGroups)
            {
                if (group.ContainsSkill(skill.oldSkill))
                {
                    return group;
                }
            }

            return null;
        }

        public UpgradableSkill UpdateSkills(UpgradableSkill skill)
        {
            // TODO: Update skill index in save system
            SkillProgressionGroup group = GetSkillProgressionGroup(skill);
            if (group != null)
            {
                playerBalance -= skill.cost;
            }

            UpgradableSkill nextSkill=AddNextSkill(group, skill);
            return nextSkill;
        }

        private UpgradableSkill AddNextSkill(SkillProgressionGroup progressionGroup, UpgradableSkill skill)
        {
            int index=0;
            BaseSkill oldSkill = skill.newSkill;
            for (int i=0;i< progressionGroup.skillProgression.Count;i++)
            {
                if (progressionGroup.skillProgression[i].skill == oldSkill)
                {
                    index = i + 1;
                    break;
                }
            }
            if (index == progressionGroup.skillProgression.Count) return null;

            SkillCost newSkill = progressionGroup.skillProgression[index];
            UpgradableSkill upgradableSkill = new UpgradableSkill(newSkill.cost,oldSkill,newSkill.skill);
            upgradableSkills.Add(upgradableSkill);
            return upgradableSkill;
        }
    }

    [Serializable]
    public class UpgradableSkill
    {
        public int cost;
        public BaseSkill oldSkill;
        public BaseSkill newSkill;

        public UpgradableSkill(int cost, BaseSkill oldSkill, BaseSkill newSkill)
        {
            this.cost = cost;
            this.oldSkill = oldSkill;
            this.newSkill = newSkill;
        }
    }
}