using System;
using Game.Hub.Interactables;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Hub
{
    public class PlayerCharacter : MonoBehaviour
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

            SaveManager.instance.SetCurrentParagon(playerParagonInfo);

            return tempPlayerInfo;
        }
        public void SetParagonInfo(ParagonInfo paragonInfo){
            if (paragonInfo == null){
                return;
            }
            Debug.Log(paragonInfo);
            playerParagonInfo = paragonInfo;
            Debug.Log(playerImage);
            Debug.Log(playerParagonInfo);
            playerImage.sprite = playerParagonInfo.sprite;
        }
    }
}