using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Hub.Interactables
{
    public class PlayerInteraction : MonoBehaviour, IInteractor
    {
        private CustomInput playerInput= null;
        private List<Interactable> closeInteractables = new();
        private Interactable activeInteractable;
        private bool isInteracting = false;
        
        private void Awake()
        {
            playerInput = new CustomInput();
        }

        private void Update()
        {
            FindClosestInteractable();
        }

        #region Enable/Disable

        private void OnEnable()
        {
            playerInput.Enable();
            playerInput.Player.Interact.performed += OnInteract;
        }

        private void OnDisable()
        {
            playerInput.Disable();
            playerInput.Player.Interact.performed -= OnInteract;
        }

        #endregion

        #region Triggers

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out Interactable interactable) && !closeInteractables.Contains(interactable))
            {
                closeInteractables.Add(interactable);
                FindClosestInteractable();
            }
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.TryGetComponent(out Interactable interactable) && closeInteractables.Contains(interactable))
            {
                closeInteractables.Remove(interactable);
                FindClosestInteractable();
            }
        }

        #endregion
        
        /// <summary>
        /// Find and store the closest interactable in range.
        /// </summary>
        private void FindClosestInteractable()
        {
            Interactable closestInteractable = null;
            float closestDistance = float.MaxValue;
            
            foreach (Interactable interactable in closeInteractables)
            {
                float distance = Vector2.Distance(transform.position, interactable.transform.position);
                if (distance < closestDistance)
                {
                    closestInteractable = interactable;
                    closestDistance = distance;
                }
            }

            //Replace activeInteractable with the closest interactable
            if (closestInteractable != activeInteractable)
            {
                activeInteractable?.Unfocus();
                activeInteractable = closestInteractable;
                activeInteractable?.Focus();
            }
        }

        /// <summary>
        /// Handles player interaction
        /// </summary>
        /// <param name="callbackContext"></param>
        private void OnInteract(InputAction.CallbackContext callbackContext)
        {
            if(!isInteracting) {activeInteractable.TryInteraction();}
            else {activeInteractable.EndInteraction();}
            isInteracting = !isInteracting;
        }
    }
}