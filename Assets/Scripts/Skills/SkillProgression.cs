using System;
using System.Collections.Generic;

namespace Skills
{
    [Serializable]
    public class SkillProgression
    {
        public SkillProgressionGroup progressionGroup;
        public int index;

        public SkillCost GetCurrentSkillCost()
        {
            List<SkillCost> skillCosts = progressionGroup.skillProgression;
            return skillCosts[index];
        }
        
        public SkillCost GetNextSkillCost()
        {
            if (CanProgress())
            {
                List<SkillCost> skillCosts = progressionGroup.skillProgression;
                return skillCosts[index + 1];
            }

            return null;
        }
        
        public bool CanProgress()
        {
            List<SkillCost> skillCosts = progressionGroup.skillProgression;
            return (index + 1) != skillCosts.Count;
        }
    }
}