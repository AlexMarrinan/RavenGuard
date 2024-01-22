namespace Game.Dialogue
{
    /// <summary>
    /// Interface for shared dialogue manager behavior.
    /// </summary>
    public interface IDialogueManager
    {
        #region Playback
        /// <summary>
        /// Start a dialogue beginning at a given node.
        /// </summary>
        /// <param name="node">The node to start from.</param>
        public void StartDialogue(string node);

        /// <summary>
        /// Run at the end of a dialogue.
        /// </summary>
        public void EndDialogue();
        #endregion
        
        #region State
        /// <summary>
        /// Checks whether this dialogue manager is running.
        /// </summary>
        /// <returns>Whether this dialogue manager is running.</returns>
        public bool IsRunning();

        /// <summary>
        /// Checks whether this dialogue manager can run the given node.
        /// </summary>
        /// <param name="node">Node's string name</param>
        /// <returns>Whether this dialogue manager can run.</returns>
        public bool CanRun(string node);
        #endregion
    }
}