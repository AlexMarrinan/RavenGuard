using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Hub.Blacksmith
{
    public class BlacksmithSkill:MonoBehaviour
    {
        // Internal
        private BaseSkill skillData;
        
        // References
        [SerializeField] private Button button;
        [SerializeField] private Image skillIcon;
        [SerializeField] private TextMeshProUGUI skillName;
        [SerializeField] private TextMeshProUGUI skillCost;
        [SerializeField] private Image skillIconPrefab;
        [SerializeField] private Transform skillIconParent;
        [SerializeField] private GameObject skillCostParent;

        public static Action toggleCost;

        private void Awake()
        {
            toggleCost += ToggleCost;
        }

        /// <summary>
        /// Loads the skill's data into the references
        /// </summary>
        /// <param name="skill"></param>
        /// <param name="seeDetails">Opens the detail view if invoked</param>
        public void Init(BaseSkill skill, Action<BaseSkill> seeDetails)
        {
            skillData = skill;
            skillName.text = skill.skillName;
            skillIcon.sprite = skill.menuIcon;
            // TODO: Replace with actual cost when data is available
            skillCost.text = 5 + "G";
            skillCostParent.SetActive(true);
            
            foreach (Sprite sprite in skill.skillIcons)
            {
                skillIconPrefab.sprite = sprite;
                Instantiate(skillIconPrefab,skillIconParent);
            }
            button.onClick.AddListener(delegate { toggleCost.Invoke();});
        }

        private void ToggleCost()
        {
            skillCostParent.SetActive(!skillCostParent.activeSelf);
        }
    }
}