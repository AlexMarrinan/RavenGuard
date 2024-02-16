using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hub.Weapons
{
    public class WeaponUpgradeDisplay:MonoBehaviour
    {
        private SingleWeaponView view;
        [SerializeField] private List<WeaponUpgrade> weaponUpgrades;
        
        public void Init(SingleWeaponView view)
        {
            view.onWeaponSelected += LoadUpgrades;
            this.view = view;
        }
        /// <summary>
        /// Load the upgrades of the current weaponData
        /// </summary>
        private void LoadUpgrades(BaseWeapon weapon)
        {
            List<BaseWeapon> upgrades= weapon.weaponUpgradeGroup.weaponUpgrades;
            upgrades.Remove(weapon);
            for (int i = 0; i < weaponUpgrades.Count; i++)
            {
                if(i >= upgrades.Count)
                {
                    weaponUpgrades[i].gameObject.SetActive(false);
                    continue;
                }
                weaponUpgrades[i].Init(upgrades[i],view);
            }
            upgrades.Add(weapon);
        }
    }
}