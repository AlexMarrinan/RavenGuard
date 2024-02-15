using UnityEngine;

namespace Hub.UI
{
    public abstract class View:MonoBehaviour
    {
        [Header("References")] 
        [SerializeField] private Canvas canvas;
        
        public void ToggleUI()
        {
            canvas.enabled = !canvas.enabled;
        }
        
        public void ShowUI(bool showUI)
        {
            canvas.enabled = showUI;
        }
    }
}