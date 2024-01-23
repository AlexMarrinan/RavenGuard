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
        [SerializeField] private BlacksmithStoreController controller;
        [SerializeField] private TextMeshProUGUI playerMoney;
        [SerializeField] private Transform skillParent;
        [SerializeField] private GameObject sortParent;
        
        [Header("Detail View")] 
        [SerializeField] private GameObject detailViewParent;
        [SerializeField] private DetailView oldItem;
        [SerializeField] private DetailView newItem;
        [SerializeField] private Button leaveDetailedView;
        [SerializeField] private Button confirmUpgrade;
        [SerializeField] private TextMeshProUGUI newItemCost;

        [Header("Prefabs")] 
        [SerializeField] private BlacksmithSkill skillPrefab;
        
        // Internals
        private UpgradableSkill currentSkills;
        private List<BlacksmithSkill> blacksmithSkills = new List<BlacksmithSkill>();

        public void Init(int money, List<UpgradableSkill> skills, BlacksmithStoreController blacksmithStoreController)
        {
            playerMoney.text = money.ToString();
            controller = blacksmithStoreController;
            LoadSkills(skills);
        }
        
        /// <summary>
        /// Updates the playerMoney text to match the playerBalance.
        /// </summary>
        /// <param name="playerBalance">How much money the player has.</param>
        public void UpdatePlayerBalance(int playerBalance)
        {
            playerMoney.text = playerBalance+"G";
        }

        #region Skills
        
        private void ShowSkillCost(bool showCost)
        {
            foreach (BlacksmithSkill skill in blacksmithSkills)
            {
                skill.ShowCost(showCost);
            }
        }

        public void ConfirmSkillUpgrade()
        {
            controller.ConfirmSkillUpgrade();
            
        }

        #endregion

        #region Detail View

        public void ToggleDetailView(UpgradableSkill skill)
        {
            if (currentSkills.newSkill != skill.newSkill)
            {
                currentSkills = skill;
                controller.currentSkill = skill;

                OpenDetailView();
                detailViewParent.SetActive(true);
                ShowSkillCost(false);
            }
            else
            {
                detailViewParent.SetActive(false);
                ShowSkillCost(true);
            }
        }
        
        /// <summary>
        /// Open the detail view and set its info
        /// </summary>
        private void OpenDetailView()
        {
            //Updates skill info
            oldItem.SetItem(currentSkills.oldSkill);
            newItem.SetItem(currentSkills.newSkill);
            newItemCost.text = currentSkills.cost+"G";
            
            //If the player has enough money, they can upgrade the skill
            confirmUpgrade.interactable=controller.GetPlayerBalance()>=currentSkills.cost;
            
            // Show the detail view
            detailViewParent.SetActive(true);
            
            //Hide each skill upgrade cost
            ShowSkillCost(false);
        }

        /// <summary>
        /// Hides the detail view
        /// </summary>
        public void HideDetailView()
        {
            detailViewParent.SetActive(false);
        }

        #endregion

        #region Instantiating Skills

        /// <summary>
        /// Instantiates the given skill
        /// </summary>
        /// <param name="skills">The skill</param>
        private void LoadSkills(List<UpgradableSkill> skills)
        {
            ClearOldSkills();
            foreach (UpgradableSkill skill in skills)
            {
                BlacksmithSkill blacksmithSkill=Instantiate(skillPrefab, skillParent);
                blacksmithSkill.Init(skill,this);
                blacksmithSkills.Add(blacksmithSkill);
            }
        }
        
        /// <summary>
        /// Removes skills if present
        /// </summary>
        private void ClearOldSkills()
        {
            while(skillParent.transform.childCount != 0)
            {
                Destroy(skillParent.transform.GetChild(0).transform);
            }
        }

        public void ReloadSkills(List<UpgradableSkill> skills)
        {
            ClearOldSkills();
            LoadSkills(skills);
        }

        #endregion

        public void ToggleSort()
        {
            sortParent.SetActive(!sortParent.activeSelf);
        }
        
        
    }
}