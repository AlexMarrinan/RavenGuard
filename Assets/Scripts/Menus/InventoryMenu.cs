using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InventoryMenu : BaseMenu
{
    public TextMeshProUGUI buttonNameText;
    public List<UnitSummaryMenu> unitSummaries;
    public override void Move(Vector2 direction)
    {
        base.Move(direction);
        SetNameText();
    }
    public override void Reset()
    {
        base.Reset();
        SetNameText();
        for (int i = 0; i < InventoryManager.instance.ItemCount(); i++){
            buttons[i].image.sprite = InventoryManager.instance.GetItem(i).sprite;
        }
        var heroes = UnitManager.instance.GetAllHeroes();
        for (int i = 0; i < heroes.Count; i++){
            unitSummaries[i].Init(heroes[i]);
        }
    }
    public override void Select()
    {
        Debug.Log(buttonNameText.text);
    }
    private void SetNameText(){
        buttonNameText.text = InventoryManager.instance.GetItemName(buttonIndex);
    }
}
