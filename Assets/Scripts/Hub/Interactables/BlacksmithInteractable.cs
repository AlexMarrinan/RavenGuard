using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

namespace Game.Hub.Interactables
{
    public class BlacksmithInteractable : MonoBehaviour
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
    }
}
