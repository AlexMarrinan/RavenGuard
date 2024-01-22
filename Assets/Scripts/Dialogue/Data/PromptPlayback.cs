using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace Game.Dialogue
{
    /// <summary>
    /// Data object for associating Dialogue Nodes from a file with specific prompts and conditions.
    /// </summary>
    [CreateAssetMenu(fileName = "Prompt Playback", menuName = "Game/Dialogue/Prompt Playback")]
    public class PromptPlayback : DialoguePlayback<PromptNode>
    {
        /// <summary>
        /// Get a node that corresponds to the given prompt
        /// </summary>
        /// <param name="prompt">The prompt of the node</param>
        /// <returns>The name of the node that corresponding prompt</returns>
        public string GetPromptNode(DialoguePrompt prompt)
        {
            List<PromptNode> nodes = dialogueNodes.Where(i=> i.prompt == prompt).ToList();
            if (nodes.Count > 0)
            {
                return GetRandomNode(nodes).nodeName;
            }
            
            Debug.LogWarning($"PromptPlayback does not have nodes for {prompt.ToString()}");
            return "";
        }
        
        #region DialoguePlayback Implementation
        /// <summary>
        /// Create a DialogueNode based on the given title and sorts it with prompts.
        /// </summary>
        /// <param name="nodeTitle">The title for the node.</param>
        protected override void CreateNode(string nodeTitle)
        {
            PromptNode promptNode = new PromptNode(nodeTitle);
            promptNode.prompt = GetPossiblePrompt(nodeTitle);
            dialogueNodes.Add(promptNode);
        }

        /// <summary>
        /// Gets a possible prompt of the node based on the nodeTitle
        /// </summary>
        /// <param name="nodeTitle">The title of the node</param>
        /// <returns>A prompt that may correspond with nodeTitle</returns>
        private DialoguePrompt GetPossiblePrompt(string nodeTitle)
        {
            string title = nodeTitle.ToLower();
            if (title.Contains("good"))
            {
                return DialoguePrompt.RECEIVE_MEAL_GOOD;
            }
            if (title.Contains("neutral"))
            {
                return DialoguePrompt.RECEIVE_MEAL_NEUTRAL;
            }
            if (title.Contains("bad"))
            {
                return DialoguePrompt.RECEIVE_MEAL_BAD;
            }
            if (title.Contains("wait"))
            {
                return DialoguePrompt.WAIT_FOR_MEAL;
            }

            return DialoguePrompt.ORDER_MEAL;
        }
        #endregion
    }
}