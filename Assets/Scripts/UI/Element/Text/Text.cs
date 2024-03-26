using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game.UI.Element;
using TMPro;

public class Text : UIElement
{
    public TextSO textData;
    public Style style;

    public TextMeshProUGUI text;

    protected override void Setup() {
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    protected override void Configure() {
        text.color = textData.theme.GetTextColor(style);
        text.font = textData.font;
        text.fontSize = textData.fontSize;

        
    }
}
