using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BattleMenu : BaseMenu
{
    public Sprite noSkillSprite;
    public TextMeshProUGUI buttonNameText;
    public UnitStatsMenu heroStats, enemyStats;
    public override void Move(Vector2 direction)
    {
        base.Move(direction);
        SetNameText();
    }

    public override void Reset()
    {
        base.Reset();
        // SetNameText();
        // var u =UnitManager.instance.selectedUnit;
        // if (u == null){
        //     return;
        // }
        // BaseSkill skill = u.activeSkill;
        // if (skill == null){
        //     buttons[1].image.sprite = noSkillSprite;
        //     buttons[1].bonusText = "";
        // }else{
        //     buttons[1].image.sprite = skill.sprite;
        //     buttons[1].bonusText = ": " + skill.skillName;
        // }
        // skill = u.universalPassiveSkill;
        // if (skill == null){
        //     buttons[2].image.sprite = noSkillSprite;
        //     buttons[2].bonusText = "";
        // }else{
        //     buttons[2].image.sprite = skill.sprite;
        //     buttons[1].bonusText = ": " + skill.skillName;
        // }
        
        // skill = u.classPassiveSkill;
        // if (u.classPassiveSkill == null){
        //     buttons[3].image.sprite = noSkillSprite;
        //     buttons[2].bonusText = "";
        // }else{
        //     buttons[3].image.sprite = skill.sprite;
        //     buttons[3].bonusText = skill.skillName;
        // }
    }

    public override void Select()
    {
        base.Select();
        Debug.Log(GetCurrentButton().buttonName);
    }
    private void SetNameText(){
        // var b = GetCurrentButton();
        // buttonNameText.text = b.buttonName + b.bonusText;
    }
}
