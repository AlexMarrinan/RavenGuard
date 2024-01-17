using AYellowpaper.SerializedCollections;
using Game.Dialogue.Portraits;
using UnityEngine;

namespace Game.Dialogue
{
    /// <summary>
    /// Data for a character in the game that speaks using the dialogue system.
    /// </summary>
    [CreateAssetMenu(fileName = "New Speaker Data", menuName = "Game/Dialogue/Speaker Data")]
    public class SpeakerData : ScriptableObject, ISpeaker
    {
        // Inspector
        [Header("Speaker")]
        [Tooltip("The character's name.")]
        public string characterName;
        
        [Tooltip("The portraits of the character.")]
        [SerializeField] private SerializedDictionary<PortraitEmotion, Sprite> characterPortraits;


        #region ISpeaker Implementation
        public string GetName()
        {
            return characterName;
        }

        public Sprite GetPortrait(PortraitEmotion portraitEmotion)
        {
            if (characterPortraits.ContainsKey(portraitEmotion))
            {
                return characterPortraits[portraitEmotion];
            }

            // Attempt to use neutral sprite if no sprite is found
            Sprite defaultSprite;
            characterPortraits.TryGetValue(PortraitEmotion.Neutral, out defaultSprite);
            return defaultSprite;
        }
        #endregion
    }
}