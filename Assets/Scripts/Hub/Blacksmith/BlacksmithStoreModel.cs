using System;
using System.Collections.Generic;
using Game.Inventory;
using UnityEngine;
using UnityEngine.Events;

namespace Hub.Blacksmith
{
    /// <summary>
    /// The Model is strictly a data container that holds values. It does not perform gameplay logic or run calculations.
    /// </summary>
    public class BlacksmithStoreModel:MonoBehaviour
    {
        public int playerBalance;
        public List<Item> upgradableItems;

        public void AddItem(Item item)
        {
            upgradableItems.Add(item);
        }

        public void UpdatePlayerBalance(int deposit)
        {
            playerBalance += deposit;
        }
    }
}