using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class HealthBarMenu : MonoBehaviour
{
    public Image bottom;
    public Image top;
    public TextMeshProUGUI text;
    public BaseUnit unit;
    public Image factionColorBG;
    private int tempHealth = -1000;
    void Update(){
        if (tempHealth != -1000){
            top.fillAmount = (float)tempHealth / (float)unit.maxHealth;
            if (tempHealth < 0){
                text.text =  "0 / " + unit.maxHealth;
                return;
            }
            text.text = tempHealth + " / " + unit.maxHealth;
            return;
        }
        top.fillAmount = (float)unit.health / (float)unit.maxHealth;
        if (unit.health < 0){
            text.text =  "0 / " + unit.maxHealth;
            return;
        }
        text.text = unit.health + " / " + unit.maxHealth;
    }
    public void SetUnit(BaseUnit unit){
        tempHealth = -1000;
        this.unit = unit;
        if (unit.faction == UnitFaction.Hero){
            factionColorBG.color = GameManager.instance.heroColor;
        }else{
            factionColorBG.color = GameManager.instance.enemyColor;
        }
    }
    public void SetHealth(int value){
        tempHealth = value;
    }
}
