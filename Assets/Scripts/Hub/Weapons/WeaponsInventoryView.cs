using System;
using System.Collections.Generic;
using DG.Tweening;
using Hub.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Hub.Weapons
{
    public class WeaponsInventoryView : View
    {
        [SerializeField] private Transform parent;
        [SerializeField] private Transform grid;
        [SerializeField] private WeaponObject weaponPrefab;
        [SerializeField] private Button toggleButton;
        
        [SerializeField] private float moveDuration;
        [SerializeField] private Transform hideTransform;
        [SerializeField] private Transform showTransform;
        private List<WeaponObject> weaponObjects=new List<WeaponObject>();
        
        

        public override void ShowUI(bool show)
        {
            toggleButton.interactable = false;
            Vector3 position=show ? showTransform.position : hideTransform.position;
            SetButtonsInteractable(false);
            parent.DOMove(position,moveDuration).OnComplete(() =>
            {
                OnAnimationComplete(show);
            });
        }

        public void ToggleUINoTween()
        {
            bool show;
            show = parent.position != showTransform.position;
            Vector3 position=show ? showTransform.position : hideTransform.position;
            parent.position = position;
        }

        private void OnAnimationComplete(bool show)
        {
            toggleButton.interactable = true;
            if (show)
            {
                SetButtonsInteractable(true);
            }
        }
        
        public void LoadWeapons(List<BaseWeapon> weapons, Action<BaseWeapon> onClick)
        {
            ClearGrid();
            
            foreach (BaseWeapon weapon in weapons)
            {
                WeaponObject weaponObject = Instantiate(weaponPrefab, grid);
                weaponObject.Init(weapon, onClick);
                weaponObjects.Add(weaponObject);
            }
        }

        /// <summary>
        /// Determines if you can interact with the weaponObjects
        /// </summary>
        /// <param name="isInteractable">Should the buttons be interactable</param>
        private void SetButtonsInteractable(bool isInteractable)
        {
            foreach(WeaponObject weaponObject in weaponObjects)
            {
                weaponObject.SetInteractable(isInteractable);
            }
        }

        /// <summary>
        /// Removes the child weaponObjects
        /// </summary>
        private void ClearGrid()
        {
            foreach(Transform child in grid.transform)
            {
                Destroy(child.gameObject);
            }
            weaponObjects.Clear();
        }
    }
}
