using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Buildings{
    public class Building : MonoBehaviour
    {
        [SerializeField] private GameObject interior;
        [SerializeField] private List<SpriteRenderer> exteriorRenderers;
        [SerializeField] private float transitionTime;
        private bool isInside;

        /// <summary>
        /// Toggle whether the player is in the building or not
        /// </summary>
        public void ToggleBuilding()
        {
            print("Toggle");
            if(!isInside) EnterBuilding();
            else ExitBuilding();
            isInside = !isInside;
        }
        
        /// <summary>
        /// Player enters building
        /// </summary>
        public void EnterBuilding()
        {
            if (!isInside)
            {
                isInside = true;
                TweenExterior();
            }
        }
        
        /// <summary>
        /// Player leaves building
        /// </summary>
        public void ExitBuilding()
        {
            if (isInside)
            {
                isInside = false;
                TweenExterior();
            }
        }

        /// <summary>
        /// Tween the building's exterior depending on if the player enters or leaves
        /// </summary>
        private void TweenExterior()
        {
            if (isInside)
            {
                foreach (SpriteRenderer renderer in exteriorRenderers)
                {
                    interior.SetActive(true);
                    renderer.DOFade(0, transitionTime).OnComplete(() => { renderer.enabled = false; });
                }
            }
            else
            {
                foreach (SpriteRenderer renderer in exteriorRenderers)
                {
                    renderer.enabled = true;
                    renderer.DOFade(1, transitionTime).OnComplete(() => { interior.SetActive(false); });
                }
            }
        }
    }
}