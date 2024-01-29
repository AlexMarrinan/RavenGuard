using System;
using Game.Dialogue;
using UnityEngine;

namespace Game.Hub.Interactables
{
    public class Paragon:NPC
    {
        [SerializeField] private ParagonInfo paragonInfo;
        private void Awake()
        {
            gameObject.name = speakerData.characterName;
            paragonInfo.sprite = image.sprite;
            paragonInfo.speakerData = speakerData;
        }

        public override void StartCutscene()
        {
            gameObject.name = speakerData.characterName;
            EventDialogueManager.Instance.AddVariable("$CharacterName", speakerData.characterName);
            base.StartCutscene();
        }

        public override void Interact()
        {
            paragonInfo=FindObjectOfType<PlayerCharacter>().SwapParagonInfo(paragonInfo);
            image.sprite = paragonInfo.sprite;
        }
    }
}