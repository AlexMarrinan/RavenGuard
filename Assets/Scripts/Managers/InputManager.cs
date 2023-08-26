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
            Debug.Log(currentMoveFrameDelay);
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
    }
    private void OnDisable() {
        input.Disable();
        input.Player.Move.performed -= OnMovePerformed;
        input.Player.Move.canceled -= OnMoveCanceled;
        
        input.Player.Select.performed -= OnSelectPerformed;
        input.Player.Select.canceled -= OnSelectCancled;

        input.Player.Back.performed -= OnBackPerformed;
        input.Player.Back.canceled -= OnBackCancled;
    }
    private void OnMovePerformed(InputAction.CallbackContext value){

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
        GridManager.instance.SelectHoveredTile();
    }
    private void OnSelectCancled(InputAction.CallbackContext value){
        
    }
    private void OnBackPerformed(InputAction.CallbackContext value){
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
}
