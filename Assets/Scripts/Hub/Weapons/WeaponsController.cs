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
                weaponsInventoryView.LoadWeapons(weaponsModel.GetAvailableWeapons(),weaponSelected);
            }
            view.SetActive(!view.activeSelf);
        }

        public void OpenWeaponsShop()
        {
            weaponsInventoryView.LoadWeapons(weaponsModel.GetAvailableWeapons(),weaponSelected);
            OpenWeaponInventory();
        }

        void CloseWeaponsShop()
        {
            singleWeaponView.ShowUI(false);
            weaponsInventoryView.ShowUI(false);
        }

        public void OpenWeaponInventory()
        {
            singleWeaponView.ShowUI(false);
            weaponsInventoryView.ShowUI(true);
        }

        public void ToggleWeaponInventory()
        {
            weaponsInventoryView.ToggleUI();
        }

        private void OpenSingleWeaponView(BaseWeapon weapon)
        {
            singleWeaponView.ShowUI(true);
            weaponsInventoryView.ShowUI(false);
        }
        
    }
}
