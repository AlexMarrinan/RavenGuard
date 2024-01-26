using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        var u = UnitManager.instance.selectedUnit;
        UnitManager.instance.RemoveAllValidMoves();
        if (buttonIndex == 0){
            UnitManager.instance.SetValidMoves(u);
        }
        else if (buttonIndex == 1){
            UnitManager.instance.SetValidAttacks(u);
        }
    }

    public override void Reset()
    {
        base.Reset();
        SetNameText();
        buttons.ForEach(b => b.SetOn());
        var u = UnitManager.instance.selectedUnit;
        Debug.Log(u);
        if (u == null)
        {
            return;
        }

        if (u.NumValidAttacks() <= 0){
            buttons[1].SetOn(false);
        }
        BaseSkill skill = u.GetBoringSkill();
        if (skill == null)
        {
            buttons[3].image.sprite = noSkillSprite;
            buttons[3].bonusText = "";
            buttons[3].SetOn(false);
        }
        else
        {
            buttons[3].image.sprite = skill.sprite;
            buttons[3].bonusText = ": " + skill.skillName;
        }
        if (u.hasMoved == true){
            //Disable Move Button
            buttons[0].SetOn(false);
            if (buttons[1].IsOn()){
                buttonIndex = 1;
                UnitManager.instance.RemoveAllValidMoves();
                UnitManager.instance.SetValidAttacks(u);
            }else{
                buttonIndex = 2;
                UnitManager.instance.RemoveAllValidMoves();
            }
        }else{
            buttons[0].SetOn(true);
        }
        SetSkill(u, 4);
        SetSkill(u, 5);
        SetSkill(u, 6);
        SetHighlight();
    }

    private void SetSkill(BaseUnit u, int index)
    {
//        Debug.Log("seting skill:" + index);
        var skill = u.GetSkill(index-4);
        MenuButton b = buttons[index];
        Image skillBG = skillBackgrounds[index-4];
        if (skill == null) {
            b.image.sprite = noSkillSprite;
            b.buttonText.text = "Empty Skill Slot";
            b.bonusText = "";
            skillBG.color = Color.white;
            b.SetOn(false);
        }
        else {
            skill.SetMethod();
            b.buttonText.text = skill.skillName;
            b.image.sprite = skill.sprite;
            if (skill is ActiveSkill){
                skillBG.color = SkillManager.instance.activeSkillColor;
            }else{
                skillBG.color = SkillManager.instance.passiveSkillColor;
                b.SetOn(false);
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
                //MOVE
                MenuManager.instance.CloseMenus();
                GridManager.instance.SelectHoveredTile();
            }
            else if (buttonIndex == 1){
                //ATACK
                MenuManager.instance.CloseMenus();
                GridManager.instance.SelectHoveredTile();
                UnitManager.instance.RemoveAllValidMoves();
                UnitManager.instance.SetValidAttacks(u);
                PathLine.instance.Reset();
                Debug.Log("attacking...");
            }else if (buttonIndex == 2){
                //WAIT
                u.FinishTurn();
                MenuManager.instance.CloseMenus();
            }else if (buttonIndex == 3){
                Debug.Log("boring skill...");
            }else{
                var s = u.GetSkill(buttonIndex - 4);
                if (s != null){
                    s.OnSelect(u);
                }
            }
        }
    }
    private void SetNameText(){
        return;
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
