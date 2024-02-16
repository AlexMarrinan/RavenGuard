using UnityEngine;

namespace Hub.UI
{
    public abstract class View:MonoBehaviour
    {
        [Header("References")] 
        [SerializeField] private Canvas canvas;

        private bool isShowing;
        
        public void ToggleUI()
        {
            ShowUI(!isShowing);
            isShowing = !isShowing;
        }
        
        public virtual void ShowUI(bool showUI)
        {
            canvas.enabled = showUI;
        }
    }
}