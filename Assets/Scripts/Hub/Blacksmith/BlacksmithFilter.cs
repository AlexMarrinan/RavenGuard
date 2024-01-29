using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Hub.Blacksmith
{
    public class BlacksmithFilter:MonoBehaviour
    {
        [SerializeField] private BlacksmithStoreView view;
        [SerializeField] private TMP_Dropdown rarityDropdown;
        [SerializeField] private TMP_Dropdown skillType;
        [SerializeField] private TMP_Dropdown skillRestrictions;
        [SerializeField] private Button sortByCost;
        [SerializeField] private Button sortByRarity;
        [SerializeField] private Button sortByRank;

        private void Awake()
        {
            rarityDropdown.onValueChanged.AddListener(delegate(int arg0) { OnRarityChange(); });
            skillType.onValueChanged.AddListener(delegate(int arg0) { OnSkillTypeChange(); });
            skillRestrictions.onValueChanged.AddListener(delegate(int arg0) { OnSkillRestrictions(); });
            sortByCost.onClick.AddListener(delegate { view.SortSkillsBy((SortBy.Cost)); });
            sortByRank.onClick.AddListener(delegate { view.SortSkillsBy((SortBy.Rank)); });
            sortByRarity.onClick.AddListener(delegate { view.SortSkillsBy((SortBy.Rarity)); });
            LoadFilters();
        }

        #region Load Filters

        private void LoadFilters()
        {
            LoadOptions(rarityDropdown,typeof(RarityFilter));
            LoadOptions(skillType,typeof(SkillTypeFilter));
            LoadOptions(skillRestrictions,typeof(SkillRestrictionsFilter));
        }

        private List<TMP_Dropdown.OptionData> GetDropdownOptions(List<string> list)
        {
            List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
            foreach (string str in list)
            {
                TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(str);
                options.Add(option);
            }

            return options;
        }
        
        private void LoadOptions(TMP_Dropdown dropdown,Type type )
        {
            dropdown.options.Clear();
            List<string> values = Enum.GetNames(type).ToList();
            List<TMP_Dropdown.OptionData> options = GetDropdownOptions(values);
            dropdown.options.AddRange(options);
        }

        #endregion
        
        private void OnRarityChange()
        {
            RarityFilter rarityValue=(RarityFilter) Enum.Parse(typeof(RarityFilter), rarityDropdown.captionText.text);
            view.FilterRarity(rarityValue);
        }

        private void OnSkillTypeChange()
        {
            SkillTypeFilter filter=(SkillTypeFilter)Enum.Parse(typeof(SkillTypeFilter), skillType.captionText.text);
            view.FilterSkillType(filter);
        }

        private void OnSkillRestrictions()
        {
            SkillRestrictionsFilter filter=(SkillRestrictionsFilter)Enum.Parse(typeof(SkillRestrictionsFilter), skillRestrictions.captionText.text);
            view.FilterSkillRestrictions(filter);
        }
    }

    public enum SortBy
    {
        Cost,
        Rarity,
        Rank
    }
    
    public enum SkillRestrictionsFilter
    {
        Restrictions=0,
        R1=1,
        R2=2,
        R3=3
    }
    
    public enum SkillTypeFilter
    {
        Type=0,
        Active=1,
        Passive=2
    }
    
    public enum RarityFilter
    {
        Rarity=0,
        Common=1,
        Rare=2,
        Legendary=3
    }
}