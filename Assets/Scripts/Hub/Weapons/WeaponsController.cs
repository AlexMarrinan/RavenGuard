using System;
using System.Collections.Generic;
using Hub.UI;
using UnityEngine;
using UnityEngine.Events;

namespace Hub.Weapons
{
    public class WeaponsController : Controller
    {
        // References
        [SerializeField] private GameObject view;
        [SerializeField] private WeaponsModel weaponsModel;
        [SerializeField] private SingleWeaponView singleWeaponView;
        [SerializeField] private WeaponsInventoryView weaponsInventoryView;
        public Action<BaseWeapon> weaponSelected;
        

        void Awake()
        {
            weaponSelected = SelectWeapon;
        }
        
        /// <summary>
        /// Toggle the weapon shop open and closed
        /// </summary>
        public override void ToggleView()
        {
            if (!view.activeSelf) OpenShop();
            else CloseShop();
        }

        /// <summary>
        /// Opens the weapon shop
        /// </summary>
        private void OpenShop()
        {
            LoadWeaponsShop();
            view.SetActive(true);
        }

        /// <summary>
        /// Closes the weapon shop
        /// </summary>
        private void CloseShop()
        {
            weaponsInventoryView.ShowUI(false);
            view.SetActive(false);
        }
        
        /// <summary>
        /// Loads the weapons in the inventory and the equipped weapon in single view
        /// </summary>
        private void LoadWeaponsShop()
        {
            weaponsInventoryView.LoadWeapons(weaponsModel.GetAvailableWeapons(),weaponSelected);
            singleWeaponView.LoadWeapon(weaponsModel.GetEquippedWeapon());
        }

        /// <summary>
        /// Set the given weapon to be shown in singleWeaponView
        /// </summary>
        /// <param name="weapon">The weapon being shown</param>
        private void SelectWeapon(BaseWeapon weapon)
        {
            singleWeaponView.LoadWeapon(weapon);
        }
        
    }
}
