using System;

namespace Skills
{
    [Serializable]
    public class UpgradableSkill
    {
        public int cost;
        public int rank;
        public BaseSkill current;
        public BaseSkill next;
        public SkillProgressionGroup group;

        public UpgradableSkill(int cost, int rank,BaseSkill current, BaseSkill next, SkillProgressionGroup group)
        {
            this.cost = cost;
            this.rank = rank;
            this.current = current;
            this.next = next;
            this.group = group;
        }
    }
}