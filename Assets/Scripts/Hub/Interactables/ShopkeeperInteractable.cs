using UnityEngine;
using Yarn.Unity;

namespace Game.Hub.Interactables
{
    public class ShopkeeperInteractable:Interactable
    {
        [SerializeField] private Canvas storeUI;
        
        [YarnCommand("Interact")]
        public void OpenShop() {
            storeUI.gameObject.SetActive(true);
        }

        public void CloseShop()
        {
            storeUI.gameObject.SetActive(false);
        }

        protected override void Interaction()
        {
            throw new System.NotImplementedException();
        }

        public override void EndInteraction()
        {
            if(storeUI.isActiveAndEnabled) CloseShop();
        }

        protected override bool CanUseInteraction()
        {
            return true;
        }
    }
}