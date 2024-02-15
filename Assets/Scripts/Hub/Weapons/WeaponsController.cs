using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Hub.Weapons
{
    public class WeaponsController : MonoBehaviour
    {
        // References
        [SerializeField] private WeaponsModel weaponsModel;
        [SerializeField] private SingleWeaponView singleWeaponView;
        [SerializeField] private WeaponsInventoryView weaponsInventoryView;
        private Action<BaseWeapon> weaponSelected;

        void Awake()
        {
            weaponSelected = OpenSingleWeaponView;
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

        private void OpenWeaponInventory()
        {
            singleWeaponView.ShowUI(false);
            weaponsInventoryView.ShowUI(true);
        }

        private void OpenSingleWeaponView(BaseWeapon weapon)
        {
            singleWeaponView.ShowUI(true);
            weaponsInventoryView.ShowUI(false);
        }
    }
}
