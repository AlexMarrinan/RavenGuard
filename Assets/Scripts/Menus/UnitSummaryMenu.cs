using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitSummaryMenu : BaseMenu
{
    public Image unitIcon, healthBarTop, healthBarBottom;
    public TextMeshProUGUI healthText, weaponClassText, unitClassText;
    public void Init(BaseUnit unit){
        unitIcon.sprite = unit.GetSprite();
        unitIcon.color = unit.GetColor();
        healthBarTop.fillAmount = (float)unit.health / (float)unit.maxHealth;
        healthText.text = unit.health + " / " + unit.maxHealth;
        weaponClassText.text = unit.weaponClass.ToString();
        unitClassText.text = unit.unitClass.ToString();

        if (unit.weapon != null){
            buttons[0].image.sprite = unit.weapon.sprite;
        }
        if (unit.activeSkill != null){
            buttons[1].image.sprite = unit.activeSkill.sprite;
        }
        if (unit.universalPassiveSkill != null){
            buttons[2].image.sprite = unit.universalPassiveSkill.sprite;
        }
        if (unit.classPassiveSkill != null){
            buttons[3].image.sprite = unit.classPassiveSkill.sprite;
        }
    }
}
