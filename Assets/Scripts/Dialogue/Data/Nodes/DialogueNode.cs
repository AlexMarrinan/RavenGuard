using System;

namespace Game.Dialogue
{
    /// <summary>
    /// A data representation of a YarnSpinner dialogue node and its related information.
    /// </summary>
    [Serializable]
    public class DialogueNode
    {
        public string nodeName;
        public int weight;

        
        public DialogueNode(string nodeName)
        {
            this.nodeName = nodeName;
            weight = 0;
        }
    }
}