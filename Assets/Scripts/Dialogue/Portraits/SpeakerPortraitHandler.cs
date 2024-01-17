using System;
using System.Globalization;
using UnityEngine;
using Yarn.Unity;

namespace Game.Dialogue.Portraits
{
    /// <summary>
    /// Updates and manages portrait for a speaker during dialogue.
    /// </summary>
    public class SpeakerPortraitHandler : MonoBehaviour
    {
        // Internal
        private ISpeaker speaker;
        
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
        /// Initialize a portrait handler for the character.
        /// </summary>
        /// <param name="speaker">A speaker involved in dialogue.</param>
        public void Initialize(ISpeaker speaker)
        {
            this.speaker = speaker;
            gameObject.name = speaker.GetName();
        }

        /// <summary>
        /// If name equals characterName, trigger SetSprite.
        /// </summary>
        /// <param name="speakerName">The name of a speaker from dialogue</param>
        private void ShouldSetSprite(string speakerName)
        {
            print(speakerName);
            if (speakerName == name)
            {
                SetSprite("Neutral");
            }
        }
        
        /// <summary>
        /// Sets the portrait of the speaker based on the given PortraitEmotion.
        /// </summary>
        /// <param name="portraitEmotion">The emotion of the speaker.</param>
        [YarnCommand("SetSprite")]
        public void SetSprite(string portraitEmotion)
        {
            PortraitEmotion emotion;
            TextInfo textInfo = new CultureInfo("en-US",false).TextInfo;
            if (Enum.TryParse(textInfo.ToTitleCase(portraitEmotion), out emotion))
            {
                EventDialogueManager.Instance.SetSpeaker(speaker.GetPortrait(emotion));
            }
            else
            {
                EventDialogueManager.Instance.HideSpeaker();
                Debug.LogError($"PortraitEmotion not found for the specified: {portraitEmotion}.");
            }
        }
    }
}