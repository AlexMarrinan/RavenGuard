using System;
using System.Collections.Generic;
using Hub.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
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
        [SerializeField] private GameObject sortParent;
        
        [Header("Detail View")] 
        [SerializeField] private GameObject detailViewParent;
        [SerializeField] private DetailView oldItem;
        [SerializeField] private DetailView newItem;
        [SerializeField] private Button confirmUpgrade;
        [SerializeField] private TextMeshProUGUI newItemCost;

        [FormerlySerializedAs("itemPrefab")]
        [Header("Prefabs")] 
        [SerializeField] private BlacksmithSkill skillPrefab;

        public void Init(int money, List<BaseSkill> skills, Action<BaseSkill> seeDetails)
        {
            playerMoney.text = money.ToString();
            LoadItems(skills, seeDetails);
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
        /// Instantiates the given skill
        /// </summary>
        /// <param name="skills">The skill</param>
        /// <param name="seeDetails">If the item is clicked on, open the detail view</param>
        private void LoadItems(List<BaseSkill> skills, Action<BaseSkill> seeDetails)
        {
            foreach (BaseSkill skill in skills)
            {
                BlacksmithSkill blacksmithSkill=Instantiate(skillPrefab, itemParent);
                blacksmithSkill.Init(skill,seeDetails);
            }
        }

        /// <summary>
        /// Open the detail view and set its info
        /// </summary>
        /// <param name="oldBlacksmithSkill">The item potentially being upgraded.</param>
        /// <param name="newBlacksmithSkill">The next version of the old item.</param>
        /// <param name="playerBalance">How much money the player has.</param>
        /// <param name="upgradeCost">How much the upgrade will cost.</param>
        /// <param name="upgradeItem">Triggers upgrade logic if invoked.</param>
        public void OpenDetailView(BaseSkill oldBlacksmithSkill, BaseSkill newBlacksmithSkill, int playerBalance,int upgradeCost, Action<BaseSkill,int> upgradeItem)
        {
            oldItem.SetItem(oldBlacksmithSkill);
            newItem.SetItem(newBlacksmithSkill);
            newItemCost.text = upgradeCost.ToString();
            detailViewParent.SetActive(true);

            confirmUpgrade.interactable=playerBalance>=upgradeCost;
            confirmUpgrade.onClick.AddListener(delegate { upgradeItem.Invoke(newBlacksmithSkill,upgradeCost); });
        }

        /// <summary>
        /// Hides the detail view
        /// </summary>
        public void HideDetailView()
        {
            detailViewParent.SetActive(false);
        }

        public void ToggleSort()
        {
            sortParent.SetActive(!sortParent.activeSelf);
        }
    }
}