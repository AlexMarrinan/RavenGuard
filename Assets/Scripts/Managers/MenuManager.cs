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
    public InventoryMenu inventoryMenu;
    public PauseMenu pauseMenu;
    private Dictionary<MenuState, BaseMenu> menuMap;
    private int textFrames = 0;
    //public int textFramesBeginFadeout = 30;
    public int textFramesMax = 120;
    public Color moveColor, attackColor, inRangeColor, supportColor;
    public void Awake(){
        instance = this;
        InitMenuMap();
    }
    private void InitMenuMap(){
        menuMap = new Dictionary<MenuState, BaseMenu>();
        menuMap.Add(MenuState.Pause, pauseMenu);
        menuMap.Add(MenuState.Inventory, inventoryMenu);
        menuMap.Add(MenuState.UnitAction, unitActionMenu);
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
        if (menuState == MenuState.UnitAction){
            CloseMenus();
            return;
        }
        if (UnitManager.instance.selectedUnit == null){
            return;
        }
        unitActionMenu.Reset();
        unitActionMenu.gameObject.SetActive(true);
        unitActionMenu.transform.SetAsLastSibling();
        menuState = MenuState.UnitAction;
    }
    public void TogglePauseMenu(){
        if (menuState == MenuState.Pause){
            CloseMenus();
            return;
        }
        pauseMenu.Reset();
        //if the unit action menu is shown, hide it
        unitActionMenu.gameObject.SetActive(false);
        inventoryMenu.gameObject.SetActive(false);

        pauseMenu.gameObject.SetActive(true);
        pauseMenu.transform.SetAsLastSibling();
        menuState = MenuState.Pause;
    }
    public void ToggleInventoryMenu()
    {
        if (menuState == MenuState.Inventory){
            CloseMenus();
            return;
        }
        inventoryMenu.Reset();
        //if the unit action menu is shown, hide it
        unitActionMenu.gameObject.SetActive(false);
        pauseMenu.gameObject.SetActive(false);

        inventoryMenu.gameObject.SetActive(true);
        inventoryMenu.transform.SetAsLastSibling();
        menuState = MenuState.Inventory;
    }
    public void CloseMenus(){
        unitActionMenu.gameObject.SetActive(false);
        pauseMenu.gameObject.SetActive(false);
        inventoryMenu.gameObject.SetActive(false);
        menuState = MenuState.None;
    }
    public void Move(Vector2 direction){
        if (menuState == MenuState.None){
            return;
        }
        menuMap[menuState].Move(direction);
    }
    public bool InMenu(){
        return menuState != MenuState.None;
    }
    public void Select(){
        if (menuState == MenuState.None){
            return;
        }
        menuMap[menuState].Select();
    }

    public bool InPauseMenu(){
        return menuState == MenuState.Pause;
    }
}


public enum MenuState{
    None,
    Pause,
    UnitAction,
    Inventory,
}