using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UnitSummaryMenu : BaseMenu
{
    public Image unitIcon, healthBarTop, healthBarBottom, weaponImage, skill1image, skill2image, skill3image;
    public TextMeshProUGUI healthText, weaponClassText, unitClassText;
    public XPMenu xpBar;
    public TMP_Text atkT, defT, aglT, atuT, frsT, lckT;
    public void Init(BaseUnit unit){
        xpBar.SetUnit(unit);
        unitIcon.sprite = unit.GetSprite();
        unitIcon.color = unit.GetColor();
        healthBarTop.fillAmount = (float)unit.health / (float)unit.maxHealth;
        healthText.text = unit.health + " / " + unit.maxHealth;
        weaponClassText.text = unit.weaponClass.ToString();
        unitClassText.text = unit.unitClass.ToString();
        weaponImage.sprite = unit.weapon.sprite;
        var skill1 = unit.GetSkill(0);
        if (skill1 != null){
            skill1image.sprite = skill1.sprite;
        }
        var skill2 = unit.GetSkill(1);
        if (skill2 != null){
            skill2image.sprite = skill2.sprite;
        }
        var skill3 = unit.GetSkill(2);
        if (skill3 != null){
            skill3image.sprite = skill3.sprite;
        }



        var atk = unit.GetAttack();
        atkT.text = atk.total.ToString();
        if (atk.GetStatIncreaseType() == StatIncreaseType.DOWN){
            atkT.color = Color.red;
        }else if (atk.GetStatIncreaseType() == StatIncreaseType.UP){
            atkT.color = Color.green;
        }else{
            atkT.color = Color.white;
        }

        var def = unit.GetDefense();
        defT.text = def.total.ToString();
        if (def.GetStatIncreaseType() == StatIncreaseType.DOWN){
            defT.color = Color.red;
        }else if (def.GetStatIncreaseType() == StatIncreaseType.UP){
            defT.color = Color.green;
        }else{
            defT.color = Color.white;
        }

       var agi = unit.GetAgility();
        aglT.text = agi.total.ToString();
        if (agi.GetStatIncreaseType() == StatIncreaseType.DOWN){
            aglT.color = Color.red;
        }else if (agi.GetStatIncreaseType() == StatIncreaseType.UP){
            aglT.color = Color.green;
        }else{
            aglT.color = Color.white;
        }


       var atu = unit.GetAttuenment();
        atuT.text = atu.total.ToString();
        if (atu.GetStatIncreaseType() == StatIncreaseType.DOWN){
            atuT.color = Color.red;
        }else if (atu.GetStatIncreaseType() == StatIncreaseType.UP){
            atuT.color = Color.green;
        }else{
            atuT.color = Color.white;
        }

        var frs = unit.GetForesight();
        frsT.text = frs.total.ToString();
        if (frs.GetStatIncreaseType() == StatIncreaseType.DOWN){
            frsT.color = Color.red;
        }else if (frs.GetStatIncreaseType() == StatIncreaseType.UP){
            frsT.color = Color.green;
        }else{
            frsT.color = Color.white;
        }

        var lck = unit.GetLuck();
        lckT.text = lck.total.ToString();
        if (lck.GetStatIncreaseType() == StatIncreaseType.DOWN){
            lckT.color = Color.red;
        }else if (lck.GetStatIncreaseType() == StatIncreaseType.UP){
            lckT.color = Color.green;
        }else{
            lckT.color = Color.white;
        }
    }
}
