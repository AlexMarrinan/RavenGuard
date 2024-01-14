using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Hub {
    
    /// <summary>
    /// Movement for player while in the hub
    /// </summary>
    public class PlayerMovement : MonoBehaviour
    {
        private CustomInput input = null;
        private Vector2 moveVector=Vector2.zero;
        
        
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
        }
        
        private void OnDisable()
        {
            input.Disable();
            input.Player.Move.performed -= OnMovementPerformed;
            input.Player.Move.canceled += OnMovementCanceled;
        }

        private void FixedUpdate()
        {
            rb.velocity=moveVector*moveSpeed;
        }

        private void OnMovementPerformed(InputAction.CallbackContext value)
        {
            moveVector = value.ReadValue<Vector2>();
        }
        
        private void OnMovementCanceled(InputAction.CallbackContext value)
        {
            moveVector=Vector2.zero;
        }
    }
}
