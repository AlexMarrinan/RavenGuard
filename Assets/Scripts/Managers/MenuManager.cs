using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
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
    public BattleMenu battleMenu;
    private Dictionary<MenuState, BaseMenu> menuMap;
    public UnitStatsMenu unitStatsMenu, otherUnitStatsMenu;
    private int textFrames = 0;
    //public int textFramesBeginFadeout = 30;
    public int textFramesMax = 120;
    public Color moveColor, attackColor, inRangeColor, supportColor;
    public void Awake(){
        instance = this;
        InitMenuMap();
        turnStartText.transform.SetAsLastSibling();
    }
    private void InitMenuMap(){
        menuMap = new Dictionary<MenuState, BaseMenu>();
        menuMap.Add(MenuState.Pause, pauseMenu);
        menuMap.Add(MenuState.Inventory, inventoryMenu);
        menuMap.Add(MenuState.UnitAction, unitActionMenu);
        menuMap.Add(MenuState.Battle, battleMenu);
    }
    private void FixedUpdate() {
        if (textFrames <= 0){
            turnStartText.alpha -= 0.05f;
            return;
        }
        textFrames--;
    }
    public void HighlightTile(BaseTile tile){
        if (!tile.IsTileSelectable()){
            UnhighlightTile();
            return;
        }
        highlightObject.transform.position = tile.transform.position;        
        highlightObject.SetActive(true);
        
        if (tile.occupiedUnit != null){
            tile.occupiedUnit.ResetCombatStats();
            if (UnitManager.instance.selectedUnit == null){
                unitStatsMenu.gameObject.SetActive(true);
                unitStatsMenu.SetUnit(tile.occupiedUnit);
            }else if (tile.occupiedUnit.faction == UnitFaction.Enemy){
                UnitManager.instance.selectedUnit.ResetCombatStats();
                BattlePrediction bp = new BattlePrediction(UnitManager.instance.selectedUnit, tile.occupiedUnit);
                unitStatsMenu.SetUnit(UnitManager.instance.selectedUnit);
                if (bp.attacker == UnitManager.instance.selectedUnit){
                    unitStatsMenu.healthBar.SetHealth(bp.atkHealth);
                }else{
                    unitStatsMenu.healthBar.SetHealth(bp.defHealth);
                }

                otherUnitStatsMenu.gameObject.SetActive(true);
                otherUnitStatsMenu.SetUnit(tile.occupiedUnit);
                if (bp.attacker == tile.occupiedUnit){
                    otherUnitStatsMenu.healthBar.SetHealth(bp.atkHealth);
                }else{
                    otherUnitStatsMenu.healthBar.SetHealth(bp.defHealth);
                }
            }
        }else{
            if (UnitManager.instance.selectedUnit == null){
                unitStatsMenu.gameObject.SetActive(false);
            }else{
                UnitManager.instance.selectedUnit.ResetCombatStats();
                unitStatsMenu.SetUnit(UnitManager.instance.selectedUnit);
            }
            otherUnitStatsMenu.gameObject.SetActive(false);
        }

        if (UnitManager.instance.selectedUnit == null){
            GameManager.instance.LookCameraAtHighlight();
        }
    }
    public void UnhighlightTile(){
        highlightObject.SetActive(false);
    }

    public void SelectTile(BaseTile tile){
        if (!tile.IsTileSelectable()){
            UnselectTile();
            return;
        }
        selectedObject.transform.position = tile.transform.position;
        selectedObject.SetActive(true);
    }
    public void UnselectTile(){
        selectedObject.SetActive(false);
    }

    public void ShowStartText(string text, bool forever){
        turnStartText.text = text;
        turnStartText.alpha = 1.0f;
        if (forever){
            textFrames = 10000;
        }else{
            textFrames = textFramesMax;
        }
    }

    public void ToggleUnitActionMenu(){
        if (menuState == MenuState.UnitAction){
            CloseMenus();
            return;
        }
        if (UnitManager.instance.selectedUnit == null){
            var temp = GridManager.instance.hoveredTile.occupiedUnit;
            if (temp != null){
                UnitManager.instance.SetSeclectedUnit(temp);
            }else{
                return;
            }
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
        UnitManager.instance.UnselectUnit();
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
    Battle,
}