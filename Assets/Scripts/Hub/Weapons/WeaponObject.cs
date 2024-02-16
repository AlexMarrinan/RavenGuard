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
        
        /// <summary>
        /// Initializes the weaponObject with the given weapon's data and sets up selection event
        /// </summary>
        /// <param name="weapon">The weapon being displayed</param>
        /// <param name="onWeaponClick">Event triggered when this weaponObject is clicked</param>
        public void Init(BaseWeapon weapon, Action<BaseWeapon> onWeaponClick)
        {
            weaponImage.sprite = weapon.sprite;
            onClick = onWeaponClick;
            weaponData = weapon;
        }

        /// <summary>
        /// Sets whether or not the player can interact with the weaponObject
        /// </summary>
        /// <param name="isInteractable">Should button be interactable</param>
        public void SetInteractable(bool isInteractable)
        {
            button.interactable = isInteractable;
        }

        /// <summary>
        /// When the player clicks the weapon object
        /// </summary>
        public void WeaponClicked()
        {
            onClick.Invoke(weaponData);
        }
    }
}