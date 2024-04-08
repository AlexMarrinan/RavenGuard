using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LevelEndMenu : BaseMenu
{
    public List<ItemButton> itemButtons;
    public DescriptionMenu descriptionMenu;
    public override void Reset()
    {
        base.Reset();
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
        MenuManager.instance.ToggleInventoryMenu();
        MenuManager.instance.EnableInventorySwapping();
        //GameManager.instance.LoadNextLevel();
    }
}