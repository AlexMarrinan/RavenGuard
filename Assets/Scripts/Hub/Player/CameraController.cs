using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Hub
{
    public class CameraController : MonoBehaviour
    {
        public Transform target;

        private void Update()
        {
            transform.position = new Vector3(target.transform.position.x, target.transform.position.y,
                transform.position.z);
        }
    }
}