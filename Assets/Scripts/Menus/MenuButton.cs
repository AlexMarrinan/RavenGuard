using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour
{
    public Image image, bgimage;
    public MenuButton upButton, downButton, leftButton, rightButton;
    public string buttonName; 
    public string buttonDescription;
    [HideInInspector]
    public string bonusText = "";
    public SpriteRenderer spriteRenderer;
    public GameObject buttonHighlight;

    internal void SetHighlight(bool v)
    {
        buttonHighlight.SetActive(v);
    }

    private void Awake() {
        
    }
}
