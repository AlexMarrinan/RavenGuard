using System;
using UnityEngine;
using UnityEngine.UI;

namespace Hub.Weapons
{
    public class WeaponObject:MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Button button;
        [SerializeField] private Image weaponImage;
        //Internal
        private BaseWeapon weaponData;
        private Action<BaseWeapon> onClick;
        public void Init(BaseWeapon weapon, Action<BaseWeapon> onWeaponClick)
        {
            weaponImage.sprite = weapon.sprite;
            onClick = onWeaponClick;
        }

        public void SetInteractable(bool isInteractable)
        {
            button.interactable = isInteractable;
        }

        public void WeaponClicked()
        {
            onClick.Invoke(weaponData);
        }
    }
}