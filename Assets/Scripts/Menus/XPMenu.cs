using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class XPMenu : MonoBehaviour
{
    public Image bottom;
    public Image top;
    public TextMeshProUGUI text, leveltext;
    public BaseUnit unit;
    private int tempXP = -1000;
    void Update(){
        if (tempXP != -1000){
            top.fillAmount = (float)tempXP / (float)unit.maxXP;
            if (tempXP < 0){
                text.text =  "0 / " + unit.maxXP;
                return;
            }
            text.text = tempXP + " / " + unit.maxXP;
            return;
        }
        top.fillAmount = (float)unit.currentXP / (float)unit.maxXP;
        if (unit.currentXP < 0){
            text.text =  "0 / " + unit.maxXP;
            return;
        }
        text.text = unit.currentXP + " / " + unit.maxXP;
        leveltext.text = "Lv. " + unit.level;

    }
    public void SetUnit(BaseUnit unit){
        tempXP = -1000;
        this.unit = unit;
    }
    public void SetXP(int value){
        tempXP = value;
    }
}
