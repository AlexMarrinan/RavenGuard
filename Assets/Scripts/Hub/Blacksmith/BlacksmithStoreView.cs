using System;
using System.Collections.Generic;
using Game.Inventory;
using Hub.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Hub.Blacksmith
{
    /// <summary>
    /// The View formats and renders a graphical presentation of your data onscreen.
    /// </summary>
    public class BlacksmithStoreView:View
    {
        [SerializeField] private TextMeshProUGUI playerMoney;
        [SerializeField] private Transform itemParent;
        
        [Header("Detail View")] 
        [SerializeField] private GameObject detailViewParent;
        [SerializeField] private DetailView oldItem;
        [SerializeField] private DetailView newItem;
        [SerializeField] private Button confirmUpgrade;
        [SerializeField] private TextMeshProUGUI newItemCost;

        [Header("Prefabs")] 
        [SerializeField] private BlacksmithItem itemPrefab;

        public void Init(int money, List<Item> items, Action<Item> seeDetails)
        {
            playerMoney.text = money.ToString();
            LoadItems(items, seeDetails);
        }

        /// <summary>
        /// Updates the playerMoney text to match the playerBalance.
        /// </summary>
        /// <param name="playerBalance">How much money the player has.</param>
        public void UpdatePlayerBalance(int playerBalance)
        {
            playerMoney.text = playerBalance+"G";
        }

        /// <summary>
        /// Instantiates the given item
        /// </summary>
        /// <param name="items">The item</param>
        /// <param name="seeDetails">If the item is clicked on, open the detail view</param>
        private void LoadItems(List<Item> items, Action<Item> seeDetails)
        {
            foreach (Item item in items)
            {
                BlacksmithItem blacksmithItem=Instantiate(itemPrefab, itemParent);
                blacksmithItem.Init(item,seeDetails);
            }
        }

        /// <summary>
        /// Open the detail view and set its info
        /// </summary>
        /// <param name="oldBlacksmithItem">The item potentially being upgraded.</param>
        /// <param name="newBlacksmithItem">The next version of the old item.</param>
        /// <param name="playerBalance">How much money the player has.</param>
        /// <param name="upgradeCost">How much the upgrade will cost.</param>
        /// <param name="upgradeItem">Triggers upgrade logic if invoked.</param>
        public void OpenDetailView(Item oldBlacksmithItem, Item newBlacksmithItem, int playerBalance,int upgradeCost, Action<Item,int> upgradeItem)
        {
            oldItem.SetItem(oldBlacksmithItem);
            newItem.SetItem(newBlacksmithItem);
            newItemCost.text = upgradeCost.ToString();
            detailViewParent.SetActive(true);

            confirmUpgrade.interactable=playerBalance>=upgradeCost;
            confirmUpgrade.onClick.AddListener(delegate { upgradeItem.Invoke(newBlacksmithItem,upgradeCost); });
        }

        /// <summary>
        /// Hides the detail view
        /// </summary>
        public void HideDetailView()
        {
            detailViewParent.SetActive(false);
        }
    }
}