using System;
using Game.Hub.Interactables;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Hub
{
    public class PlayerCharacter:MonoBehaviour
    {
        [SerializeField] private SpriteRenderer playerImage;
        [SerializeField] private ParagonInfo playerParagonInfo;

        private void Awake()
        {
            playerParagonInfo.sprite=playerImage.sprite;
        }

        public ParagonInfo SwapParagonInfo(ParagonInfo paragonInfo)
        {
            ParagonInfo tempPlayerInfo = playerParagonInfo;
            playerParagonInfo = paragonInfo;
            playerImage.sprite = playerParagonInfo.sprite;
            return tempPlayerInfo;
        }
    }
}