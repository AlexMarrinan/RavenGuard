using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Hub.Blacksmith
{
    /// <summary>
    /// A detailed view of an item
    /// </summary>
    public class DetailView:MonoBehaviour
    {
        private BaseSkill skillData;
        
        // References
        [SerializeField] private Image skillIcon;
        [SerializeField] private TextMeshProUGUI skillName;
        [SerializeField] private TextMeshProUGUI skillDesc;
        [SerializeField] private Image skillIconPrefab;
        [SerializeField] private Transform skillIconParent;
        
        /// <summary>
        /// Loads the item's information into references
        /// </summary>
        /// <param name="skill"></param>
        public void SetItem(BaseSkill skill)
        {
            ClearOldInfo();
            skillData = skill;
            skillName.text = skillData.skillName;
            skillDesc.text = skillData.description;
            skillIcon.sprite = skillData.menuIcon;
            foreach (Sprite sprite in skillData.skillIcons)
            {
                skillIconPrefab.sprite = sprite;
                Instantiate(skillIconPrefab,skillIconParent);
            }
        }

        private void ClearOldInfo()
        {
            while( skillIconParent.transform.childCount != 0)
            {
                Destroy(skillIconParent.transform.GetChild(0).transform);
            }
        }
    }
}