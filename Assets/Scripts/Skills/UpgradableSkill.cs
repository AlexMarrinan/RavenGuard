using System;

namespace Skills
{
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