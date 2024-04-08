using System;
using System.Collections.Generic;
using Game.Hub.Interactables;
using Unity.VisualScripting;
using UnityEngine;
using AYellowpaper.SerializedCollections;

[Serializable]
public class PlayerData {

    //Data to Save
    public int copperCoins;
    public ParagonInfo currentParagon;
    public List<ParagonInfo> paragonsOwned;
    public SerializedDictionary<SkillProgressionGroup, int> skillUpgrades;
    //TODO: SKILL UPGRADES ON SAVE FILE

    //Boilerplate
    private static PlayerData instance = null;

    public PlayerData(){
        copperCoins = 0;
        paragonsOwned = new();
        skillUpgrades = new();
    }
    public void SetData(PlayerData other){
        this.copperCoins = other.copperCoins;
        this.currentParagon = other.currentParagon;
        this.paragonsOwned = other.paragonsOwned;
        this.skillUpgrades = other.skillUpgrades;
    }
    public static PlayerData Instance{
        get{
            if (instance == null){
                instance = new();
            }
            return instance;
        }
    }
}