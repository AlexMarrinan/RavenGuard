using TMPro;
using UnityEngine;

namespace Game.UI.Element
{
    [CreateAssetMenu(fileName = "Text", menuName = "CustomUI/TextSO", order = 0)]
    public class TextSO : ScriptableObject
    {
        public ThemeSO theme;
        public TMP_FontAsset font;
        public float fontSize;
    }
}