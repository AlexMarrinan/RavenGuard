using Game.Hub.Interactables;
using UnityEngine;
using UnityEngine.Events;

namespace Buildings
{
    /// <summary>
    /// A interactable object that opens new areas to the player
    /// </summary>
    public class DoorInteractable: Interactable
    {
        [SerializeField] private UnityEvent onEnter;
        [SerializeField] private UnityEvent onExit;
        [SerializeField] private bool locked;
        
        
        public void OutlineSprite()
        {
            
        }
        
        protected override void Interaction()
        {
            onEnter.Invoke();
        }

        public override void EndInteraction()
        {
            onExit.Invoke();
        }

        protected override bool CanUseInteraction()
        {
            return !locked;
        }
    }
}