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

        input.Player.ZoomIn.performed += OnZoomInPerformed;
        input.Player.ZoomIn.canceled += OnZoomInCanceled;

        input.Player.ZoomOut.performed += OnZoomOutPerformed;
        input.Player.ZoomOut.canceled += OnZoomOutCanceled;
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
        GridManager.instance.MoveHoveredTile(moveVector);
    }
    private void OnMoveCanceled(InputAction.CallbackContext value){
        moveVector = Vector2.zero;
    }

    private void OnSelectPerformed(InputAction.CallbackContext value){
        GameManager.instance.SetUsingMouse(false);
        GridManager.instance.SelectHoveredTile();
    }
    private void OnSelectCancled(InputAction.CallbackContext value){
        
    }
    private void OnBackPerformed(InputAction.CallbackContext value){
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
        if (GameManager.instance.gameState == GameState.EnemiesTurn){
            GameManager.instance.ChangeState(GameState.HeroesTurn);
        }else{
            GameManager.instance.ChangeState(GameState.EnemiesTurn);
        }
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


    
    private void OnZoomOutCanceled(InputAction.CallbackContext context)
    {

    }

    private void OnZoomOutPerformed(InputAction.CallbackContext context)
    {
        GameManager.instance.ZoomCamera(+1);
    }

    private void OnZoomInCanceled(InputAction.CallbackContext context)
    {

    }

    private void OnZoomInPerformed(InputAction.CallbackContext context)
    {
        GameManager.instance.ZoomCamera(-1);
    }
}
