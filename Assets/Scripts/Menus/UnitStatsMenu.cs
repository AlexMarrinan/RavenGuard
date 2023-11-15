using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class UnitStatsMenu : BaseMenu
{
    private BaseUnit unit;
    public Image unitIcon;
    public FaceDirection faceDirection;
    public UnitFaction faction;
    public Image weaponImage, skillIcon0, skillIcon1, skillIcon2, skillIcon0BG, skillIcon1BG, skillIcon2BG;
    public TextMeshProUGUI unitNameText, weaponText, attackText, defenseText, agilityText, attunementText, forsightText, luckText;
    public TextMeshProUGUI weaponClassText, unitClassText;
    public HealthBarMenu healthBar;
    public void Start(){
        if (faceDirection == FaceDirection.Left){
            unitIcon.transform.localScale = new Vector3(-1, 1, 1);
        }
        if (this.faction == UnitFaction.Hero){
            unitIcon.color = Color.white;
        }else{
            unitIcon.color = Color.red;
        }
    }
    public void SetUnit(BaseUnit unit){
        this.unit = unit;
        DisplayUnit();
    }

    private void DisplayUnit(){
        this.transform.SetAsLastSibling();
        unitIcon.sprite = unit.GetSprite();
        unitNameText.text = unit.unitName;
        unitClassText.text = unit.unitClass.ToString();
        weaponImage.sprite = unit.weapon.sprite;
        weaponText.text = unit.weapon.weaponName;
        weaponClassText.text = unit.weapon.weaponClass.ToString();
        
        healthBar.SetUnit(unit);

        var atk = unit.GetAttack();
        attackText.text = atk.total.ToString();
        if (atk.GetStatIncreaseType() == StatIncreaseType.DOWN){
            attackText.color = Color.red;
        }else if (atk.GetStatIncreaseType() == StatIncreaseType.UP){
            attackText.color = Color.green;
        }else{
            attackText.color = Color.white;
        }

        var def = unit.GetDefense();
        defenseText.text = def.total.ToString();
        if (def.GetStatIncreaseType() == StatIncreaseType.DOWN){
            defenseText.color = Color.red;
        }else if (def.GetStatIncreaseType() == StatIncreaseType.UP){
            defenseText.color = Color.green;
        }else{
            defenseText.color = Color.white;
        }

       var agi = unit.GetAgility();
        agilityText.text = agi.total.ToString();
        if (agi.GetStatIncreaseType() == StatIncreaseType.DOWN){
            agilityText.color = Color.red;
        }else if (agi.GetStatIncreaseType() == StatIncreaseType.UP){
            agilityText.color = Color.green;
        }else{
            agilityText.color = Color.white;
        }


       var atu = unit.GetAttuenment();
        attunementText.text = atu.total.ToString();
        if (atu.GetStatIncreaseType() == StatIncreaseType.DOWN){
            attunementText.color = Color.red;
        }else if (atu.GetStatIncreaseType() == StatIncreaseType.UP){
            attunementText.color = Color.green;
        }else{
            attunementText.color = Color.white;
        }

        var frs = unit.GetForesight();
        forsightText.text = frs.total.ToString();
        if (frs.GetStatIncreaseType() == StatIncreaseType.DOWN){
            forsightText.color = Color.red;
        }else if (frs.GetStatIncreaseType() == StatIncreaseType.UP){
            forsightText.color = Color.green;
        }else{
            forsightText.color = Color.white;
        }

        var lck = unit.GetLuck();
        luckText.text = lck.total.ToString();
        if (lck.GetStatIncreaseType() == StatIncreaseType.DOWN){
            luckText.color = Color.red;
        }else if (lck.GetStatIncreaseType() == StatIncreaseType.UP){
            luckText.color = Color.green;
        }else{
            luckText.color = Color.white;
        }

        var skill = unit.GetSkill(0);
        if (skill == null){
            skillIcon0.sprite = null;
        }else{
            skillIcon0.sprite = skill.sprite;
        }
        if (skill is ActiveSkill){
            skillIcon0BG.color = SkillManager.instance.activeSkillColor;
        }else{
            skillIcon0BG.color = SkillManager.instance.passiveSkillColor;
        }

        skill = unit.GetSkill(1);
        if (skill == null){
            skillIcon1.sprite = null;
        }else{
            skillIcon1.sprite = skill.sprite;
        }
        if (skill is ActiveSkill){
            skillIcon1BG.color = SkillManager.instance.activeSkillColor;
        }else{
            skillIcon1BG.color = SkillManager.instance.passiveSkillColor;
        }

        skill = unit.GetSkill(2);
        if (skill == null){
            skillIcon2.sprite = null;
        }else{
            skillIcon2.sprite = skill.sprite;
        }
        if (skill is ActiveSkill){
            skillIcon2BG.color = SkillManager.instance.activeSkillColor;
        }else{
            skillIcon2BG.color = SkillManager.instance.passiveSkillColor;
        }
    }
}

public enum FaceDirection{
    Left,
    Right
}