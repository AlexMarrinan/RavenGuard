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
    public XPMenu xpBar;
    public void Start(){
        if (faceDirection == FaceDirection.Left){
            unitIcon.transform.localScale = new Vector3(-1, 1, 1);
        }
        if (this.faction == UnitFaction.Hero){
            unitIcon.color = Color.white;
        }else{
            unitIcon.color = GameManager.instance.enemyColor;
        }
    }
    public void SetUnit(BaseUnit unit){
        this.unit = unit;
        this.buttonIndex = 1;
        EnableHighlight(false);
        // if (MenuManager.instance.otherUnitStatsMenu.gameObject.activeSelf){
        //     MenuManager.instance.unitStatsMenu.EnableHighlight(false);
        //     MenuManager.instance.otherUnitStatsMenu.EnableHighlight(false);
        // }else{
        //     SetHighlight();
        // }
        DisplayUnit();
    }

    private void DisplayUnit(){
        this.transform.SetAsLastSibling();
        unitIcon.sprite = unit.GetSprite();
        unitNameText.text = unit.unitName;
        unitClassText.text = unit.unitClass.ToString();
        if (unit.weapon != null){
            weaponImage.sprite = unit.weapon.sprite;
        }else{
            weaponImage.sprite = null;
        }
        weaponText.text = unit.weapon.weaponName;
        weaponClassText.text = unit.weapon.weaponClass.ToString();
        
        healthBar.SetUnit(unit);
        if (unit.faction == UnitFaction.Enemy){
            xpBar.gameObject.SetActive(false);
        }else{
            xpBar.gameObject.SetActive(true);
            xpBar.SetUnit(unit);
        }

        var atk = unit.GetAttack();
        attackText.text = atk.total.ToString();
        if (atk.GetStatIncreaseType() == StatIncreaseType.DOWN){
            attackText.color = GameManager.instance.enemyColor;
        }else if (atk.GetStatIncreaseType() == StatIncreaseType.UP){
            attackText.color = Color.green;
        }else{
            attackText.color = Color.white;
        }

        var def = unit.GetDefense();
        defenseText.text = def.total.ToString();
        if (def.GetStatIncreaseType() == StatIncreaseType.DOWN){
            defenseText.color = GameManager.instance.enemyColor;
        }else if (def.GetStatIncreaseType() == StatIncreaseType.UP){
            defenseText.color = Color.green;
        }else{
            defenseText.color = Color.white;
        }

       var agi = unit.GetAgility();
        agilityText.text = agi.total.ToString();
        if (agi.GetStatIncreaseType() == StatIncreaseType.DOWN){
            agilityText.color = GameManager.instance.enemyColor;
        }else if (agi.GetStatIncreaseType() == StatIncreaseType.UP){
            agilityText.color = Color.green;
        }else{
            agilityText.color = Color.white;
        }


       var atu = unit.GetAttuenment();
        attunementText.text = atu.total.ToString();
        if (atu.GetStatIncreaseType() == StatIncreaseType.DOWN){
            attunementText.color = GameManager.instance.enemyColor;
        }else if (atu.GetStatIncreaseType() == StatIncreaseType.UP){
            attunementText.color = Color.green;
        }else{
            attunementText.color = Color.white;
        }

        var frs = unit.GetForesight();
        forsightText.text = frs.total.ToString();
        if (frs.GetStatIncreaseType() == StatIncreaseType.DOWN){
            forsightText.color = GameManager.instance.enemyColor;
        }else if (frs.GetStatIncreaseType() == StatIncreaseType.UP){
            forsightText.color = Color.green;
        }else{
            forsightText.color = Color.white;
        }

        var lck = unit.GetLuck();
        luckText.text = lck.total.ToString();
        if (lck.GetStatIncreaseType() == StatIncreaseType.DOWN){
            luckText.color = GameManager.instance.enemyColor;
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