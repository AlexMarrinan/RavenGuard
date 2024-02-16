using System;
using System.Collections.Generic;
using Hub.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Hub.Weapons
{
    public class SingleWeaponView : View
    {
        private BaseWeapon weaponData;
        [SerializeField] private SpotlightWeaponDisplay spotlightDisplay;
        [SerializeField] private WeaponUpgradeDisplay upgradeDisplay;
        public Action<BaseWeapon> onWeaponSelected;
        private bool initialized;

        private void Awake()
        {
            Init();
        }

        void Init()
        {
            initialized = true;
            spotlightDisplay.Init(this);
            upgradeDisplay.Init(this);
        }
        
        /// <summary>
        /// Load the given weapon's data
        /// </summary>
        /// <param name="weapon">The weapon being selected</param>
        public void LoadWeapon(BaseWeapon weapon)
        {
            if(!initialized) Init();
            if(weapon==null) return;
            onWeaponSelected.Invoke(weapon);
        }
    }
}
