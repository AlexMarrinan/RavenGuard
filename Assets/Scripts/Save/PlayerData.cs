using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerData {

    //Data to Save
    public int copperCoins;
    public List<UnitClass> paragonsOwned;


    //Boilerplate
    private static PlayerData instance = null;
    public void SetData(PlayerData other){
        this.copperCoins = other.copperCoins;
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