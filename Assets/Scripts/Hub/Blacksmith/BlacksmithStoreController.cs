using System;
using Game.Inventory;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace Hub.Blacksmith
{
    /// <summary>
    /// It processes the game data and calculates how the values change at runtime.
    /// </summary>
    public class BlacksmithStoreController:MonoBehaviour
    {
        [SerializeField] private BlacksmithStoreModel model;
        [SerializeField] private BlacksmithStoreView view;
        [SerializeField] private DetailView oldItem;
        
        public Action<Item,int> upgradeItem;
        public Action<Item> seeDetails;

        private void Awake()
        {
            upgradeItem += UpgradeItem;
            view.Init(0,model.upgradableItems,seeDetails,model.updatePlayerBalance);
        }

        public void OpenDetail(BlacksmithItem item)
        {
            //Get upgrade info from model
            
        }

        public void UpgradeItem(Item item, int cost)
        {
            model.UpdatePlayerBalance(-1*cost);
        }
    }
}