using System;
using Game.Dialogue;
using UnityEngine;
using UnityEngine.Events;
using Yarn.Unity;

namespace Game.Hub.Interactables
{
    public class CharacterInteractable:Interactable
    {
        [SerializeField] private UnityEvent onInteractionStart;
        [SerializeField] private UnityEvent onInteractionEnd;
        
        [Header("References")]
        [SerializeField] private NPC npc;

        protected override void Interaction()
        {
            onInteractionStart.Invoke();
        }

        public override void EndInteraction()
        {
            onInteractionEnd.Invoke();
        }

        protected override bool CanUseInteraction()
        {
            return npc.CanInteract();
        }
    }
}