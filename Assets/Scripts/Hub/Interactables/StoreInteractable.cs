using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Hub.Interactables
{
    /// <summary>
    /// An interactable object that opens up a store UI
    /// </summary>
    public class StoreInteractable : Interactable
    {
        [SerializeField] private Canvas storeUI;
        [SerializeField] private SpriteRenderer sprite;
        [SerializeField] private Color focusedColor;
        [SerializeField] private Color unfocusedColor;

        /// <summary>
        /// Changes the color of the sprite
        /// </summary>
        /// <param name="isActive">Is the player sprite focusing on the store</param>
        public void ChangedColor(bool isActive)
        {
            if (isActive) sprite.color = focusedColor;
            else sprite.color = unfocusedColor;
        }

        #region Interactable

        protected override void Interaction()
        {
            Debug.Log("Store Interaction");
            storeUI.gameObject.SetActive(true);
        }

        public override void EndInteraction()
        {
            storeUI.gameObject.SetActive(false);
        }

        protected override bool CanUseInteraction()
        {
            return true;
        }

        #endregion
    }
}