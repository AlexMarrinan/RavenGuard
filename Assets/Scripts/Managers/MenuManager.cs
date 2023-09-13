using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;
    public MenuState menuState;
    [SerializeField] public GameObject highlightObject, selectedObject;
    public TextMeshProUGUI turnStartText;
    public UnitActionMenu unitActionMenu;
    public PauseMenu pauseMenu;
    private int textFrames = 0;
    //public int textFramesBeginFadeout = 30;
    public int textFramesMax = 120;
    public Color moveColor, attackColor, inRangeColor, supportColor;
    public void Awake(){
        instance = this;
    }
   private void FixedUpdate() {
        if (textFrames <= 0){
            turnStartText.alpha -= 0.05f;
            return;
        }
        textFrames--;
    }
    public void HighlightTile(Tile tile){
        if (!tile.isTileSelectable()){
            UnhighlightTile();
            return;
        }
        highlightObject.transform.position = tile.transform.position;        
        highlightObject.SetActive(true);
        
        if (UnitManager.instance.selectedUnit == null){
            GameManager.instance.LookCameraAtHighlight();
        }
    }
    public void UnhighlightTile(){
        highlightObject.SetActive(false);
    }

    public void SelectTile(Tile tile){
        if (!tile.isTileSelectable()){
            UnselectTile();
            return;
        }
        selectedObject.transform.position = tile.transform.position;
        selectedObject.SetActive(true);
    }
    public void UnselectTile(){
        selectedObject.SetActive(false);
    }

    public void ShowStartText(string text){
        turnStartText.text = text;
        turnStartText.alpha = 1.0f;
        textFrames = textFramesMax;
        turnStartText.transform.SetAsLastSibling();
    }

    public void ToggleUnitMenu(){
        if (menuState == MenuState.UnitActionMenu){
            CloseMenus();
            return;
        }
        if (UnitManager.instance.selectedUnit == null){
            return;
        }
        unitActionMenu.Reset();
        unitActionMenu.gameObject.SetActive(true);
        unitActionMenu.transform.SetAsLastSibling();
        menuState = MenuState.UnitActionMenu;
    }
    public void TogglePauseMenu(){
        if (menuState == MenuState.PauseMenu){
            CloseMenus();
            return;
        }
        pauseMenu.Reset();
        //if the unit action menu is shown, hide it
        unitActionMenu.gameObject.SetActive(false);

        pauseMenu.gameObject.SetActive(true);
        pauseMenu.transform.SetAsLastSibling();
        menuState = MenuState.PauseMenu;
    }

    public void CloseMenus(){
        unitActionMenu.gameObject.SetActive(false);
        pauseMenu.gameObject.SetActive(false);
        menuState = MenuState.None;
    }
    public void Move(Vector2 direction){
        switch (menuState){
            case MenuState.None:
                break;
            case MenuState.PauseMenu:
                pauseMenu.Move(direction);
                break;
            case MenuState.UnitActionMenu:
                unitActionMenu.Move(direction);
                break;
        }
    }
    public bool InMenu(){
        return menuState != MenuState.None;
    }
    public void Select(){
        switch (menuState){
            case MenuState.None:
                break;
            case MenuState.PauseMenu:
                pauseMenu.Select();
                break;
            case MenuState.UnitActionMenu:
                unitActionMenu.Select();
                break;
        }
    }

    public bool InPauseMenu(){
        return menuState == MenuState.PauseMenu;
    }
}


public enum MenuState{
    None,
    PauseMenu,
    UnitActionMenu
}