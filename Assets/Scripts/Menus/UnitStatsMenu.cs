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
    public Image weaponImage;
    public TextMeshProUGUI unitNameText, weaponText, attackText, defenseText, agilityText, attunementText, forsightText, luckText;
    public TextMeshProUGUI healthText, weaponClassText, unitClassText;
    public void SetUnit(BaseUnit unit){
        this.unit = unit;
        DisplayUnit();
    }

    private void DisplayUnit(){
        unitIcon.sprite = unit.GetSprite();
        weaponImage.sprite = unit.weapon.sprite;
        weaponText.text = unit.weapon.weaponName;
    }
}

public enum FaceDirection{
    Left,
    Right
}