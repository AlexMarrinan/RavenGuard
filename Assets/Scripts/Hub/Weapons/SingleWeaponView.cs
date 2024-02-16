using System.Collections.Generic;
using Hub.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Hub.Weapons
{
    public class SingleWeaponView : View
    {
        private BaseWeapon weaponData;
        [SerializeField] private TextMeshProUGUI weaponName;
        [SerializeField] private Image weaponImage;
        [SerializeField] private TextMeshProUGUI weaponEffect;
        [SerializeField] private TextMeshProUGUI weaponDesc;
        [SerializeField] private List<WeaponUpgrade> weaponUpgrades;

        public void LoadWeapon(BaseWeapon weapon)
        {
            weaponData = weapon;
            LoadCurrentWeapon();
            LoadUpgrades();
        }

        private void LoadCurrentWeapon()
        {
            if (weaponData == null) return;
            weaponName.text = weaponData.weaponName;
            weaponImage.sprite = weaponData.sprite;
            weaponEffect.text = weaponData.damage + " Damage";
            weaponDesc.text = weaponData.weaponDescription;
        }

        private void LoadUpgrades()
        {
            if (weaponData == null) return;
            for (int i = 0; i < weaponUpgrades.Count; i++)
            {
                if(i >= weaponData.weaponUpgrades.Count)
                {
                    weaponUpgrades[i].gameObject.SetActive(false);
                    continue;
                }
                weaponUpgrades[i].Init(weaponData.weaponUpgrades[i]);
            }
        }
        
    }
}
