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
        var u = UnitManager.instance.selectedUnit;
        UnitManager.instance.RemoveAllValidMoves();
        if (buttonIndex == 0){
            UnitManager.instance.SetValidMoves(u);
        }
        else if (buttonIndex == 1){
            UnitManager.instance.SetValidAttacks(u);
        }
        else if (buttonIndex > 2){
            var skill = UnitManager.instance.selectedUnit.GetSkill(buttonIndex - 3);
            if (skill != null && skill is ActiveSkill){
                SkillManager.instance.currentActiveSkill = skill as ActiveSkill;
                SkillManager.instance.user = UnitManager.instance.selectedUnit;
                SkillManager.instance.ShowSkillPreview();
            }
        }
    }

    public override void Reset()
    {
        base.Reset();
        buttons.ForEach(b => b.SetOn());
        var u = UnitManager.instance.selectedUnit;
//        Debug.Log(u);
        if (u == null)
        {
            return;
        }
        if (u.NumValidAttacks() <= 0){
            buttons[1].SetOn(false);
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
        SetSkill(u, 3);
        SetSkill(u, 4);
        SetSkill(u, 5);
        SetHighlight();
    }

    private void SetSkill(BaseUnit u, int index)
    {
//        Debug.Log("seting skill:" + index);
        int trueIndex = index-3;
        var skill = u.GetSkill(trueIndex);
        MenuButton b = buttons[index];
        Image skillBG = skillBackgrounds[trueIndex];
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
                if (u.activeSkillCooldowns[trueIndex] <= 0){
                    b.SetOn(true);
                }else{
                    b.SetOn(false);
                }
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
//                Debug.Log("Here!!!! 1");
                MenuManager.instance.CloseMenus();
                GridManager.instance.SelectHoveredTile();
                UnitManager.instance.RemoveAllValidMoves();
                UnitManager.instance.SetValidAttacks(u);
                PathLine.instance.Reset();
//                Debug.Log("attacking...");
            }else if (buttonIndex == 2){
                //WAIT
                u.FinishTurn();
                MenuManager.instance.CloseMenus();
            }else{
                var s = u.GetSkill(buttonIndex - 3);
                Debug.Log(s);
                if (s != null){
                    s.OnSelect(u);
                }
            }
        }
    }
}
