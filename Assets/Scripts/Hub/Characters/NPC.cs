using Game.Dialogue;
using UnityEngine;
using Yarn.Unity;

namespace Game.Hub.Interactables
{
    public class NPC : MonoBehaviour
    {
        public SpeakerData speakerData;
        [SerializeField] private Canvas display;

        public void StartCutscene()
        {
            string node = speakerData.GetDialogue(DialogueNodeType.Greeting);
            EventDialogueManager.Instance.StartDialogue(node);
        }
        
        [YarnCommand("OpenDisplay")]
        public void OpenDisplay() {
            display.gameObject.SetActive(true);
        }

        public void CloseDisplay()
        {
            display.gameObject.SetActive(false);
        }

        public bool IsDisplayOn() => display.gameObject.activeSelf;
    }
}