using Game.Dialogue;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

namespace Game.Hub.Interactables
{
    public class NPC : MonoBehaviour
    {
        [SerializeField] public SpeakerData speakerData;
        [SerializeField] protected SpriteRenderer image;

        public virtual void StartCutscene()
        {
            string node = speakerData.GetDialogue(DialogueNodeType.Greeting);
            EventDialogueManager.Instance.StartDialogue(node);
        }
        
        [YarnCommand("Interact")]
        public virtual void Interact() {}

        public virtual bool CanInteract()
        {
            return true;
        }
    }
}