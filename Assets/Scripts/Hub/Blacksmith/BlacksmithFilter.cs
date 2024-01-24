using System;
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
        }
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
        None=0
    }
    
    public enum SkillTypeFilter
    {
        None=0,
        Active=1,
        Passive=2
    }
    
    public enum RarityFilter
    {
        None=0,
        Common=1,
        Rare=2,
        Legendary=3
    }
}