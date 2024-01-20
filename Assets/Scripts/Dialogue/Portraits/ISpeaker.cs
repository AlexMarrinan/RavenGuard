using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

namespace Game.Dialogue.Portraits
{
    /// <summary>
    /// Enum allows characters to have multiple portraits
    /// </summary>
    public enum PortraitType
    {
        Default
    }
    
    /// <summary>
    /// Characters speaking in visual novel format
    /// </summary>
    public interface ISpeaker
    {
        /// <summary>
        /// Returns the name of this speaker.
        /// </summary>
        /// <returns>The name of this speaker.</returns>
        public string GetName();
        
        /// <summary>
        /// Returns the sprite that corresponds with the given PortraitEmotion
        /// </summary>
        /// <param name="portraitEmotion">The emotion displayed in the portrait</param>
        /// <returns>If the given portraitEmotion is a key in characterPortraits, return value.
        /// Otherwise, return null</returns>
        public Sprite GetPortrait(PortraitType portraitEmotion = PortraitType.Default);
    }
}

