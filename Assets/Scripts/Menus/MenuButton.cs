using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour
{
    public Image image;
    public MenuButton upButton, downButton, leftButton, rightButton;
    public string buttonName; 
    public string buttonDescription;
    [HideInInspector]
    public string bonusText = "";
    public TMP_Text buttonText;
    public SpriteRenderer spriteRenderer;
    public GameObject buttonHighlight;
    private bool isOn = true;

    internal void SetHighlight(bool v)
    {
        buttonHighlight.SetActive(v);
    }

    private void Awake() {
        
    }
    public bool IsOn(){
        return isOn;
    }
    public void SetOn(bool on = true){
        isOn = on;
        buttonText.alpha = on ? 1.0f : 0.25f;
    }
    public virtual void OnPress(){
        
    }
}
