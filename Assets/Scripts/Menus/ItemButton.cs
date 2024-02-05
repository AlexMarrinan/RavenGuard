using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemButton : MenuButton
{
    InventoryMenu inventoryMenu;
    public BaseUnit unit;
    public BaseItem item;
    public Image bgImage;
    public override void OnPress(){
        //TODO PICKUP ITEM AND MOVE TO OTHER MENU
    }
    public virtual void SetItem(BaseItem item){
        if (item == null){
            return;
        }
        this.item = item;
        this.image.sprite = item.sprite;
        if (item is BaseSkill){
            SetSkill(item as BaseSkill);
        }else{
            SetWeapon(item as BaseWeapon);
        }
    }
    private void SetSkill(BaseSkill skill) {
        if (skill is ActiveSkill){
            bgImage.color = SkillManager.instance.activeSkillColor;
        }else{
            bgImage.color = SkillManager.instance.passiveSkillColor;
        }
    }
    private void SetWeapon(BaseWeapon weapon)
    {
        bgImage.color = new Color(190, 168, 144);
    }
}

public enum ItemType{
    Skill,
    Weapon,
}
