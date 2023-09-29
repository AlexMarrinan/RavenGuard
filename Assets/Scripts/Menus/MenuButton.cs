using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour
{
    public Image image;
    public string buttonName; 
    public string buttonDescription;
    [HideInInspector]
    public string bonusText = "";
    public SpriteRenderer spriteRenderer;
    private void Awake() {
        
    }
}
