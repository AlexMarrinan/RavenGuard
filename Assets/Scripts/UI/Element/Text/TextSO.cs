using TMPro;
using UnityEngine;

namespace Game.UI
{
    [CreateAssetMenu(fileName = "Text", menuName = "UI/Text", order = 0)]
    public class TextSO:ScriptableObject
    {
        public TMP_FontAsset font;
        public float fontSize;
    }
}