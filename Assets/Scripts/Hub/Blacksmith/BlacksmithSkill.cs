using System.Collections.Generic;
using Skills;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Hub.Blacksmith
{
    public class BlacksmithSkill:MonoBehaviour
    {
        // Internal
        public BaseSkill skillData;
        public int cost;
        public int rank;
        private UpgradableSkill upgradableSkill;
        private BlacksmithStoreView view;
        
        // References
        [SerializeField] private Button button;
        [SerializeField] private Image skillIcon;
        [SerializeField] private TextMeshProUGUI skillName;
        [SerializeField] private TextMeshProUGUI skillCost;
        [SerializeField] private GameObject skillCostParent;
        [SerializeField] private List<Image> skillMiniIcon;

        private void Awake()
        {
            view = FindObjectOfType<BlacksmithStoreView>();
            button.onClick.AddListener(
                delegate { view.ToggleDetailView(upgradableSkill); }
            );
        }

        public void Init(UpgradableSkill skill, BlacksmithStoreView view)
        {
            this.view = view;
            Init(skill);
        }

        /// <summary>
        /// Loads the skill's data into the references
        /// </summary>
        /// <param name="skill"></param>
        /// <param name="view">The menu ui</param>
        public void Init(UpgradableSkill skill)
        {
            //Remembers what it is able to upgrade into and what it currently is
            upgradableSkill = skill;
            cost = skill.cost;
            rank = skill.rank;
            skillCost.text = skill.cost + "G";
            
            //The skill it currently is
            skillData = skill.next;
            skillName.text = skillData.skillName + " " + SkillLevelString(skillData.skillLevel);
            skillIcon.sprite = skillData.menuIcon;
            skillCostParent.SetActive(true);
            
            SetIcons();
        }
        
        private string SkillLevelString(int level){
            string str = "[";
            for (int i = 0; i < level; i++){
                str += "I";
            }        
            return str + "]";
        }
        /// <summary>
        /// Set the prefab's icon images
        /// Instantiation is too expensive
        /// </summary>
        void SetIcons()
        {
            for (int i = 0; i < skillMiniIcon.Count; i++)
            {
                if (skillData.skillIcons.Count > i)
                {
                    skillMiniIcon[i].sprite = skillData.skillIcons[i];
                    skillMiniIcon[i].enabled = true;
                }
                else
                {
                    skillMiniIcon[i].enabled = false;
                }
            }
        }
    }
}