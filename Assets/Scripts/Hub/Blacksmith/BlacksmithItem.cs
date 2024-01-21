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
        private Item item;
        
        // References
        [SerializeField] private Button button;
        [SerializeField] private Image itemIcon;
        [SerializeField] private TextMeshProUGUI itemName;
        [SerializeField] private Image skillIconPrefab;
        [SerializeField] private Transform skillIconParent;

        public void Init(Item item, Action<Item> seeDetails)
        {
            this.item = item;
            itemName.text = item.itemName;
            itemIcon.sprite = item.itemIcon;
            foreach (Sprite sprite in item.skillIcons)
            {
                skillIconPrefab.sprite = sprite;
                Instantiate(skillIconPrefab,skillIconParent);
            }

            button.onClick.AddListener(delegate { seeDetails.Invoke(this.item); });
        }
    }
}