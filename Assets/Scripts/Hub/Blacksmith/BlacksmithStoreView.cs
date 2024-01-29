using System;
using System.Collections.Generic;
using System.Linq;
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

        /// <summary>
        /// Toggles the detail view on or off
        /// </summary>
        /// <param name="upgradableSkill">Skill that could be shown in detail view</param>
        public void ToggleDetailView(UpgradableSkill upgradableSkill)
        {
            detailView.ToggleDetailView(upgradableSkill);
        }
        
        public void ToggleSort()
        {
            sortParent.SetActive(!sortParent.activeSelf);
        }

        #region Sorting & Filtration

        public void SortSkillsBy(SortBy sortBy, bool isDescending = true)
        {
            if (sortBy == SortBy.Cost)
            {
                if (isDescending)
                {
                    blacksmithSkills=blacksmithSkills.OrderByDescending(skill => skill.cost).ToList();
                }
                else
                {
                    blacksmithSkills=blacksmithSkills.OrderBy(skill => skill.cost).ToList();
                }
            }else if (sortBy == SortBy.Rarity)
            {
                if (isDescending)
                {
                    blacksmithSkills=blacksmithSkills.OrderByDescending(skill => skill.skillData.rarity).ToList();
                }
                else
                {
                    blacksmithSkills=blacksmithSkills.OrderBy(skill => skill.skillData.rarity).ToList();
                }
            }else if (sortBy == SortBy.Rank)
            {
                if (isDescending)
                {
                    blacksmithSkills=blacksmithSkills.OrderByDescending(skill => skill.rank).ToList();
                }
                else
                {
                    blacksmithSkills=blacksmithSkills.OrderBy(skill => skill.rank).ToList();
                }
            }

            for (int i = 0; i < blacksmithSkills.Count; i++)
            {
                blacksmithSkills[i].transform.SetSiblingIndex(i);
            }
        }
        
        
        public void FilterRarity(RarityFilter filter)
        {
            if (filter == RarityFilter.Rarity)
            {
                FilterNone();
            }
            else
            {
                foreach (BlacksmithSkill blacksmithSkill in blacksmithSkills)
                {
                    if(blacksmithSkill.skillData.rarity != filter)
                    {
                        blacksmithSkill.gameObject.SetActive(false);
                    }else
                    {
                        blacksmithSkill.gameObject.SetActive(true);
                    }
                }
            }
        }
        
        public void FilterSkillType(SkillTypeFilter filter)
        {
            if (filter == SkillTypeFilter.Type)
            {
                FilterNone();
            }else{
                foreach (BlacksmithSkill blacksmithSkill in blacksmithSkills)
                {
                    if(blacksmithSkill.skillData.skillType != filter)
                    {
                        blacksmithSkill.gameObject.SetActive(false);
                    }else
                    {
                        blacksmithSkill.gameObject.SetActive(true);
                    }
                }
            }
        }

        public void FilterSkillRestrictions(SkillRestrictionsFilter filter)
        {
            if (filter == SkillRestrictionsFilter.Restrictions)
            {
                FilterNone();
            }else{
                foreach (BlacksmithSkill blacksmithSkill in blacksmithSkills)
                {
                    if(blacksmithSkill.skillData.skillRestrictions != filter)
                    {
                        blacksmithSkill.gameObject.SetActive(false);
                    }else
                    {
                        blacksmithSkill.gameObject.SetActive(true);
                    }
                }
            }
        }

        private void FilterNone()
        {
            foreach (BlacksmithSkill blacksmithSkill in blacksmithSkills)
            {
                blacksmithSkill.gameObject.SetActive(true);
            }
        }
        #endregion
    }

    
}