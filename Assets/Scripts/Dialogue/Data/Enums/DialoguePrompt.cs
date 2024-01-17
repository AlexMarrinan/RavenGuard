namespace Game.Dialogue
{
    /// <summary>
    /// Enum for the different prompts that can be given to an NPC.
    /// </summary>
    public enum DialoguePrompt
    {
        ORDER_MEAL,             // Customer orders a meal
        WAIT_FOR_MEAL,          // Customer is waiting for a meal
        RECEIVE_MEAL_BAD,       // Customer receives a bad meal
        RECEIVE_MEAL_NEUTRAL,   // Customer receives a neutral meal
        RECEIVE_MEAL_GOOD       // Customer receives a good meal
    }
}