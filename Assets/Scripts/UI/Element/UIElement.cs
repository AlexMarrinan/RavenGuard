using UnityEngine;

namespace Game.UI.Element
{
    public abstract class UIElement : MonoBehaviour
    {
        private void Awake() {
            Init();
        }
        
        public void Init() {
            Setup();
            Configure();
        }

        private void OnValidate() {
            Init();
        }
        
        protected abstract void Setup();
        
        protected abstract void Configure();
    }
}