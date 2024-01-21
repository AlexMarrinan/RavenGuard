using System;
using Game.Inventory;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Hub.Blacksmith
{
    public class BlacksmithItem:MonoBehaviour
    {
        // Internal
        private Item itemData;
        
        // References
        [SerializeField] private Button button;
        [SerializeField] private Image itemIcon;
        [SerializeField] private TextMeshProUGUI itemName;
        [SerializeField] private Image skillIconPrefab;
        [SerializeField] private Transform skillIconParent;

        /// <summary>
        /// Loads the item's data into the references
        /// </summary>
        /// <param name="item"></param>
        /// <param name="seeDetails">Opens the detail view if invoked</param>
        public void Init(Item item, Action<Item> seeDetails)
        {
            itemData = item;
            itemName.text = item.itemName;
            itemIcon.sprite = item.itemIcon;
            
            foreach (Sprite sprite in item.skillIcons)
            {
                skillIconPrefab.sprite = sprite;
                Instantiate(skillIconPrefab,skillIconParent);
            }
            
            button.onClick.AddListener(delegate { seeDetails.Invoke(itemData); });
        }
    }
}