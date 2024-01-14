using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Hub.Interactables
{
    public abstract class Interactable : MonoBehaviour
    {
        // Static
        internal static Action<Interactable> onFocused;
        internal static Action<Interactable> onUnfocused;
        
        // Inspector
        [Header("Feedback")]
        [Tooltip("The event invoked when an interactable receives focus from the player.")]
        [SerializeField] private UnityEvent onFocus;
        [Tooltip("The event invoked when an interactable loses focus from the player.")]
        [SerializeField] private UnityEvent onUnfocus;
        
        /// <summary>
        /// Focus on this interactable.
        /// </summary>
        public void Focus()
        {
            onFocus?.Invoke();
            onFocused?.Invoke(this);
        }
        
        /// <summary>
        /// Unfocus on this interactable.
        /// </summary>
        public void Unfocus()
        {
            onUnfocus?.Invoke();
            onUnfocused?.Invoke(this);
        }
        
        /// <summary>
        /// Attempt to interact.
        /// </summary>
        public void TryInteraction()
        {
            // Validation
            if (!CanUseInteraction()) return;
            
            // Interaction
            Interaction();
        }
        
        #region Interactable Implementation
        
        /// <summary>
        /// The interaction defined for this interactable.
        /// </summary>
        protected abstract void Interaction();

        /// <summary>
        /// Ends the object's interaction
        /// </summary>
        public abstract void EndInteraction();
        
        /// <summary>
        /// Check whether this interactable accepts interaction.
        /// </summary>
        /// <returns>Whether this interactable accepts interaction.</returns>
        protected abstract bool CanUseInteraction();
        #endregion
    }
}
