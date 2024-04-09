using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{
    private Vector2 moveVector = Vector2.zero;
    private CustomInput input = null;
    public int moveFrameDelays = 5;
    private int currentMoveFrameDelay = 0;
    public bool enableMouse = true;
    public static InputManager instance;
    void Awake()
    {
        instance = this;        
        input = new CustomInput();
    }
    private void FixedUpdate() {
        if ((input.Player.Move.IsPressed() || 
            (input.Player.MoveMenu.IsPressed() && MenuManager.instance.InMenu())) 
            && CanMove()){
            Move();
        }
        currentMoveFrameDelay--;
        if (input.Player.ZoomIn.IsPressed()){
            return;
            GameManager.instance.ZoomCamera(-0.1f);
        }else if (input.Player.ZoomOut.IsPressed()){
            return;
            GameManager.instance.ZoomCamera(0.1f);
        }
    }
    private void Move(){

        if (TurnManager.instance.currentFaction == UnitFaction.Enemy){
            return;
        }
        currentMoveFrameDelay = moveFrameDelays;
        if (GameManager.instance.gameState == GameState.MainMenu){
            MainMenuManager.instance.Move(moveVector);
            return;
        }
        if (SkillManager.instance.selectingSkill){
            //Debug.Log("moving skill");
            SkillManager.instance.ShowSkillPreview();
        }
        if (MenuManager.instance.InMenu()){
            //Debug.Log("moving menu");
            MenuManager.instance.Move(moveVector);
            return;
        }
        if (OverworldMapManager.instance.InOverworldMap()){
            OverworldMapManager.instance.Move(moveVector);
            return;
        }
        GridManager.instance.MoveHoveredTile(moveVector);
    }
    private void OnEnable() {
        input.Enable();
        input.Player.Move.performed += OnMovePerformed;
        input.Player.Move.canceled += OnMoveCanceled;

        input.Player.MoveMenu.performed += OnMoveMenuPerformed;
        input.Player.MoveMenu.canceled += OnMoveMenuCanceled;
        
        input.Player.Select.performed += OnSelectPerformed;
        input.Player.Select.canceled += OnSelectCancled;

        input.Player.Back.performed += OnBackPerformed;
        input.Player.Back.canceled += OnBackCancled;

        input.Player.Pause.performed += OnPausePerformed;
        input.Player.Pause.canceled += OnPauseCanceled;

        input.Player.PanCamera.performed += OnPanCameraPerformed;
        input.Player.PanCamera.canceled += OnPanCameraCanceled;
        
        input.Player.Previous.performed += OnPreviousPerformed;
        input.Player.Previous.canceled += OnPreviousCanceled;

        input.Player.Next.performed += OnNextPerformed;
        input.Player.Next.canceled += OnNextCanceled;

        input.Player.UnitMenu.performed += OnUnitMenuPerformed;
        input.Player.UnitMenu.canceled += OnUnitMenuCanceled;

        input.Player.InventoryMenu.performed += OnInventoryMenuPerformed;
        input.Player.InventoryMenu.canceled += OnInventoryMenuCanceled;

        input.Player.SkipTurn.performed += OnSkipTurnPerformed;
        input.Player.SkipTurn.canceled += OnSkipTurnCanceled;
    }
    private void OnMoveMenuPerformed(InputAction.CallbackContext context)
    {
        if (MenuManager.instance.unitStatsMenu.gameObject.activeSelf
         && !MenuManager.instance.InMenu()){
            if (MenuManager.instance.otherUnitStatsMenu.gameObject.activeSelf){
                return;
            }
            Debug.Log("Moving menu...");
            MenuManager.instance.unitStatsMenu.Move(context.ReadValue<Vector2>());
        }
        else { //if (MenuManager.instance.InMenu()){
            moveVector = context.ReadValue<Vector2>();
            FixMoveVector();
        }
    }
    private void OnMoveMenuCanceled(InputAction.CallbackContext context)
    {
        
    }



    private void OnDisable() {
        input.Disable();
        input.Player.Move.performed -= OnMovePerformed;
        input.Player.Move.canceled -= OnMoveCanceled;
        
        input.Player.MoveMenu.performed -= OnMoveMenuPerformed;
        input.Player.MoveMenu.canceled -= OnMoveMenuCanceled;

        input.Player.Select.performed -= OnSelectPerformed;
        input.Player.Select.canceled -= OnSelectCancled;

        input.Player.Back.performed -= OnBackPerformed;
        input.Player.Back.canceled -= OnBackCancled;

        input.Player.Pause.performed -= OnPausePerformed;
        input.Player.Pause.canceled -= OnPauseCanceled;

        input.Player.PanCamera.performed -= OnPanCameraPerformed;
        input.Player.PanCamera.canceled -= OnPanCameraCanceled;

        input.Player.Previous.performed -= OnPreviousPerformed;
        input.Player.Previous.canceled -= OnPreviousCanceled;

        input.Player.Next.performed -= OnNextPerformed;
        input.Player.Next.canceled -= OnNextCanceled;
        
        input.Player.UnitMenu.performed -= OnUnitMenuPerformed;
        input.Player.UnitMenu.canceled -= OnUnitMenuCanceled;

        input.Player.InventoryMenu.performed -= OnInventoryMenuPerformed;
        input.Player.InventoryMenu.canceled -= OnInventoryMenuCanceled;

        input.Player.SkipTurn.performed -= OnSkipTurnPerformed;
        input.Player.SkipTurn.canceled -= OnSkipTurnCanceled;
    }

    private void OnSkipTurnCanceled(InputAction.CallbackContext context)
    {
        TurnManager.instance.SkipTurn();
    }

    private void OnSkipTurnPerformed(InputAction.CallbackContext context)
    {

    }

    private void OnMovePerformed(InputAction.CallbackContext value){
        GameManager.instance.SetUsingMouse(false);
        //TODO: FIX CHOPPY ANALOGUE STICK MOVEMENT !!!
        moveVector = value.ReadValue<Vector2>();
        FixMoveVector();
    }
    private void OnMoveCanceled(InputAction.CallbackContext value){
        moveVector = Vector2.zero;
    }

    private void OnSelectPerformed(InputAction.CallbackContext value){
        if (GameManager.instance.gameState == GameState.MainMenu){
            MainMenuManager.instance.Select();
            return;
        }
        if (MenuManager.instance.InMenu()){
            MenuManager.instance.Select();
            return;
        }
        if (OverworldMapManager.instance.InOverworldMap()){
            OverworldMapManager.instance.Select();
            return;
        }
        if (GridManager.instance.hoveredTile.occupiedUnit != null 
        && GridManager.instance.hoveredTile.occupiedUnit.hasMoved 
        && MenuManager.instance.menuState != MenuState.UnitAction
        && !SkillManager.instance.selectingSkill){
            MenuManager.instance.ToggleUnitActionMenu();
            return;
        }
        if (SkillManager.instance.selectingSkill){
            SkillManager.instance.Select();
            return;
        }
        GameManager.instance.SetUsingMouse(false);
        GridManager.instance.SelectHoveredTile();
    }
    private void OnSelectCancled(InputAction.CallbackContext value){
        
    }
    private void OnBackPerformed(InputAction.CallbackContext value){
        if (MenuManager.instance.menuState == MenuState.UnitSelection){
            MenuManager.instance.unitSelectionMenu.UnselectUnit();
            return;
        }
        if (MenuManager.instance.menuState == MenuState.Battle){
            return;
        }
        if (MenuManager.instance.menuState == MenuState.LevelEnd){
            return;
        }
        AudioManager.instance.PlayCancel();
        if (SkillManager.instance.selectingSkill){
            SkillManager.instance.OnSkilEnd();
            return;
        }
        if (MenuManager.instance.InMenu()){
            if (MenuManager.instance.menuState == MenuState.Inventory ) {
                if (MenuManager.instance.inventoryMenu.hoveredItem != null){
                    MenuManager.instance.inventoryMenu.UnhoverItem();
                    return;
                }
                if (MenuManager.instance.inventoryMenu.currentInventoryScreen == InventoryScreen.Items){
                    MenuManager.instance.inventoryMenu.ChangeInventoryScreen();
                    return;
                }
            }
            MenuManager.instance.CloseMenus();
            return;
        }
        GameManager.instance.SetUsingMouse(false);
        UnitManager.instance.UnselectUnit();
    }
    private void OnBackCancled(InputAction.CallbackContext value){
        
    }
    private void FixMoveVector(){
        if (moveVector.x > 0.4){
            moveVector.x = 1;
        }
        else if (moveVector.x < -0.4){
            moveVector.x = -1;
        }else {
            moveVector.x = 0;
        }
        if (moveVector.y > 0.4){
            moveVector.y = 1;
        }
        else if (moveVector.y < -0.4){
            moveVector.y = -1;
        }else {
            moveVector.y = 0;
        }
    }

    private bool CanMove(){
        return currentMoveFrameDelay < 0;
    }

    private void OnPausePerformed(InputAction.CallbackContext value){
        if (MenuManager.instance.menuState == MenuState.UnitSelection){
            MenuManager.instance.unitSelectionMenu.ConfirmUnits();
            return;
        }
        if (MenuManager.instance.menuState == MenuState.Inventory && MenuManager.instance.inventoryMenu.swapping){
            MenuManager.instance.DisableInventorySwapping();
            MenuManager.instance.CloseMenus();
            GameManager.instance.LoadOverworldMap();
            return;
        }
        if (MenuManager.instance.menuState == MenuState.LevelEnd){
            return;
        }
        if (GameManager.instance.gameState == GameState.BattleScene){
            SceneManager.LoadScene("MainMenu");
            return;
        }
        GameManager.instance.SetUsingMouse(false);
        MenuManager.instance.TogglePauseMenu();
    }
    private void OnPauseCanceled(InputAction.CallbackContext value){

    }

    private void OnPanCameraPerformed(InputAction.CallbackContext value){
        return;
        GameManager.instance.SetUsingMouse(false);
        var panVector = value.ReadValue<Vector2>();
        GameManager.instance.PanCameraInDirection(panVector);
    }
    private void OnPanCameraCanceled(InputAction.CallbackContext value){

    }

    
    private void OnPreviousPerformed(InputAction.CallbackContext value)
    {
        var mm = MenuManager.instance;
        if (mm.menuState == MenuState.None){
            TurnManager.instance.GoToPreviousUnit();
        }
        if (mm.menuState == MenuState.Inventory){
            mm.inventoryMenu.UnhoverItem();
            mm.InventoryShowUntis();
        }    
    }
    private void OnPreviousCanceled(InputAction.CallbackContext context)
    {
        
    }
    private void OnNextPerformed(InputAction.CallbackContext value)
    {   
        var mm = MenuManager.instance;
        if (mm.menuState == MenuState.None){
            TurnManager.instance.GoToNextUnit();
        }
        if (mm.menuState == MenuState.Inventory){
            mm.inventoryMenu.UnhoverItem();
            mm.InventoryShowItems();
        }
    }
    private void OnNextCanceled(InputAction.CallbackContext context)
    {
        
    }


    private void OnUnitMenuPerformed(InputAction.CallbackContext context)
    {
        if (MenuManager.instance.menuState == MenuState.Inventory){
            MenuManager.instance.inventoryMenu.UnequipItem();
            return;
        }
        if (MenuManager.instance.InMenu()){
            return;
        }
        MenuManager.instance.ToggleUnitActionMenu();
    }
    private void OnUnitMenuCanceled(InputAction.CallbackContext context)
    {

    }
    private void OnInventoryMenuPerformed(InputAction.CallbackContext context)
    {
        if (MenuManager.instance.inventoryMenu.swapping){
            //DSIABLED AT END OF LEVEL, CAN ONLY TOGGLE DURING NORMAL GAMEPLAY
            return;
        }
        if (MenuManager.instance.menuState == MenuState.Battle){
            return;
        }
        if (MenuManager.instance.menuState == MenuState.LevelEnd){
            return;
        }
        MenuManager.instance.ToggleInventoryMenu();
    }

    private void OnInventoryMenuCanceled(InputAction.CallbackContext context)
    {
        
    }
}
