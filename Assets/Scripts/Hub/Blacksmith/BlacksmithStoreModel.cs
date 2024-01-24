using System.Collections.Generic;
using Skills;
using UnityEngine;

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
        
        /// <summary>
        /// Get the UpgradableSkill from the skillProgressionGroup at the given skillIndex
        /// </summary>
        /// <param name="skillGroup">The skill group with the needed skill</param>
        /// <param name="currentSkillIndex">The index of the current skill</param>
        /// <returns>The next skill in the progression, or null if one does not exist</returns>
        private UpgradableSkill GetUpgradableSkill(SkillProgressionGroup skillGroup, int currentSkillIndex)
        {
            List<SkillCost> skillCosts = skillGroup.skillProgression;
                
            // Avoids putting skills in when the skill is maxed out
            if (currentSkillIndex < skillGroup.skillProgression.Count)
            {
                BaseSkill currentSkill= skillCosts[currentSkillIndex].skill;
                SkillCost newSkill = skillGroup.skillProgression[currentSkillIndex + 1];
                return new UpgradableSkill(newSkill.cost,currentSkill,newSkill.skill);
            }

            return null;
        }

        #endregion

        /// <summary>
        /// Gets the skillProgression element that holds the given skill
        /// </summary>
        /// <param name="skill"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Updates the skills
        /// </summary>
        /// <param name="skill">The skill being upgraded</param>
        /// <returns>Return the upgraded skill</returns>
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
        
        /// <summary>
        /// Removes the given skillProgression from the list
        /// </summary>
        /// <param name="progression">The skillProgression being removed</param>

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
            skillProgression.index++;
            if (skillProgression.CanProgress())
            {
                BaseSkill currentSkill = skillProgression.GetCurrentSkillCost().skill;
                SkillCost skillCost = skillProgression.GetNextSkillCost();
                return new UpgradableSkill(skillCost.cost, currentSkill, skillCost.skill);
            }
            RemoveSkill(skillProgression);
            return null;
        }
        
        
    }
}