using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class UnitStatsMenu : BaseMenu
{
    private BaseUnit unit;
    public Image unitIcon, healthBarTop, healthBarBottom;
    public FaceDirection faceDirection;
    public UnitFaction faction;
    public Image weaponImage;
    public TextMeshProUGUI unitNameText, weaponText, attackText, defenseText, agilityText, attunementText, forsightText, luckText;
    public TextMeshProUGUI healthText, weaponClassText, unitClassText;
    public void Start(){
        if (faceDirection == FaceDirection.Left){
            unitIcon.transform.localScale = new Vector3(-1, 1, 1);
        }
        if (this.faction == UnitFaction.Hero){
            unitIcon.color = Color.cyan;
        }else{
            unitIcon.color = Color.red;
        }
    }
    public void SetUnit(BaseUnit unit){
        this.unit = unit;
        DisplayUnit();
    }

    private void DisplayUnit(){
        unitIcon.sprite = unit.GetSprite();
        unitNameText.text = unit.unitName;
        unitClassText.text = unit.unitClass.ToString();
        weaponImage.sprite = unit.weapon.sprite;
        weaponText.text = unit.weapon.weaponName;
        weaponClassText.text = unit.weapon.weaponClass.ToString();
        
        attackText.text = unit.GetAttack().ToString();
        defenseText.text = unit.GetDefense().ToString();
        agilityText.text = unit.GetAgility().ToString();
        attunementText.text = unit.GetAttuenment().ToString();
        forsightText.text = unit.GetForesight().ToString();
        luckText.text = unit.GetLuck().ToString();
    }
}

public enum FaceDirection{
    Left,
    Right
}