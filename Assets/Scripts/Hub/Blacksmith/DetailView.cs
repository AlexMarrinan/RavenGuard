using Game.Inventory;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Hub.Blacksmith
{
    /// <summary>
    /// A detailed view of an item
    /// </summary>
    public class DetailView:MonoBehaviour
    {
        private Item itemData;
        
        // References
        [SerializeField] private Image itemIcon;
        [SerializeField] private TextMeshProUGUI itemName;
        [SerializeField] private TextMeshProUGUI itemDesc;
        [SerializeField] private Image skillIconPrefab;
        [SerializeField] private Transform skillIconParent;
        
        /// <summary>
        /// Loads the item's information into references
        /// </summary>
        /// <param name="item"></param>
        public void SetItem(Item item)
        {
            itemData = item;
            itemName.text = itemData.itemName;
            itemDesc.text = itemData.desc;
            itemIcon.sprite = itemData.itemIcon;
            foreach (Sprite sprite in itemData.skillIcons)
            {
                skillIconPrefab.sprite = sprite;
                Instantiate(skillIconPrefab,skillIconParent);
            }
        }
    }
}