using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Buildings{
    public class Building : MonoBehaviour
    {
        [SerializeField] private GameObject inside;
        [SerializeField] private SpriteRenderer outside;
        private bool isInside;

        /// <summary>
        /// Player enters room
        /// </summary>
        public void EnterRoom()
        {
            print("Enter Room");
            if (!isInside)
            {
                isInside = true;
                LoadBuilding();
            }
        }
        
        /// <summary>
        /// Player leaves room
        /// </summary>
        public void ExitRoom()
        {
            print("Exit Room");
            if (isInside)
            {
                isInside = false;
                LoadBuilding();
            }
        }

        private void LoadBuilding()
        {
            outside.enabled = !isInside;
        }
    }
}