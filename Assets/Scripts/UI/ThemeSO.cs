using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
    [CreateAssetMenu(fileName = "Theme", menuName = "UI/Theme", order = 0)]
    public class ThemeSO : ScriptableObject
    {
        [Header("Primary")] public Color primaryBackgroundColor;
        public Color primaryTextColor;

        [Header("Secondary")] public Color secondaryBackgroundColor;
        public Color secondaryTextColor;

        [Header("Tertiary")] public Color tertiaryBackgroundColor;
        public Color tertiaryTextColor;

        [Header("Other")] public Color disable;

        public Color GetBackgroundColor(Style style) => style switch{
            Style.Primary => primaryBackgroundColor,
            Style.Secondary => secondaryBackgroundColor,
            Style.Tertiary => tertiaryBackgroundColor,
            _ => disable
        };
        
        public Color GetTextColor(Style style) => style switch{
            Style.Primary => primaryTextColor,
            Style.Secondary => secondaryTextColor,
            Style.Tertiary => tertiaryTextColor,
            _ => disable
        };
    }
}