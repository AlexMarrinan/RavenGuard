using System.Collections;
using System.Collections.Generic;
using Game.Hub.Interactables;
using UnityEngine;

namespace Buildings
{
    public class BuildingInteractable : Interactable
    {
        [SerializeField] private bool needsButtonInteraction;
        [SerializeField] private Building building;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                
            }
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                
            }
        }

        protected override void Interaction()
        {
            throw new System.NotImplementedException();
        }

        public override void EndInteraction()
        {
            throw new System.NotImplementedException();
        }

        protected override bool CanUseInteraction()
        {
            throw new System.NotImplementedException();
        }
    }
}