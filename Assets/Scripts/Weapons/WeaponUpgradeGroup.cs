using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Weapons
{
    [CreateAssetMenu(fileName = "New Weapon Upgrade Group", menuName = "Weapon/Upgrade Group", order = 0)]
    public class WeaponUpgradeGroup: ScriptableObject
    {
        public List<BaseWeapon> weaponUpgrades;
        
        #if UNITY_EDITOR
        public void SetWeaponReferences()
        {
            foreach (BaseWeapon weapon in weaponUpgrades)
            {
                weapon.weaponUpgradeGroup = this;
                EditorUtility.SetDirty(weapon);
                AssetDatabase.SaveAssets();
            }
        }
        #endif
    }
}