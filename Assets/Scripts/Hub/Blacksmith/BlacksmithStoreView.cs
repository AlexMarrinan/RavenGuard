using System;
using System.Collections.Generic;
using Game.Inventory;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Hub.Blacksmith
{
    /// <summary>
    /// The View formats and renders a graphical presentation of your data onscreen.
    /// </summary>
    public class BlacksmithStoreView:MonoBehaviour
    {
        // Detail View
        [SerializeField] private GameObject detailViewParent;
        [SerializeField] private DetailView oldItem;
        [SerializeField] private DetailView newItem;
        [SerializeField] private Button confirmUpgrade;
        [SerializeField] private TextMeshProUGUI newItemCost;
        
        [SerializeField] private TextMeshProUGUI playerMoney;
        [SerializeField] private Transform itemParent;

        [Header("Prefabs")] 
        [SerializeField] private BlacksmithItem itemPrefab;

        private void Awake()
        {
            throw new NotImplementedException();
        }

        public void Init(int money, List<Item> items, Action<Item> seeDetails, UnityEvent<int> updatePlayerBalance)
        {
            updatePlayerBalance.AddListener(UpdatePlayerBalance);
            playerMoney.text = money.ToString();
            LoadItems(items, seeDetails);
        }

        private void UpdatePlayerBalance(int playerBalance)
        {
            playerMoney.text = playerBalance.ToString();
        }

        private void LoadItems(List<Item> items, Action<Item> seeDetails)
        {
            foreach (Item item in items)
            {
                BlacksmithItem blacksmithItem=Instantiate(itemPrefab, itemParent);
                blacksmithItem.Init(item,seeDetails);
            }
        }

        public void OpenDetailView(Item oldBlacksmithItem, Item newBlacksmithItem, int playerBalance,int upgradeCost, Action<Item,int> upgradeItem)
        {
            oldItem.SetItem(oldBlacksmithItem);
            newItem.SetItem(newBlacksmithItem);
            newItemCost.text = upgradeCost.ToString();
            detailViewParent.SetActive(true);

            confirmUpgrade.interactable=playerBalance>=upgradeCost;
            confirmUpgrade.onClick.AddListener(delegate { upgradeItem.Invoke(newBlacksmithItem,upgradeCost); });
        }
    }
}