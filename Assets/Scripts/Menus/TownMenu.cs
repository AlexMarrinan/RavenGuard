using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TownMenu : BaseMenu
{
    public TMP_Text moneyText;
    void Update(){
        PlayerData data = SaveManager.instance.GetData();
        moneyText.text = "$" + data.copperCoins;
    }
}
