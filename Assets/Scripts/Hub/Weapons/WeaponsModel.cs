using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hub.Weapons
{
    public class WeaponsModel : MonoBehaviour
    {
        [SerializeField] private List<BaseWeapon> weapons;

        private void Awake()
        {
            //TODO: Load available weapons
        }

        public BaseWeapon GetEquippedWeapon()
        {
            print("GetEquippedWeapon()");
            // TODO: Get equipped weapon
            if (weapons == null)
            {
                weapons = InventoryManager.instance.GetItems<BaseWeapon>();
            }
            return weapons[0];
        }

        public List<BaseWeapon> GetAvailableWeapons()
        {
            // TODO: Check for changes
            weapons = InventoryManager.instance.GetItems<BaseWeapon>();
            return weapons;
        }
    }
}
