using System;

namespace Game.Dialogue
{
    /// <summary>
    /// A context object which holds information about a speaker.
    /// </summary>
    public struct SpeakerContext
    {
        public bool wasNotFed, wasFed;
        public bool wasNotRoomCleaned, wasRoomCleaned;
        public bool isPositiveExperience, isNegativeExperience;
        
        
        /// <summary>
        /// Determines whether the given condition is fulfilled in this speaker's context.
        /// </summary>
        /// <param name="condition">The condition the to check.</param>
        /// <returns>Return whether the data meets the necessary condition.</returns>
        /// <exception cref="ArgumentOutOfRangeException">DialogueCondition not accounted for.</exception>
        public bool CheckValidCondition(DialogueCondition condition)
        {
            return condition switch
            {
                DialogueCondition.MEAL_COMPLETED => wasFed,
                DialogueCondition.MEAL_INCOMPLETE => wasNotFed,
                DialogueCondition.ROOM_CLEANED => wasRoomCleaned,
                DialogueCondition.ROOM_DIRTY => wasNotRoomCleaned,
                DialogueCondition.POSITIVE_EXPERIENCE => isPositiveExperience,
                DialogueCondition.NEGATIVE_EXPERIENCE => isNegativeExperience,
                _ => throw new ArgumentOutOfRangeException(nameof(condition), condition, null)
            };
        }
    }
}