using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Game.Hub;
using UnityEngine;

namespace Buildings{
    public class Building : MonoBehaviour
    {
        [SerializeField] private GameObject interior;
        [SerializeField] private List<SpriteRenderer> exteriorRenderers;
        [SerializeField] private float transitionTime;
        [SerializeField] private Transform outsideSpawm, insideSpawn;
        [SerializeField] private PlayerMovement player;

        private bool isInside;

        /// <summary>
        /// Toggle whether the player is in the building or not
        /// </summary>
        public void ToggleBuilding()
        {
            Debug.Log("Toggle building!");
            if(!isInside) EnterBuilding();
            else ExitBuilding();
        }
        
        /// <summary>
        /// Player enters building
        /// </summary>
        public void EnterBuilding()
        {
            if (!isInside)
            {
                isInside = true;
                player.transform.position = insideSpawn.position;
                //TweenExterior();
            }
        }
        
        /// <summary>
        /// Player leaves building
        /// </summary>
        public void ExitBuilding()
        {
            Debug.Log("Exiting...");
            if (isInside)
            {
                isInside = false;
                player.transform.position = outsideSpawm.position;
                //TweenExterior();
            }
        }

        /// <summary>
        /// Tween the building's exterior depending on if the player enters or leaves
        /// </summary>
        private void TweenExterior()
        {
            StopTweening();
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

        /// <summary>
        /// Stop active tweening
        /// </summary>
        private void StopTweening()
        {
            foreach (SpriteRenderer renderer in exteriorRenderers)
            {
                renderer.DOKill();
            }
        }
    }
}