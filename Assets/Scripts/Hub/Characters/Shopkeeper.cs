using Game.Dialogue;
using UnityEngine;

namespace Game.Hub.Interactables
{
    public class Shopkeeper:NPC
    {
        [SerializeField] private Canvas display;

        public override bool CanInteract()
        {
            return !EventDialogueManager.Instance.IsRunning() && !IsDisplayOn();
        }
        
        public override void Interact() {
            display.gameObject.SetActive(true);
        }

        public void CloseDisplay()
        {
            display.gameObject.SetActive(false);
        }
        
        public bool IsDisplayOn() => display.gameObject.activeSelf;
    }
}