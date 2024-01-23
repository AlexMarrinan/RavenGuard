using System;
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
        private BaseSkill skillData;
        private UpgradableSkill upgradableSkill;
        
        // References
        [SerializeField] private Button button;
        [SerializeField] private Image skillIcon;
        [SerializeField] private TextMeshProUGUI skillName;
        [SerializeField] private TextMeshProUGUI skillCost;
        [SerializeField] private Image skillIconPrefab;
        [SerializeField] private Transform skillIconParent;
        [SerializeField] private GameObject skillCostParent;

        /// <summary>
        /// Loads the skill's data into the references
        /// </summary>
        /// <param name="skill"></param>
        /// <param name="view">The menu ui</param>
        public void Init(UpgradableSkill skill, BlacksmithStoreView view)
        {
            //Remembers what it is able to upgrade into and what it currently is
            upgradableSkill = skill;
            skillCost.text = skill.cost + "G";
            
            //The skill it currently is
            skillData = skill.newSkill;
            skillName.text = skillData.skillName;
            skillIcon.sprite = skillData.menuIcon;
            skillCostParent.SetActive(true);
            
            foreach (Sprite sprite in skillData.skillIcons)
            {
                skillIconPrefab.sprite = sprite;
                Instantiate(skillIconPrefab,skillIconParent);
            }
            button.onClick.AddListener(delegate
            {
                view.ToggleDetailView(upgradableSkill);
            });
        }

        public void ShowCost(bool showCost)
        {
            skillCostParent.SetActive(showCost);
        }
    }
}