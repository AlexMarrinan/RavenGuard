using System.Collections.Generic;
using Hub.UI;
using TMPro;
using UnityEngine;

namespace Hub.Weapons
{
    public class WeaponUpgrade:MonoBehaviour
    {
        [SerializeField] private List<StatUI> stats = new List<StatUI>();
        [SerializeField] private TextMeshProUGUI weaponName;
        [SerializeField] private TextMeshProUGUI rarity;
        [SerializeField] private TextMeshProUGUI desc;
        
        [SerializeField] private GameObject costPanel;
        [SerializeField] private TextMeshProUGUI cost;

        private bool weaponUnlocked;
        
        public void Init(WeaponType weapon)
        {
            weaponName.text = weapon.weapon.weaponName;
            rarity.text = "Common";
            desc.text = weapon.weapon.weaponDescription;
            cost.text = weapon.cost.ToString();
            LoadStats(weapon.weapon);
            SetUnlocked(false);
        }

        private void LoadStats(BaseWeapon weapon)
        {
            List<Sprite> statIcons = weapon.statIcons;
            for (int i = 0; i < stats.Count; i++)
            {
                stats[i].gameObject.SetActive(i < statIcons.Count);
                if (i < statIcons.Count)
                {
                    stats[i].Init(statIcons[i]);
                }
            }
        }

        public void UnlockWeapon()
        {
            // TODO: Check if player has enough money
            if (true)
            {
                SetUnlocked(true);
                //TODO: Tell save system that this weapon has been unlocked
            }
        }

        private void SetUnlocked(bool unlockedStatus)
        {
            weaponUnlocked = unlockedStatus;
            costPanel.SetActive(unlockedStatus);
        }
    }
}