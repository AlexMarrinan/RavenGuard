namespace Game.Dialogue
{
    /// <summary>
    /// Conditions that determine if a dialogue node can be played.
    /// Has both positive and negative experience conditions to allow for neutral experience
    /// </summary>
    public enum DialogueCondition
    {
        MEAL_COMPLETED,         // Whether the customer was fed the previous day
        MEAL_INCOMPLETE,        // Whether the customer was not fed the previous day
        ROOM_CLEANED,           // Whether the customer's room was cleaned the previous day
        ROOM_DIRTY,             // Whether the customer's room was not cleaned the previous day
        POSITIVE_EXPERIENCE,    // Whether the customer is having an overall positive experience
        NEGATIVE_EXPERIENCE     // Whether the customer is having an overall negative experience
    }
}