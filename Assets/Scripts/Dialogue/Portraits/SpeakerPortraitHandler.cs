using System;
using System.Globalization;
using Game.Hub.Interactables;
using UnityEngine;
using Yarn.Unity;

namespace Game.Dialogue.Portraits
{
    /// <summary>
    /// Updates and manages portrait for a speaker during dialogue.
    /// </summary>
    [RequireComponent(typeof(DialogueCharacterNameView))]
    public class SpeakerPortraitHandler : MonoBehaviour
    {
        // References
        [SerializeField] private NPC npc;
        [SerializeField] private SpeakerData speakerData;

        void Awake()
        {
            if(npc) speakerData = npc.speakerData;
            gameObject.name = speakerData.name;
            
            
        }

        private void Start()
        {
            // Event Subscriptions
            EventDialogueManager.Instance.portraitLineView.onNameUpdate += ShouldSetSprite;
        }

        private void OnDestroy()
        {
            // Event Subscriptions
            if (EventDialogueManager.Instance?.portraitLineView)
            {
                EventDialogueManager.Instance.portraitLineView.onNameUpdate -= ShouldSetSprite;
            }
        }

        /// <summary>
        /// If name equals characterName, trigger SetSprite.
        /// </summary>
        /// <param name="speakerName">The name of a speaker from dialogue</param>
        private void ShouldSetSprite(string speakerName)
        {
            if (speakerName == name)
            {
                SetSprite("Default");
            }
        }
        
        /// <summary>
        /// Sets the portrait of the speaker based on the given PortraitType.
        /// </summary>
        /// <param name="portraitType">The emotion of the speaker.</param>
        [YarnCommand("SetSprite")]
        public void SetSprite(string portraitType)
        {
            PortraitType portrait;
            TextInfo textInfo = new CultureInfo("en-US",false).TextInfo;
            if (Enum.TryParse(textInfo.ToTitleCase(portraitType), out portrait))
            {
                EventDialogueManager.Instance.SetSpeaker(speakerData.GetPortrait(portrait));
            }
            else
            {
                EventDialogueManager.Instance.HideSpeaker();
                Debug.LogError($"PortraitEmotion not found for the specified: {portraitType}.");
            }
        }
    }
}