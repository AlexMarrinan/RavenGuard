using System;
using Game.Inventory;
using Hub.UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace Hub.Blacksmith
{
    /// <summary>
    /// It processes the game data and calculates how the values change at runtime.
    /// </summary>
    public class BlacksmithStoreController : Controller
    {
        [SerializeField] private BlacksmithStoreModel model;
        [SerializeField] private BlacksmithStoreView view;

        private Action<Item, int> upgradeItem;
        private Action<Item> seeDetails;

        private void Awake()
        {
            upgradeItem += UpgradeItem;
            seeDetails += OpenDetail;
            view.Init(model.playerBalance, model.upgradableItems, seeDetails);
        }

        /// <summary>
        /// Gets the upgradable version of the given item and opens the detail view.
        /// </summary>
        /// <param name="item">Item potentially being upgraded.</param>
        private void OpenDetail(Item item)
        {
            //Get upgrade info from model
            Item newItem = item;
            view.OpenDetailView(item,newItem,model.playerBalance,5,upgradeItem);
        }

        /// <summary>
        /// Add the given item to model and subtract the cost from playerBalance
        /// </summary>
        /// <param name="item">The item being added</param>
        /// <param name="cost">The cost of the item</param>
        private void UpgradeItem(Item item, int cost)
        {
            model.UpdatePlayerBalance(-1*cost);
            view.UpdatePlayerBalance(model.playerBalance);
        }
        
        public override void ToggleView()
        {
            view.ToggleUI();
        }
    }
}