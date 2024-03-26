using UnityEngine;

namespace Game.UI.Element
{
    [CreateAssetMenu(fileName = "ViewSO", menuName = "CustomUI/ViewSO", order = 0)]
    public class ViewSO : ScriptableObject
    {
        public ThemeSO theme;
        public RectOffset padding;
        public float spacing;
    }
}