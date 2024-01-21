using Game.Inventory;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Hub.Blacksmith
{
    public class DetailView:MonoBehaviour
    {
        private Item itemData;
        
        // References
        [SerializeField] private Image itemIcon;
        [SerializeField] private TextMeshProUGUI itemName;
        [SerializeField] private TextMeshProUGUI itemDesc;
        [SerializeField] private Image skillIconPrefab;
        [SerializeField] private Transform skillIconParent;
        public void SetItem(Item oldBlacksmithItem)
        {
            itemData = oldBlacksmithItem;
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