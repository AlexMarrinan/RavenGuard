using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Game.Hub {
    
    /// <summary>
    /// Movement for player while in the hub
    /// </summary>
    public class PlayerMovement : MonoBehaviour
    {
        private CustomInput input = null;
        private Vector2 moveVector=Vector2.zero;
        
        [SerializeField] private SpriteRenderer sprite;
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private float moveSpeed=10f;
        
        private void Awake()
        {
            input = new CustomInput();
        }

        private void OnEnable()
        {
            input.Enable();
            input.Player.Move.performed += OnMovementPerformed;
            input.Player.Move.canceled += OnMovementCanceled;
            input.Player.Save.performed += OnSavePerformed;
            input.Player.Money.performed += OnGetMoneyPerformed;
        }

        private void OnDisable()
        {
            input.Disable();
            input.Player.Move.performed -= OnMovementPerformed;
            input.Player.Move.canceled -= OnMovementCanceled;
        }

        private void FixedUpdate()
        {
            rb.velocity = moveVector*moveSpeed;
            //if no input, dont try to flip sprite;
            if (moveVector.x < 0.2 && moveVector.x > -0.2f){
                return;
            }

            //if move input, flip sprite correct directiom
            if (moveVector.x < 0){
                sprite.flipX = true;
                //Debug.Log("facing left");
            }else{
                sprite.flipX = false;
                //Debug.Log("facing right");
            }
        }

        private void OnMovementPerformed(InputAction.CallbackContext value)
        {
            moveVector = value.ReadValue<Vector2>();
        }
        
        private void OnMovementCanceled(InputAction.CallbackContext value)
        {
            moveVector=Vector2.zero;
        }

        
        private void OnSavePerformed(InputAction.CallbackContext value)
        {
            Debug.Log("forced save!");
            SaveManager.instance.SaveData();
        }

        
        private void OnGetMoneyPerformed(InputAction.CallbackContext context)
        {
            SaveManager.instance.AddCopperCoins(100);
        }
    }
}
