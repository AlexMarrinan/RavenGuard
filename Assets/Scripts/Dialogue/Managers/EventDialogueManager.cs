using System.Collections.Generic;
using System.Linq;
using Game.Dialogue.Portraits;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;
using UnityEngine.Events;

namespace Game.Dialogue
{
    /// <summary>
    /// Manages the playback of event dialogue (e.g. cutscenes, interactions) for Visual Novel style format.
    /// </summary>
    public class EventDialogueManager :  MonoBehaviour, IDialogueManager
    {
        // Internal
        private Dictionary<string, float> floatVariables = new();
        private Dictionary<string, string> stringVariables = new();
        private Dictionary<string, bool> boolVariables = new();
        
        // References
        [SerializeField] private DialogueRunner dialogueRunner;
        private InMemoryVariableStorage variableStorage;
        
        [Header("Internal References")]
        [SerializeField] internal PortraitLineView portraitLineView;
        [SerializeField] private Image lineSpeakerImage;
        [SerializeField] private Image optionSpeakerImage;

        [SerializeField] private UnityEvent onConversationStarts;
        [SerializeField] private UnityEvent onConversationEnds;

        public static EventDialogueManager Instance;
        private List<SpeakerPortraitHandler> availableSpeakerPortraits;


        protected void Awake()
        {
            if (Instance == null) {
                Instance = this;
            } else {
                DestroyImmediate(gameObject);
            }
        }

        private void Start()
        {
            // Commands
            dialogueRunner.AddCommandHandler("load_variables", LoadVariables);
            
            // Callback
            dialogueRunner.onDialogueComplete.AddListener(EndDialogue);
            
            // Event Subscriptions
            portraitLineView.onNameUpdate += CheckForSpeaker;
            portraitLineView.onNameNotPresent += HideSpeaker;
        }

        #region Speaker Sprites
        /// <summary>
        /// Check to make sure the character with the given speakerName has a
        /// SpeakerPortraitHandler. If it doesn't, it won't have a portrait so
        /// hide the speaker portraits.
        /// </summary>
        /// <param name="speakerName">The name of the character speaking.</param>
        private void CheckForSpeaker(string speakerName)
        {
            foreach (SpeakerPortraitHandler speaker in availableSpeakerPortraits)
            {
                if (speaker.name == speakerName)
                {
                    return;
                }
            }
            HideSpeaker();
        }
        
        /// <summary>
        /// Hides the speaker image.
        /// </summary>
        public void HideSpeaker()
        {
            // TODO: Investigate pulling out the Speaker Image from the Line prefabs to avoid duplicates, if possible
            lineSpeakerImage.gameObject.SetActive(false);
            optionSpeakerImage.gameObject.SetActive(false);
        }

        /// <summary>
        /// Sets the portrait based on the given sprite.
        /// </summary>
        /// <param name="portrait">The sprite of the speaker.</param>
        public void SetSpeaker(Sprite portrait)
        {
            // TODO: Investigate pulling out the Speaker Image from the Line prefabs to avoid duplicates, if possible
            lineSpeakerImage.gameObject.SetActive(true);
            optionSpeakerImage.gameObject.SetActive(true);
            lineSpeakerImage.sprite = portrait;
            optionSpeakerImage.sprite = portrait;
        }
        #endregion
        
        // TODO: (OB-339) Replace later with permanent variable storage solution !!!
        #region Variables
        /// <summary>
        /// If variable doesn't exist, add it to stringVariables. Otherwise, set the variable to the given value.
        /// </summary>
        /// <param name="key">The variable name.</param>
        /// <param name="value">The value of the variable.</param>
        public void AddVariable(string key, string value)
        {
            stringVariables[key] = value;
        }
        
        /// <summary>
        /// If variable doesn't exist, add it to floatVariables. Otherwise, set the variable to the given value.
        /// </summary>
        /// <param name="key">The variable name</param>
        /// <param name="value">The value of the variable</param>
        public void AddVariable(string key, float value)
        {
            floatVariables[key] = value;
        }
        
        /// <summary>
        /// If variable doesn't exist, add it to boolVariables. Otherwise, set the variable to the given value.
        /// </summary>
        /// <param name="key">The variable name</param>
        /// <param name="value">The value of the variable</param>
        public void AddVariable(string key, bool value)
        {
            boolVariables[key] = value;
        }
        
        /// <summary>
        /// Loads the variable dictionaries into storage so YarnSpinner files can access their values.
        /// </summary>
        private void LoadVariables()
        {
            variableStorage.SetAllVariables(floatVariables,stringVariables,boolVariables);
        }
        #endregion

        #region Playback
        public void StartDialogue(string node)
        {
            // TODO: Disable player input when conversation starts
            availableSpeakerPortraits = FindObjectsOfType<SpeakerPortraitHandler>(false).ToList();
            onConversationStarts.Invoke();
            dialogueRunner.StartDialogue(node);
        }
        
        public void EndDialogue()
        {
            // TODO: Enable player input when conversation ends
            onConversationEnds.Invoke();
        }
        #endregion

        #region State
        public bool IsRunning()
        {
            return dialogueRunner.IsDialogueRunning;
        }

        public bool CanRun(string node)
        {
            return !IsRunning() && dialogueRunner.NodeExists(node);
        }
        #endregion
    }
}