using System;
using System.Collections.Generic;
using Hub.UI;
using UnityEngine;

namespace Hub.Blacksmith
{
    /// <summary>
    /// It processes the game data and calculates how the values change at runtime.
    /// </summary>
    public class BlacksmithStoreController : Controller
    {
        [SerializeField] private BlacksmithStoreModel model;
        [SerializeField] private BlacksmithStoreView view;

        private void Awake()
        {
            view.Init(model.playerBalance, model.GetUpgradableSkills(), this);
        }

        /// <summary>
        /// Confirm skill upgrade
        /// </summary>
        /// <param name="upgradableSkill"></param>
        public UpgradableSkill ConfirmSkillUpgrade(UpgradableSkill upgradableSkill)
        {
            if (upgradableSkill.newSkill != null)
            {
                return model.UpdateSkills(upgradableSkill);
            }

            return null;
        }

        public int GetPlayerBalance()
        {
            return model.playerBalance;
        }
        
        public override void ToggleView()
        {
            view.ToggleUI();
        }
    }
}