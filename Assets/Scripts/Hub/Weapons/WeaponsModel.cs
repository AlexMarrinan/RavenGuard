using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hub.Weapons
{
    public class WeaponsModel : MonoBehaviour
    {
        [SerializeField] private List<BaseWeapon> weapons=null;

        private void Awake()
        {
            LoadWeapons();
        }

        /// <summary>
        /// Gets the weapon the player currently has equipped
        /// </summary>
        /// <returns>Returns the weapon that's currently eqquiped</returns>
        public BaseWeapon GetEquippedWeapon()
        {
            LoadWeapons();
            // TODO: Get equipped weapon
            print($"Count: {weapons.Count}");
            return weapons[0];
        }

        /// <summary>
        /// Gets the currently available weapons
        /// </summary>
        /// <returns>Current weapons</returns>
        public List<BaseWeapon> GetAvailableWeapons()
        {
            LoadWeapons();
            return weapons;
        }

        /// <summary>
        /// Load the weapons from the player's inventory
        /// </summary>
        private void LoadWeapons()
        {
            // TODO: Check for changes
            print(InventoryManager.instance.GetItems<BaseWeapon>().Count);
            if (weapons == null)
            {
                weapons = InventoryManager.instance.GetItems<BaseWeapon>();
            }
        }
    }
}
