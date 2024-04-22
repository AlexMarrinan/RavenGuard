using System;
using Game.Dialogue;
using TMPro;
using UnityEngine;

namespace Game.Hub.Interactables
{
    public class Paragon:NPC
    {
        [SerializeField] private ParagonInfo paragonInfo;
        [SerializeField] private TextMeshPro costText;

        private void Awake()
        {
            if (paragonInfo != null){
                this.image.sprite = paragonInfo.sprite;
            }
            if (costText != null){
                costText.text = "$" + paragonInfo.cost;
            }
        }

        public override void StartCutscene()
        {
            gameObject.name = speakerData.characterName;
            EventDialogueManager.Instance.AddVariable("$CharacterName", speakerData.characterName);
            base.StartCutscene();
        }

        public override void Interact()
        {
            paragonInfo=FindObjectOfType<PlayerCharacter>().SwapParagonInfo(paragonInfo);
            image.sprite = paragonInfo.sprite;
        }

        public void SwapPlayerParagonInfo()
        {
            paragonInfo=FindObjectOfType<PlayerCharacter>().SwapParagonInfo(paragonInfo);
            image.sprite = paragonInfo.sprite;
        }
        public ParagonInfo GetParagonInfo(){
            return paragonInfo;
        }
        public void SetParagonInfo(ParagonInfo paragonInfo){
            this.paragonInfo = paragonInfo;
            this.image.sprite = paragonInfo.sprite;
        }

        public void PurchaseParagon(){
            if (!SaveManager.instance.SpendCopperCoins(paragonInfo.cost)){
  //              Debug.Log("not enough coins!");
                return;
            }
//            Debug.Log("Bought new paragon unit!");
            SaveManager.instance.BuyParagon(this.paragonInfo);
            gameObject.SetActive(false);
        }
    }
}