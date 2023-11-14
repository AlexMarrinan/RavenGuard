using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UnitActionMenu : BaseMenu
{
    public Sprite noSkillSprite;
    public TextMeshProUGUI buttonNameText;
    public override void Move(Vector2 direction)
    {
        base.Move(direction);
        SetNameText();
    }

    public override void Reset()
    {
        base.Reset();
        SetNameText();
        var u = UnitManager.instance.selectedUnit;
        if (u == null)
        {
            return;
        }
        BaseSkill skill = u.GetBoringSkill();
        if (skill == null)
        {
            buttons[1].image.sprite = noSkillSprite;
            buttons[1].bonusText = "";
        }
        else
        {
            buttons[1].image.sprite = skill.sprite;
            buttons[1].bonusText = ": " + skill.skillName;
        }
        SetSkill(u, 2);
        SetSkill(u, 3);
        SetSkill(u, 4);
    }

    private void SetSkill(BaseUnit u, int index)
    {
        var skill = u.GetSkill(index-2);
        if (skill == null) {
            buttons[index].image.sprite = noSkillSprite;
            buttons[index].bonusText = "";
        }
        else {
            skill.SetMethod();
            buttons[index].image.sprite = skill.sprite;
            buttons[index].bonusText = ": " + skill.skillName;
        }
    }

    public override void Select(){
        base.Select();
        if (buttonIndex > 0){
            var u = UnitManager.instance.selectedUnit;
            if (buttonIndex == 1){
                var s = u.GetBoringSkill();
                if (s != null){
                    s.OnSelect(u);
                }
            }else{
                var s = u.GetSkill(buttonIndex - 2);
                if (s != null){
                    s.OnSelect(u);
                }
            }
        }
    }
    private void SetNameText(){
        var b = GetCurrentButton();
        buttonNameText.text = b.buttonName + b.bonusText;
    }
}
