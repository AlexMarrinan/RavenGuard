using UnityEngine;

namespace Game.UI.Element
{
    public abstract class UIElement:MonoBehaviour
    {
        public void Init()
        {
            Setup();
            Configure();
        }
        
        protected abstract void Setup();
        
        protected abstract void Configure();
    }
}