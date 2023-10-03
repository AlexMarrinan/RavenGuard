using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HealthBarMenu : MonoBehaviour
{
    public Image bottom;
    public Image top;
    public TextMeshProUGUI text;
    public BaseUnit unit;
    void Update(){
        top.fillAmount = (float)unit.health / (float)unit.maxHealth;
        if (unit.health < 0){
            text.text =  "0 / " + unit.maxHealth;
            return;
        }
        text.text = unit.health + " / " + unit.maxHealth;
    }
    public void SetUnit(BaseUnit unit){
        this.unit = unit;
    }
}
