using Game.Dialogue;
using Game.Hub.Interactables;

namespace Hub.Characters
{
    public class TemporaryDialogueTrigger : Interactable
    {
        public string node="Blacksmith_Greeting_1";

        protected override void Interaction()
        {
            EventDialogueManager.Instance.StartDialogue(node);
        }

        public override void EndInteraction()
        {
            throw new System.NotImplementedException();
        }

        protected override bool CanUseInteraction()
        {
            return true;
        }
    }
}
