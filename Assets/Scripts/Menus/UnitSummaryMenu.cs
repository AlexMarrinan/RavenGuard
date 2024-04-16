using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UnitSummaryMenu : BaseMenu
{
    public Image unitIcon, healthBarTop, healthBarBottom, unitHighlightImage;
    public ItemButton weaponButton;
    public TextMeshProUGUI healthText, weaponClassText, unitClassText;
    public XPMenu xpBar;
    public TMP_Text atkT, defT, aglT, atuT, frsT, lckT;
    public List<ItemButton> itemButtons = new();
    public BaseUnit unit;
    public void SetUnit(BaseUnit unit){
        this.unit = unit;
        xpBar.SetUnit(unit);
        unitIcon.sprite = unit.GetSprite();
        //unitIcon.color = unit.GetColor();
        if (unitHighlightImage != null){
            unitHighlightImage.gameObject.SetActive(false);
        }
        healthBarTop.fillAmount = (float)unit.health / (float)unit.maxHealth;
        healthText.text = unit.health + " / " + unit.maxHealth;
        weaponClassText.text = unit.weaponClass.ToString();
        unitClassText.text = unit.unitClass.ToString();

        weaponButton.SetItem(unit.weapon, 0);
        weaponButton.SetUnit(unit);
        itemButtons[0].SetItem(unit.GetSkill(0), 0);
        itemButtons[0].SetUnit(unit);
        itemButtons[1].SetItem(unit.GetSkill(1), 1);
        itemButtons[1].SetUnit(unit);
        itemButtons[2].SetItem(unit.GetSkill(2), 2);
        itemButtons[2].SetUnit(unit);

        var atk = unit.GetAttack();
        atkT.text = atk.total.ToString();
        if (atk.GetStatIncreaseType() == StatIncreaseType.DOWN){
            atkT.color = GameManager.instance.enemyColor;
        }else if (atk.GetStatIncreaseType() == StatIncreaseType.UP){
            atkT.color = Color.green;
        }else{
            atkT.color = Color.white;
        }

        var def = unit.GetDefense();
        defT.text = def.total.ToString();
        if (def.GetStatIncreaseType() == StatIncreaseType.DOWN){
            defT.color = GameManager.instance.enemyColor;
        }else if (def.GetStatIncreaseType() == StatIncreaseType.UP){
            defT.color = Color.green;
        }else{
            defT.color = Color.white;
        }

       var agi = unit.GetAgility();
        aglT.text = agi.total.ToString();
        if (agi.GetStatIncreaseType() == StatIncreaseType.DOWN){
            aglT.color = GameManager.instance.enemyColor;
        }else if (agi.GetStatIncreaseType() == StatIncreaseType.UP){
            aglT.color = Color.green;
        }else{
            aglT.color = Color.white;
        }


       var atu = unit.GetAttuenment();
        atuT.text = atu.total.ToString();
        if (atu.GetStatIncreaseType() == StatIncreaseType.DOWN){
            atuT.color = GameManager.instance.enemyColor;
        }else if (atu.GetStatIncreaseType() == StatIncreaseType.UP){
            atuT.color = Color.green;
        }else{
            atuT.color = Color.white;
        }

        var frs = unit.GetForesight();
        frsT.text = frs.total.ToString();
        if (frs.GetStatIncreaseType() == StatIncreaseType.DOWN){
            frsT.color = GameManager.instance.enemyColor;
        }else if (frs.GetStatIncreaseType() == StatIncreaseType.UP){
            frsT.color = Color.green;
        }else{
            frsT.color = Color.white;
        }

        var lck = unit.GetLuck();
        lckT.text = lck.total.ToString();
        if (lck.GetStatIncreaseType() == StatIncreaseType.DOWN){
            lckT.color = GameManager.instance.enemyColor;
        }else if (lck.GetStatIncreaseType() == StatIncreaseType.UP){
            lckT.color = Color.green;
        }else{
            lckT.color = Color.white;
        }
    }
}
