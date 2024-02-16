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

        public List<BaseWeapon> GetAvailableWeapons()
        {
            // TODO: Check for changes
            //return InventoryManager.instance.GetItems<BaseWeapon>();
            return weapons;
        }
    }
}
