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
        public List<SkillProgression> skillProgressionList;
        public List<UpgradableSkill> upgradableSkills;

        #region Loading

        /// <summary>
        /// Loads the upgradable skills from the skillProgressionList
        /// </summary>
        /// <returns></returns>
        public List<UpgradableSkill> LoadUpgradableSkills()
        {
            List<UpgradableSkill> skills = new List<UpgradableSkill>();
            foreach (SkillProgression skillGroup in skillProgressionList)
            {
                UpgradableSkill skill = GetUpgradableSkill(skillGroup.progressionGroup,skillGroup.index);
                if(skill!=null) skills.Add(skill);
            }

            upgradableSkills = skills;
            return skills;
        }
        
        private UpgradableSkill GetUpgradableSkill(SkillProgressionGroup skillGroup, int currentSkillIndex)
        {
            List<SkillCost> skillCosts = skillGroup.skillProgression;
                
            // Avoids putting skills in when the skill is maxed out
            if (currentSkillIndex < skillGroup.skillProgression.Count)
            {
                BaseSkill currentSkill= skillCosts[currentSkillIndex].skill;
                SkillCost newSkill = skillGroup.skillProgression[currentSkillIndex + 1];
                print(newSkill.skill.skillName);
                return new UpgradableSkill(newSkill.cost,currentSkill,newSkill.skill);
            }

            return null;
        }

        #endregion

        private SkillProgression GetSkillProgression(UpgradableSkill skill)
        {
            foreach (SkillProgression skillProgression in skillProgressionList)
            {
                if (skillProgression.progressionGroup.ContainsSkill(skill.current))
                {
                    return skillProgression;
                }
            }

            return null;
        }

        public UpgradableSkill UpdateSkills(UpgradableSkill skill)
        {
            SkillProgression progression = GetSkillProgression(skill);
            if (progression != null)
            {
                playerBalance -= skill.cost;
            }

            UpgradableSkill nextSkill=GetNextUpgradableSkill(progression);
            return nextSkill;
        }

        private void RemoveSkill(SkillProgression progression)
        {
            skillProgressionList.Remove(progression);
            
        }

        /// <summary>
        /// Gets the next upgradableSkillSet from the given skillProgression
        /// </summary>
        /// <param name="skillProgression"></param>
        /// <returns></returns>
        private UpgradableSkill GetNextUpgradableSkill(SkillProgression skillProgression)
        {
            BaseSkill currentSkill = skillProgression.progressionGroup.skillProgression[skillProgression.index].skill;
            SkillCost skillCost = GetNextSkillCost(skillProgression);
            if (skillCost != null) return new UpgradableSkill(skillCost.cost, currentSkill, skillCost.skill);
            RemoveSkill(skillProgression);
            return null;
        }

        /// <summary>
        /// Gets the next skill and its cost in the given progression
        /// </summary>
        /// <param name="progression"></param>
        /// <returns></returns>
        private SkillCost GetNextSkillCost(SkillProgression progression)
        {
            int nextSkillIndex = progression.index+2;
            List<SkillCost> skillCosts = progression.progressionGroup.skillProgression;
            if (skillCosts.Count != nextSkillIndex)
            {
                // TODO: Update skill index in save system
                progression.index++;
                return skillCosts[progression.index+1];
            }
            return null;
        }
        /*

        /// <summary>
        /// 
        /// </summary>
        /// <param name="progression"></param>
        /// <param name="skill"></param>
        /// <returns></returns>
        private UpgradableSkill AddNextSkill(SkillProgression progression, UpgradableSkill skill)
        {
            BaseSkill oldSkill = skill.next;
            upgradableSkills.Remove(skill);
            SkillProgressionGroup progressionGroup = progression.progressionGroup;
            progression.index++;
            if (progression.index == progressionGroup.skillProgression.Count) return null;

            SkillCost newSkill = progressionGroup.skillProgression[progression.index];
            UpgradableSkill upgradableSkill = new UpgradableSkill(newSkill.cost,oldSkill,newSkill.skill);
            upgradableSkills.Add(upgradableSkill);
            return upgradableSkill;
        }*/
    }

    [Serializable]
    public class SkillProgression
    {
        public SkillProgressionGroup progressionGroup;
        public int index;
    }

    [Serializable]
    public class UpgradableSkill
    {
        public int cost;
        public BaseSkill current;
        public BaseSkill next;

        public UpgradableSkill(int cost, BaseSkill current, BaseSkill next)
        {
            this.cost = cost;
            this.current = current;
            this.next = next;
        }
    }
}