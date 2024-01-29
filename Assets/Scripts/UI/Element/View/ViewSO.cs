using UnityEngine;

namespace Game.UI
{
    [CreateAssetMenu(fileName = "View", menuName = "UI/View", order = 0)]
    public class ViewSO:ScriptableObject
    {
        public ThemeSO themeSo;
        public RectOffset padding;
        public float spacing;
    }
}