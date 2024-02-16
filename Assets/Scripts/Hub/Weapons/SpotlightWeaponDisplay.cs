using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Hub.Weapons
{
    public class SpotlightWeaponDisplay:MonoBehaviour
    {
        private BaseWeapon weaponData;
        [SerializeField] private TextMeshProUGUI weaponName;
        [SerializeField] private Image weaponImage;
        [SerializeField] private TextMeshProUGUI weaponEffect;
        [SerializeField] private TextMeshProUGUI weaponDesc;
        
        public void Init(SingleWeaponView singleWeaponView)
        {
            singleWeaponView.onWeaponSelected += LoadWeapon;
        }
        
        /// <summary>
        /// Load the given weapon's data
        /// </summary>
        /// <param name="weapon">The weapon being selected</param>
        private void LoadWeapon(BaseWeapon weapon)
        {
            weaponData = weapon;
            LoadCurrentWeapon();
        }
        
        /// <summary>
        /// Load in the current weapon's data
        /// </summary>
        private void LoadCurrentWeapon()
        {
            if (weaponData == null) return;
            weaponName.text = weaponData.weaponName;
            weaponImage.sprite = weaponData.sprite;
            weaponEffect.text = weaponData.damage + " Damage";
            weaponDesc.text = weaponData.weaponDescription;
        }
    }
}