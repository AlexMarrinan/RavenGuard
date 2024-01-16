using Game.Hub.Interactables;
using UnityEngine;

namespace Buildings
{
    /// <summary>
    /// A interactable object that opens new areas to the player
    /// </summary>
    public class DoorInteractable: Interactable
    {
        [SerializeField] private Building building;
        [SerializeField] private bool locked;
        
        
        public void OutlineSprite()
        {
            
        }
        
        protected override void Interaction()
        {
            building.EnterBuilding();
        }

        public override void EndInteraction()
        {
            building.ExitBuilding();
        }

        protected override bool CanUseInteraction()
        {
            return !locked;
        }
    }
}