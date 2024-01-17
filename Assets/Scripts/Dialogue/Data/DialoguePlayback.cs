using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Dialogue
{
    /// <summary>
    /// Data object for associating Dialogue Nodes from a file with playback options and conditions.
    /// Parses an associated YarnSpinner file for its nodes, storing each node in a specific class.
    /// </summary>
    /// <typeparam name="T">The class for storing Dialogue Node and its associated data.</typeparam>
    public abstract class DialoguePlayback<T> : ScriptableObject where T : DialogueNode
    {
        // Inspector
        [SerializeField] protected List<T> dialogueNodes = new();
        public TextAsset yarnFile;
        

        #region Parsing & Node Creation
        /// <summary>
        /// Parses a Yarn file for information about their nodes.
        /// </summary>
        /// <param name="path">File path.</param>
        public void ParseFile(string path)
        {
            StreamReader reader = File.OpenText(path);
            string line;
            List<string> nodeTitles = new List<string>();
            while ((line = reader.ReadLine()) != null) 
            {
                string[] items = line.Split('\t');
                
                for (int i = 0; i < items.Length; i++)
                {
                    if (items[i].StartsWith("title: "))
                    {
                        string[] words = items[i].Split(' ');
                        nodeTitles.Add(words[1]);
                    }
                }
            }

            CreateNewNodes(nodeTitles);
            CheckNodes(nodeTitles);
        }

        /// <summary>
        /// Create new nodes for the playback object that don't already exist. 
        /// </summary>
        /// <param name="nodeTitles">Titles of nodes that should exist.</param>
        private void CreateNewNodes(List<string> nodeTitles)
        {
            foreach (string title in nodeTitles)
            {
                bool exists = dialogueNodes.Exists(node => node.nodeName == title);
                if (!exists) CreateNode(title);
            }
        }
        
        /// <summary>
        /// Create a DialogueNode based on the given title.
        /// </summary>
        /// <param name="nodeTitle">The title for the node.</param>
        protected abstract void CreateNode(string nodeTitle);
        
        /// <summary>
        /// Check that all nodes in the playback object exist in the given list of nodes.
        /// </summary>
        /// <param name="nodeTitles">Titles of nodes to check against.</param>
        private void CheckNodes(List<string> nodeTitles)
        {
            foreach (T node in dialogueNodes)
            {
                if (!nodeTitles.Contains(node.nodeName))
                {
                    Debug.LogWarning($"{node.nodeName} does not exist in {yarnFile.name}.");
                }
            }
        }
        #endregion
        
        #region Randomization
        /// <summary>
        /// Get a randomly weighted node from the provided list.
        /// </summary>
        /// <param name="nodes">The nodes to select from.</param>
        /// <returns>A node selected through random weights.</returns>
        protected T GetRandomNode(List<T> nodes)
        {
            int totalWeight = 0;
            nodes.ForEach(node => totalWeight += node.weight);
            
            if (totalWeight == 0)
            {
                return nodes[Random.Range(0, nodes.Count)];
            }
            else
            {
                int randomNumber = Random.Range(0, totalWeight);
            
                foreach (T node in nodes)
                {
                    if (randomNumber < node.weight)
                    {
                        return node;
                    }

                    randomNumber -= node.weight;
                }
            }

            // NOTE: Should never reach this point.
            return null;
        }
        #endregion
    }
}
