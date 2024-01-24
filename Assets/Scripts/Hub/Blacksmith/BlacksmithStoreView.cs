using System.Collections.Generic;
using Hub.UI;
using Skills;
using TMPro;
using UnityEngine;

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
        [SerializeField] private DetailView detailView;

        [Header("Prefabs")] 
        [SerializeField] private BlacksmithSkill skillPrefab;
        
        //Internal
        private List<BlacksmithSkill> blacksmithSkills = new List<BlacksmithSkill>();

        public void Init(int money, List<UpgradableSkill> skills, BlacksmithStoreController blacksmithStoreController)
        {
            playerMoney.text = money.ToString();
            controller = blacksmithStoreController;
            detailView.Init(blacksmithStoreController,this);
            LoadSkills(skills);
        }
        
        /// <summary>
        /// Updates the playerMoney text to match the playerBalance.
        /// </summary>
        /// <param name="playerBalance">How much money the player has.</param>
        private void UpdatePlayerBalance(int playerBalance)
        {
            playerMoney.text = playerBalance+"G";
        }

        #region Skills

        /// <summary>
        /// Upgrades the skill and subtracts the cost
        /// </summary>
        /// <param name="upgradableSkill"></param>
        public void ConfirmSkillUpgrade(UpgradableSkill upgradableSkill)
        {
            UpgradableSkill skill= controller.ConfirmSkillUpgrade(upgradableSkill);
            int money = controller.GetPlayerBalance();
            UpdatePlayerBalance(money);
            UpdateSkillGameObject(upgradableSkill,skill);
            detailView.HideDetailView();
        }

        /// <summary>
        /// Updates the oldSkill game object with the newSkill's information. If newSkill is null, destroy the game object
        /// </summary>
        /// <param name="oldSkill">The skill being replaced</param>
        /// <param name="newSkill">The skill replacing the oldSkill</param>
        private void UpdateSkillGameObject(UpgradableSkill oldSkill,UpgradableSkill newSkill)
        {
            foreach (BlacksmithSkill blacksmithSkill in blacksmithSkills)
            {
                if (blacksmithSkill.skillData == oldSkill.next)
                {
                    if (newSkill == null)
                    {
                        Destroy(blacksmithSkill.gameObject);
                    }
                    else
                    {
                        blacksmithSkill.Init(newSkill);
                    }
                    return;
                }
            }
        }

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

        #endregion

        public void ToggleSort()
        {
            sortParent.SetActive(!sortParent.activeSelf);
        }

        /// <summary>
        /// Toggles the detail view on or off
        /// </summary>
        /// <param name="upgradableSkill">Skill that could be shown in detail view</param>
        public void ToggleDetailView(UpgradableSkill upgradableSkill)
        {
            detailView.ToggleDetailView(upgradableSkill);
        }
    }
}