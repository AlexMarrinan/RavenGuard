using System;
using System.Collections;
using System.Collections.Generic;
using Hub.UI;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Hub.Interactables
{
    /// <summary>
    /// An interactable object that opens up a store UI
    /// </summary>
    public class StoreInteractable : Interactable
    {
        [SerializeField] public Controller uiController;
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
            uiController.ToggleView();
        }

        public override void EndInteraction()
        {
            uiController.ToggleView();
        }

        protected override bool CanUseInteraction()
        {
            return true;
        }

        #endregion
    }
}