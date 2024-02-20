using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class DescriptionMenu : MonoBehaviour
{
    public TMP_Text nameText, descriptionText, weaponClassText, unitClassText;
    public BaseItem item;
    public void SetItem(BaseItem item){
        this.item = item;
        if (item == null){
            descriptionText.text = "";
            nameText.text = "";
            gameObject.SetActive(false);
            return;
        }
        if (item is BaseWeapon){
            descriptionText.text = "";
            nameText.text = "";
            gameObject.SetActive(false);
        }else{
            BaseSkill skill = item as BaseSkill;
            gameObject.SetActive(true);
            descriptionText.text = skill.description;
            weaponClassText.text = "Weapon: " + skill.weaponClass;
            unitClassText.text = "Unit: " + skill.unitClass;
            nameText.text = skill.skillName;
        }
    }
}