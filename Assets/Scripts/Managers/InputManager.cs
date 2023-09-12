using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private Vector2 moveVector = Vector2.zero;
    private CustomInput input = null;
    public int moveFrameDelays = 5;
    private int currentMoveFrameDelay = 0;

    void Awake()
    {
        input = new CustomInput();
    }
    private void FixedUpdate() {
        if (!CanMove()){
            currentMoveFrameDelay--;
        }
        if (input.Player.ZoomIn.IsPressed()){
            GameManager.instance.ZoomCamera(-0.1f);
        }else if (input.Player.ZoomOut.IsPressed()){
            GameManager.instance.ZoomCamera(0.1f);
        }
    }
    private void OnEnable() {
        input.Enable();
        input.Player.Move.performed += OnMovePerformed;
        input.Player.Move.canceled += OnMoveCanceled;

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
    }

    private void OnDisable() {
        input.Disable();
        input.Player.Move.performed -= OnMovePerformed;
        input.Player.Move.canceled -= OnMoveCanceled;
        
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
    }
    private void OnMovePerformed(InputAction.CallbackContext value){
        GameManager.instance.SetUsingMouse(false);
        //TODO: FIX CHOPPY ANALOGUE STICK MOVEMENT !!!

        if (!CanMove()){
            return;
        }
        currentMoveFrameDelay = moveFrameDelays;
        moveVector = value.ReadValue<Vector2>();
        FixMoveVector();
        if (GameManager.instance.gameState == GameState.MainMenu){
            MainMenuManager.instance.Move(moveVector);
            return;
        }
        if (MenuManager.instance.InMenu()){
            MenuManager.instance.Move(moveVector);
            return;
        }
        GridManager.instance.MoveHoveredTile(moveVector);
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
        GameManager.instance.SetUsingMouse(false);
        GridManager.instance.SelectHoveredTile();
    }
    private void OnSelectCancled(InputAction.CallbackContext value){
        
    }
    private void OnBackPerformed(InputAction.CallbackContext value){
        if (MenuManager.instance.InMenu()){
            MenuManager.instance.CloseMenus();
            return;
        }
        GameManager.instance.SetUsingMouse(false);
        UnitManager.instance.UnselectUnit();
    }
    private void OnBackCancled(InputAction.CallbackContext value){
    
    }

    private void FixMoveVector(){
        if (Mathf.Abs(moveVector.x) > Mathf.Abs(moveVector.y)){
            moveVector.x = 1*Mathf.Sign(moveVector.x);
            moveVector.y = 0;
        }
        else if (Mathf.Abs(moveVector.x) < Mathf.Abs(moveVector.y)){
            moveVector.x = 0;
            moveVector.y = 1*Mathf.Sign(moveVector.y);
        }
    }

    private bool CanMove(){
        return currentMoveFrameDelay <= 0;
    }

    private void OnPausePerformed(InputAction.CallbackContext value){
        GameManager.instance.SetUsingMouse(false);
        MenuManager.instance.TogglePauseMenu();
    }
    private void OnPauseCanceled(InputAction.CallbackContext value){

    }

    private void OnPanCameraPerformed(InputAction.CallbackContext value){
        GameManager.instance.SetUsingMouse(false);
        var panVector = value.ReadValue<Vector2>();
        GameManager.instance.PanCameraInDirection(panVector);
    }
    private void OnPanCameraCanceled(InputAction.CallbackContext value){

    }

    
    private void OnPreviousPerformed(InputAction.CallbackContext value)
    {
        TurnManager.instance.GoToPreviousUnit();
    }
    private void OnPreviousCanceled(InputAction.CallbackContext context)
    {
        
    }
    private void OnNextPerformed(InputAction.CallbackContext value)
    {
        TurnManager.instance.GoToNextUnit();
    }
    private void OnNextCanceled(InputAction.CallbackContext context)
    {
        
    }


    private void OnUnitMenuPerformed(InputAction.CallbackContext context)
    {
        MenuManager.instance.ToggleUnitMenu();
    }
    private void OnUnitMenuCanceled(InputAction.CallbackContext context)
    {

    }
}
