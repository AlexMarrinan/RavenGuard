using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Hub.Interactables
{
    public class StoreInteractable : Interactable
    {
        [SerializeField] public Canvas storeUI;
        
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
    }
}