using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Debug = System.Diagnostics.Debug;

namespace Hub.Blacksmith
{
    public class BlacksmithSkill:MonoBehaviour
    {
        // Internal
        public BaseSkill skillData;
        private UpgradableSkill upgradableSkill;
        private BlacksmithStoreView view;
        
        // References
        [SerializeField] private Button button;
        [SerializeField] private Image skillIcon;
        [SerializeField] private TextMeshProUGUI skillName;
        [SerializeField] private TextMeshProUGUI skillCost;
        [SerializeField] private Transform skillIconParent;
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
            skillCost.text = skill.cost + "G";
            
            //The skill it currently is
            skillData = skill.newSkill;
            skillName.text = skillData.skillName;
            skillIcon.sprite = skillData.menuIcon;
            skillCostParent.SetActive(true);
            
            SetIcons();
        }

        public void ShowCost(bool showCost)
        {
            skillCostParent.SetActive(showCost);
        }
        
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