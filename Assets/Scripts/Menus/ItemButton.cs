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
    public int index;
    public void Reset(){
        unit = null;
        item = null;
        image.sprite = null;
        bgImage.color = Color.white;
    }
    public override void OnPress(){
        //TODO PICKUP ITEM AND MOVE TO OTHER MENU
    }
    public BaseItem GetItem(){
        return item;
    }
    public virtual void SetUnit(BaseUnit unit){
        this.unit = unit;
    }
    public virtual void SetItem(BaseItem item, int index = -1){
        this.index = index;
        if (item == null){
            this.image.sprite = null;
            this.item = null;
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
        bgImage.color = new Color(0.745f, 0.659f, 0.565f);
    }
}

public enum ItemType{
    Skill,
    Weapon,
}
