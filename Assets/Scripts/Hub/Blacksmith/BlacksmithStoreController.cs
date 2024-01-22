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

        private Action<BaseSkill, int> upgradeItem;
        private Action<BaseSkill> seeDetails;

        private void Awake()
        {
            upgradeItem += UpgradeSkill;
            seeDetails += OpenDetail;
            view.Init(model.playerBalance, model.upgradableSkill, seeDetails);
        }

        /// <summary>
        /// Gets the upgradable version of the given skill and opens the detail view.
        /// </summary>
        /// <param name="skill">Item potentially being upgraded.</param>
        private void OpenDetail(BaseSkill skill)
        {
            //Get upgrade info from model
            BaseSkill newSkill = skill;
            view.OpenDetailView(skill,newSkill,model.playerBalance,5,upgradeItem);
        }

        /// <summary>
        /// Add the given skill to model and subtract the cost from playerBalance
        /// </summary>
        /// <param name="skill">The new skill</param>
        /// <param name="cost">The cost of the skill</param>
        private void UpgradeSkill(BaseSkill skill, int cost)
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