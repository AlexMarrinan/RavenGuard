using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Hub.Blacksmith
{
    /// <summary>
    /// A detailed view of an item
    /// </summary>
    public class DetailViewWindow:MonoBehaviour
    {
        private BaseSkill skillData;
        
        // References
        [SerializeField] private Image skillIcon;
        [SerializeField] private TextMeshProUGUI skillName;
        [SerializeField] private TextMeshProUGUI skillDesc;
        [SerializeField] private List<Image> skillMiniIcon;

        /// <summary>
        /// Loads the item's information into references
        /// </summary>
        /// <param name="skill"></param>
        public void SetItem(BaseSkill skill)
        {
            skillData = skill;
            
            skillName.text = skillData.skillName;
            skillDesc.text = skillData.description;
            skillIcon.sprite = skillData.menuIcon;
            SetIcons();
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