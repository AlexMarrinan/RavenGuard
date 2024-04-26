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
    public LevelupMenu levelupMenu;
    public LevelEndMenu levelEndMenu;
    private Dictionary<MenuState, BaseMenu> menuMap;
    public UnitStatsMenu unitStatsMenu, otherUnitStatsMenu;
    public ShopMenu shopMenu;
    public UnitSelectionMenu unitSelectionMenu;
    public HowToPlayMenu howToPlayMenu;
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
        menuMap = new Dictionary<MenuState, BaseMenu>
        {
            { MenuState.Pause, pauseMenu },
            { MenuState.Inventory, inventoryMenu },
            { MenuState.UnitAction, unitActionMenu },
            { MenuState.Battle, levelupMenu },
            { MenuState.LevelEnd, levelEndMenu },
            { MenuState.UnitSelection, unitSelectionMenu },
            { MenuState.Shop, shopMenu },
            {MenuState.HowToPlay, howToPlayMenu}
        };
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
//        Debug.Log(tile);

        highlightObject.transform.position = tile.transform.position;        
        highlightObject.SetActive(true);
        
        if (tile.occupiedUnit != null){
            tile.occupiedUnit.ResetCombatStats();
//            Debug.Log(UnitManager.instance.selectedUnit);
            if (UnitManager.instance.selectedUnit == null){
                unitStatsMenu.gameObject.SetActive(true);
                unitStatsMenu.transform.SetAsLastSibling();
                unitStatsMenu.SetUnit(tile.occupiedUnit);
                tile.occupiedUnit.HighlightDot();
            }else if (tile.occupiedUnit.faction == UnitFaction.Enemy && tile.moveType != TileMoveType.NotValid){
//                Debug.Log("Showing other usm");
                UnitManager.instance.selectedUnit.ResetCombatStats();
                BattlePrediction bp = new BattlePrediction(UnitManager.instance.selectedUnit, tile.occupiedUnit);
                unitStatsMenu.SetUnit(UnitManager.instance.selectedUnit);
                if (bp.attacker == UnitManager.instance.selectedUnit){
                    unitStatsMenu.healthBar.SetHealth(bp.atkHealth);
                }else{
                    unitStatsMenu.healthBar.SetHealth(bp.defHealth);
                }

                otherUnitStatsMenu.gameObject.SetActive(true);
                otherUnitStatsMenu.transform.SetAsLastSibling();
                otherUnitStatsMenu.SetUnit(tile.occupiedUnit);
                if (bp.attacker == tile.occupiedUnit){
                    otherUnitStatsMenu.healthBar.SetHealth(bp.atkHealth);
                }else{
                    otherUnitStatsMenu.healthBar.SetHealth(bp.defHealth);
                }
            }else{
                otherUnitStatsMenu.gameObject.SetActive(false);
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

    public void ShowStartText(string text, bool forever=false){
        turnStartText.transform.SetAsLastSibling();
        turnStartText.text = text;
        turnStartText.alpha = 1.0f;
        if (forever){
            textFrames = 10000;
        }else{
            textFrames = textFramesMax;
        }
    }

    public void ToggleUnitActionMenu(){
        if (UnitManager.instance.selectedUnit != null){
            //TODO: FIGURE OUT WAY TO NOT PLAY THIS SOUND DOUBLE SOMETIMES
            //AudioManager.instance.PlaySelect();
        }
        if (menuState == MenuState.UnitAction){
            CloseMenus();
            return;
        }
        if (UnitManager.instance.selectedUnit == null){
            var temp = GridManager.instance.hoveredTile.occupiedUnit;
//            Debug.Log(temp);
            if (temp != null && temp.faction == UnitFaction.Hero && TurnManager.instance.unitsAwaitingOrders.Contains(temp)){
                UnitManager.instance.SetSelectedUnit(temp);
            }else{
                return;
            }
        }
        unitActionMenu.Reset();
        unitActionMenu.gameObject.SetActive(true);
        unitActionMenu.transform.SetAsLastSibling();
        menuState = MenuState.UnitAction;
    }
    public void ToggleUnitSelectionMenu(){
        if (menuState == MenuState.UnitSelection){
            CloseMenus();
            return;
        }
        unitSelectionMenu.gameObject.SetActive(true);
        unitSelectionMenu.SetUnits();
        menuState = MenuState.UnitSelection;
//        Debug.Log("opened select menus");
    }
    public void TogglePauseMenu(){
        if (menuState == MenuState.Pause){
            CloseMenus();
            return;
        }
        if (TurnManager.instance.currentFaction == UnitFaction.Enemy){
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
        levelEndMenu.gameObject.SetActive(false);

        inventoryMenu.gameObject.SetActive(true);
        inventoryMenu.transform.SetAsLastSibling();
        menuState = MenuState.Inventory;
    }
    public void EnableInventorySwapping(){
        inventoryMenu.swapping = true;
    }
    public void DisableInventorySwapping(){
        inventoryMenu.swapping = false;
    }
    public void ToggleLevelEndMenu()
    {
        if (menuState == MenuState.LevelEnd){
            CloseMenus();
            return;
        }
        levelEndMenu.Reset();
        //if the unit action menu is shown, hide it
        unitActionMenu.gameObject.SetActive(false);
        pauseMenu.gameObject.SetActive(false);
        inventoryMenu.gameObject.SetActive(false);

        levelEndMenu.gameObject.SetActive(true);
        levelEndMenu.transform.SetAsLastSibling();
        menuState = MenuState.LevelEnd;
    }
    public void ToggleShopMenu()
    {
        if (menuState == MenuState.Shop){
            CloseMenus();
            GameManager.instance.LoadOverworldMap();
            return;
        }
        shopMenu.Reset();
        //if the unit action menu is shown, hide it
        unitActionMenu.gameObject.SetActive(false);
        pauseMenu.gameObject.SetActive(false);
        inventoryMenu.gameObject.SetActive(false);
        
        shopMenu.gameObject.SetActive(true);
        shopMenu.transform.SetAsLastSibling();
        menuState = MenuState.Shop;
    }
    public void ToggleHowToPlayMenu()
    {
        CloseMenus();
        if (menuState == MenuState.HowToPlay){
            return;
        }

        howToPlayMenu.gameObject.SetActive(true);
        howToPlayMenu.transform.SetAsLastSibling();
        menuState = MenuState.HowToPlay;
    }
    public void CloseMenus(){
        foreach (BaseMenu menu in menuMap.Values){
            menu.gameObject.SetActive(false);
        }
    
        UnitManager.instance.UnselectUnit();
        menuState = MenuState.None;
    }
    public void Move(Vector2 direction){
        AudioManager.instance.PlayMoveUI();
        if (menuState == MenuState.None){
            return;
        }
        
        if (menuMap[menuState].gameObject.activeSelf){
            menuMap[menuState].Move(direction);
        }
    }
    public bool InMenu(){
        return menuState != MenuState.None;
    }
    public void Select(){
        AudioManager.instance.PlaySelect();
        if (menuState == MenuState.None){
            return;
        }
        menuMap[menuState].Select();
    }

    public bool InPauseMenu(){
        return menuState == MenuState.Pause;
    }
    
    public void InventoryShowUntis(){
        if (inventoryMenu.currentInventoryScreen == InventoryScreen.Units){
            AudioManager.instance.PlayCancel();
            return;
        }
        inventoryMenu.ShowUnits();
    }
    public void InventoryShowItems(){
        if (inventoryMenu.currentInventoryScreen == InventoryScreen.Items){
            AudioManager.instance.PlayCancel();
            return;
        }
        inventoryMenu.ShowItems();
    }

    internal void toggleHTPMenu()
    {
        throw new NotImplementedException();
    }
}


public enum MenuState{
    None,
    Pause,
    UnitAction,
    Inventory,
    Battle,
    LevelEnd,
    UnitSelection,
    Shop,
    HowToPlay,
}