using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AYellowpaper.SerializedCollections;
using Game.Dialogue.Portraits;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using Random = System.Random;

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
        [SerializeField] private SerializedDictionary<PortraitType, Sprite> characterPortraits;

        [Header("Dialogue Nodes")]
        [SerializeField] private List<TextAsset> yarnScripts;
        [SerializeField] private SerializedDictionary<DialogueNodeType, List<string>> availableNodes;

        #region Nodes

        /// <summary>
        /// Gets a node name based on the given type.
        /// </summary>
        /// <param name="type">The type of dialogue node needed.</param>
        /// <returns>"" if no node in type exists, otherwise return random node corresponding to type.</returns>
        public string GetDialogue(DialogueNodeType type)
        {
            if (availableNodes.ContainsKey(type))
            {
                Random r = new Random();
                int index = r.Next(0, availableNodes[type].Count);
                return availableNodes[type][index];
            }

            return "";
        }

        #if UNITY_EDITOR
        /// <summary>
        /// Parses all the yarn scripts for yarn nodes
        /// </summary>
        public void ParseYarnScripts()
        {
            foreach (TextAsset file in yarnScripts)
            {

                string path = AssetDatabase.GetAssetPath(file);
                
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
                CleanAvailableNodes(nodeTitles);
            }
        }
        #endif

        /// <summary>
        /// Create new nodes for the playback object that don't already exist. 
        /// </summary>
        /// <param name="nodeTitles">Titles of nodes that should exist.</param>
        private void CreateNewNodes(List<string> nodeTitles)
        {
            foreach (string title in nodeTitles)
            {
                DialogueNodeType nodeType = GetNodeType(title);
                bool exists = availableNodes.ContainsKey(nodeType);
                if (!exists)
                {
                    availableNodes.Add(nodeType,new List<string>{title});
                }else if (!availableNodes[nodeType].Contains(title))
                {
                    availableNodes[nodeType].Add(title);
                }
            }
        }

        /// <summary>
        /// Gets the dialogue node type based on the nodeTitle
        /// </summary>
        /// <param name="nodeTitle">Title of the yarn node</param>
        /// <returns>Returns with unsorted if unable to match nodeTitle to another DialogueNodeType</returns>
        private DialogueNodeType GetNodeType(string nodeTitle)
        {
            string title = nodeTitle.ToLower();
            
            foreach (DialogueNodeType type in Enum.GetValues(typeof(DialogueNodeType)))
            {
                string nodeType = type.ToString().ToLower();
                if (title.Contains(nodeType))
                {
                    return type;
                }
            }
            Debug.LogError($"No DialogueNodeType for {nodeTitle}");

            return DialogueNodeType.Unsorted;
        }
        
        /// <summary>
        /// Check that all nodes in the availableNodes exist in the given list of nodes.
        /// Remove nodes that don't exist and nodeTypes with no nodes
        /// </summary>
        /// <param name="nodeTitles">Titles of nodes to check against.</param>
        private void CleanAvailableNodes(List<string> nodeTitles)
        {
            List<Node> removeNodes = new List<Node>();
            foreach (DialogueNodeType nodeType in availableNodes.Keys)
            {
                foreach (string node in availableNodes[nodeType])
                {
                    if (!nodeTitles.Contains(node))
                    {
                        Debug.LogWarning($"{node} does not exist in yarn files.");
                        removeNodes.Add(new Node(nodeType,node));
                    }
                }
            }

            foreach (Node node in removeNodes)
            {
                availableNodes[node.nodeType].Remove(node.title);
                if (availableNodes[node.nodeType].Count == 0)
                {
                    availableNodes.Remove(node.nodeType);
                }
            }
        }

        #endregion

        #region ISpeaker Implementation
        public string GetName()
        {
            return characterName;
        }

        public Sprite GetPortrait(PortraitType portraitType)
        {
            if (characterPortraits.ContainsKey(portraitType))
            {
                return characterPortraits[portraitType];
            }

            // Attempt to use neutral sprite if no sprite is found
            Sprite defaultSprite;
            characterPortraits.TryGetValue(PortraitType.Default, out defaultSprite);
            return defaultSprite;
        }
        #endregion
    }
    
    public class Node
    {
        public DialogueNodeType nodeType;
        public string title;

        public Node(DialogueNodeType nodeType, string title)
        {
            this.nodeType = nodeType;
            this.title = title;
        }
    }
}