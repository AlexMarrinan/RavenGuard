using System;

namespace Game.Dialogue
{
    /// <summary>
    /// A data representation which holds information for prompted Dialogue Nodes.
    /// </summary>
    [Serializable]
    public class PromptNode : DialogueNode
    {
        public DialoguePrompt prompt;

        
        public PromptNode(string nodeName) : base(nodeName)
        {
            this.nodeName = nodeName;
        }
    }
}