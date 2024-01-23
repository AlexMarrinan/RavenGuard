using System;
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

        public UpgradableSkill currentSkill;
        private Action<BaseSkill> seeDetails;

        private void Awake()
        {
            view.Init(model.playerBalance, model.GetUpgradableSkills(), this);
        }

        /// <summary>
        /// Confirm skill upgrade
        /// </summary>
        public void ConfirmSkillUpgrade()
        {
            print("ConfirmSkillUpgrade()");
            if (currentSkill.newSkill != null)
            {
                print("New skill");
                model.UpdateSkills(currentSkill);
                view.UpdatePlayerBalance(model.playerBalance);
            }
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