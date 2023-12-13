using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager instance;
    public MainMenu mainMenu;
    //public int textFramesBeginFadeout = 30;
    public void Awake(){
        instance = this;
    }
    public void Move(Vector2 direction){
        AudioManager.instance.PlayMoveUI();
        mainMenu.Move(direction);
    }
    public void Select(){
        AudioManager.instance.PlayConfirm();
        mainMenu.Select();
    }
}
