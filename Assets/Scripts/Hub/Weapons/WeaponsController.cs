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
        private Action<BaseWeapon> weaponSelected;
        

        void Awake()
        {
            weaponSelected = OpenSingleWeaponView;
        }
        
        public override void ToggleView()
        {
            if (!view.activeSelf)
            {
                OpenWeaponsShop();
            }
            else
            {
                weaponsInventoryView.ShowUI(false);
            }
            view.SetActive(!view.activeSelf);
        }

        public void OpenWeaponsShop()
        {
            weaponsInventoryView.LoadWeapons(weaponsModel.GetAvailableWeapons(),weaponSelected);
            singleWeaponView.LoadWeapon(weaponsModel.GetEquippedWeapon());
        }

        private void OpenSingleWeaponView(BaseWeapon weapon)
        {
            singleWeaponView.LoadWeapon(weapon);
        }
        
    }
}
