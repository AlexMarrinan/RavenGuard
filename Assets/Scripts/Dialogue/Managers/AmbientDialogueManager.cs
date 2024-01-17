using UnityEngine;
using Yarn.Unity;

namespace Game.Dialogue
{
    /// <summary>
    /// Manages the playback of ambient dialogue (e.g. barks) for Speech Bubble style format.
    /// </summary>
    [RequireComponent(typeof(DialogueRunner))]
    public class AmbientDialogueManager : MonoBehaviour, IDialogueManager
    {
        // Constants
        private const int MAX_AMBIENT_DIALOGUE = 5;
        
        // References
        private DialogueRunner dialogueRunner;
     
        // Internal
        private static int ambientDialogueCount = 0;
        
        private void Awake()
        {
            // References
            dialogueRunner = GetComponent<DialogueRunner>();
        }
        
        private void Start()
        {
            // Callback
            dialogueRunner.onDialogueComplete.AddListener(EndDialogue);
        }
        
        #region Playback
        public void StartDialogue(string node)
        {
            ambientDialogueCount++;
            dialogueRunner.StartDialogue(node);
        }

        public void EndDialogue()
        {
            ambientDialogueCount--;
        }
        #endregion

        #region State
        public bool IsRunning()
        {
            return dialogueRunner.IsDialogueRunning;
        }

        /// <summary>
        /// Checks to see if the given node can run, based on if it exists in the YarnProject and if there are
        /// 5 or less active conversations already happening
        /// </summary>
        /// <param name="node">The name of the given node</param>
        /// <returns>Returns true if the node exists and if there are 5 or less active conversations already happening</returns>
        public bool CanRun(string node)
        {
            return dialogueRunner.NodeExists(node) && ambientDialogueCount < MAX_AMBIENT_DIALOGUE;
        }
        #endregion
    }
}

