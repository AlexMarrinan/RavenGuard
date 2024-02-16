using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Weapons
{
    [CreateAssetMenu(fileName = "New Weapon Upgrade Group", menuName = "Weapon/Upgrade Group", order = 0)]
    public class WeaponUpgradeGroup: ScriptableObject
    {
        public List<BaseWeapon> weaponUpgrades;
        public string nameConvention;
        
        #if UNITY_EDITOR
        public void SetWeaponReferences()
        {
            foreach (BaseWeapon weapon in weaponUpgrades)
            {
                weapon.weaponUpgradeGroup = this;
                int num = weaponUpgrades.IndexOf(weapon);
                weapon.name = nameConvention + num;
                weapon.weaponName = nameConvention + " " + num;
                weapon.weaponDescription="Woah! A really cool description would go here: "+ weaponUpgrades.IndexOf(weapon);
                EditorUtility.SetDirty(weapon);
                AssetDatabase.SaveAssets();
            }
        }
        #endif
    }
}