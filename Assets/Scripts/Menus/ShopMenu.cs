using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ShopMenu : BaseMenu
{
    public List<ItemButton> itemButtons;
    public DescriptionMenu descriptionMenu;
    public int boughtItems = 0;
    public override void Reset()
    {
        base.Reset();
        boughtItems = 0;
        foreach (ItemButton ib in itemButtons){
            BaseSkill randomSkill = SkillManager.instance.GetRandomSkill();
            ib.SetItem(randomSkill);
        }
        descriptionMenu.SetItem(itemButtons[buttonIndex].item);
    }
    public override void Move(Vector2 direction)
    {
        base.Move(direction);
        descriptionMenu.SetItem(itemButtons[buttonIndex].item);
    }
    public override void Select()
    {
        InventoryManager.instance.AddItem(itemButtons[buttonIndex].item);
        boughtItems++;
        if (boughtItems >= 2){
            GameManager.instance.LoadOverworldMap();
            return;
        }
        buttons[buttonIndex].SetOn(false);
        Move(new Vector2(1, 0));
    }
}