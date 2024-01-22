using System;
using System.Collections.Generic;
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
        public List<BaseSkill> upgradableSkill;

        public void AddItem(BaseSkill skill)
        {
            upgradableSkill.Add(skill);
        }

        public void UpdatePlayerBalance(int deposit)
        {
            playerBalance += deposit;
        }
    }
}