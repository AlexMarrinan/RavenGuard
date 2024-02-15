using System;
using UnityEngine;
using UnityEngine.UI;

namespace Hub.Weapons
{
    public class WeaponObject:MonoBehaviour
    {
        private BaseWeapon weaponData;
        private Image weaponImage;
        private Action<BaseWeapon> onClick;
        public void Init(BaseWeapon weapon, Action<BaseWeapon> onWeaponClick)
        {
            weaponImage.sprite = weapon.sprite;
            onClick = onWeaponClick;
        }

        public void WeaponClicked()
        {
            onClick.Invoke(weaponData);
        }
    }
}