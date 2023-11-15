using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitActionMenu : BaseMenu
{
    public Sprite noSkillSprite;
    public TextMeshProUGUI buttonNameText, descriptionText;
    [SerializeField]
    public List<Image> skillBackgrounds;
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
        SetSkill(u, 1);
        SetSkill(u, 2);
        SetSkill(u, 3);
    }

    private void SetSkill(BaseUnit u, int index)
    {
        var skill = u.GetSkill(index-1);
        if (skill == null) {
            buttons[index].image.sprite = noSkillSprite;
            buttons[index].bonusText = "";
        }
        else {
            skill.SetMethod();
            buttons[index].image.sprite = skill.sprite;
            buttons[index].bonusText = ": " + skill.skillName;
            Image skillBG = skillBackgrounds[index-1];
            if (skill is ActiveSkill){
                skillBG.color = SkillManager.instance.activeSkillColor;
            }else{
                skillBG.color = SkillManager.instance.passiveSkillColor;
            }
        }
    }

    public override void Select(){
        var u = UnitManager.instance.selectedUnit;
        if (!TurnManager.instance.unitsAwaitingOrders.Contains(u)){
            return;
        }
        if (buttonIndex >= 0){
            if (buttonIndex == 0){
                u.FinishTurn();
                MenuManager.instance.ToggleUnitActionMenu();
            }else{
                var s = u.GetSkill(buttonIndex - 1);
                if (s != null){
                    s.OnSelect(u);
                }
            }
        }
    }
    private void SetNameText(){
        var b = GetCurrentButton();
        buttonNameText.text = b.buttonName + b.bonusText;
        if (buttonIndex > 0){
            var u = UnitManager.instance.selectedUnit;
            var skill = u.GetSkill(buttonIndex-1);
            if (skill == null){
                return;
            }
            var activeText = "Active: ";
            if (skill is PassiveSkill){
                activeText = "Passive: ";
            }
            buttonNameText.text = activeText + skill.skillName;
            descriptionText.text = skill.description;
        }else{
            descriptionText.text = "";
            
        }
    }
}
