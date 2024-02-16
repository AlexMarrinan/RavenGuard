using System;
using System.Collections.Generic;
using Hub.UI;
using TMPro;
using UnityEngine;

namespace Hub.Weapons
{
    public class WeaponUpgrade:MonoBehaviour
    {
        private BaseWeapon weaponData;
        private SingleWeaponView view;
        [SerializeField] private List<StatUI> stats = new List<StatUI>();
        [SerializeField] private TextMeshProUGUI weaponName;
        [SerializeField] private TextMeshProUGUI rarity;
        [SerializeField] private TextMeshProUGUI desc;
        
        [SerializeField] private GameObject costPanel;
        [SerializeField] private TextMeshProUGUI cost;
        
        private bool weaponUnlocked;
        
        /// <summary>
        /// Initializes the weaponUpgrade with the given weapon's data
        /// </summary>
        /// <param name="weapon">The weapon</param>
        public void Init(BaseWeapon weapon, SingleWeaponView view)
        {
            weaponData = weapon;
            UpdateData();
            this.view = view;
        }

        public void SelectWeapon()
        {
            if (weaponUnlocked)
            {
                view.LoadWeapon(weaponData);
            }
        }

        /// <summary>
        /// Update the data
        /// </summary>
        private void UpdateData()
        {
            weaponName.text = weaponData.weaponName;
            rarity.text = "Common";
            desc.text = weaponData.weaponDescription;
            cost.text = weaponData.cost.ToString();
            LoadStats(weaponData);
            SetUnlocked(GetLockStatus());
        }

        /// <summary>
        /// Load the stats of the given weapon
        /// </summary>
        /// <param name="weapon">The weapon's data</param>
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

        private bool GetLockStatus()
        {
            // TODO: Check the save system to see if the upgrade is unlocked
            return weaponUnlocked;
        }

        /// <summary>
        /// Unlocks the weapon if the player has enough money
        /// </summary>
        public void UnlockWeapon()
        {
            // TODO: Check if player has enough money
            if (true)
            {
                SetUnlocked(true);
                //TODO: Tell save system that this weapon has been unlocked
                //TODO: Update player balance 
            }
        }

        /// <summary>
        /// Set the weapon's availability
        /// </summary>
        /// <param name="unlockedStatus">Whether or not the weapon should be available to be equipped.</param>
        private void SetUnlocked(bool unlockedStatus)
        {
            weaponUnlocked = unlockedStatus;
            costPanel.SetActive(!unlockedStatus);
        }
    }
}