using System;
using System.Collections.Generic;
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
        [FormerlySerializedAs("upgradableSkill")] public List<SkillProgressionGroup> upgradableSkillGroups;

        public List<UpgradableSkill> GetUpgradableSkills()
        {
            List<UpgradableSkill> skills = new List<UpgradableSkill>();
            foreach (SkillProgressionGroup skillGroup in upgradableSkillGroups)
            {
                //TODO: Integrate with save system to get the current skill index
                int currentSkillIndex = 0;
                
                // Avoids putting skills in when the skill is maxed out
                if (currentSkillIndex < skillGroup.skillProgression.Count)
                {
                    UpgradableSkill upgradableSkill = new UpgradableSkill();
                    upgradableSkill.oldSkill = skillGroup.skillProgression[currentSkillIndex].skill;
                    upgradableSkill.newSkill = skillGroup.skillProgression[currentSkillIndex+1].skill;
                    upgradableSkill.cost = skillGroup.skillProgression[currentSkillIndex + 1].cost;
                    skills.Add(upgradableSkill);
                }
            }

            return skills;
        }

        public List<UpgradableSkill> UpdateSkills(UpgradableSkill skill)
        {
            // TODO: Update skill index in save system
            playerBalance -= skill.cost;
            return GetUpgradableSkills();
        }
    }

    public struct UpgradableSkill
    {
        public int cost;
        public BaseSkill oldSkill;
        public BaseSkill newSkill;
    }
}