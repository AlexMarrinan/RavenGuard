using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UI.Element.View;

public class PauseMenuUI : RowView
{

    public GameObject resumeButton;
    public GameObject restartButton;
    public GameObject quitButton;

    private Image resumeImage;
    private Image restartImage;
    private Image quitImage;

    protected override void Setup() {
        base.Setup();
        resumeImage = resumeButton.GetComponent<Image>();
        restartImage = restartButton.GetComponent<Image>();
        quitImage = quitButton.GetComponent<Image>();
    }

    protected override void Configure() {
        base.Configure();
        resumeImage.color = viewData.theme.primaryBackgroundColor;
        restartImage.color = viewData.theme.secondaryBackgroundColor;
        quitImage.color = viewData.theme.tertiaryBackgroundColor;
    }
}
