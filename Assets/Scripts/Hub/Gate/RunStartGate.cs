using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Game.Hub {
    
    /// <summary>
    /// Movement for player while in the hub
    /// </summary>
    public class RunStartGate : MonoBehaviour
    {
        public void StartRun(){
            Debug.Log("Starting run");
            SceneManager.LoadScene("StartScene");
        }
    }
}