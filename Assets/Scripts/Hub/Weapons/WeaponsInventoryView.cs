using System;
using System.Collections.Generic;
using Hub.UI;
using UnityEngine;

namespace Hub.Weapons
{
    public class WeaponsInventoryView : View
    {
        [SerializeField] private Transform grid;
        [SerializeField] private WeaponObject weaponPrefab;
        
        public void LoadWeapons(List<BaseWeapon> weapons, Action<BaseWeapon> onClick)
        {
            ClearGrid();
            
            foreach (BaseWeapon weapon in weapons)
            {
                WeaponObject weaponObject = Instantiate(weaponPrefab, grid);
                weaponObject.Init(weapon, onClick);
            }
        }

        private void ClearGrid()
        {
            foreach(Transform child in grid.transform)
            {
                Destroy(child.gameObject);
            }
        }
    }
}
