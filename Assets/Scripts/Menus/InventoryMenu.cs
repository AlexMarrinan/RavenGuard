using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class InventoryMenu : BaseMenu
{
    public TextMeshProUGUI buttonNameText;
    public List<UnitSummaryMenu> unitSummaries;
    public List<MenuButton> unitButtons;
    public List<MenuButton> inventoryButtons;
    public InventoryScreen currentInventoryScreen;
    public GameObject itemsScreen;
    private Vector3 itemScreenNextPos;
    public float itemScreenSpeed;
    private bool menuMoving;
    public override void Move(Vector2 direction)
    {
        if (menuMoving){
            return;
        }
        base.Move(direction);
        SetNameText();
    }
    public void Update(){
        Debug.Log("bleh");
        if (itemScreenNextPos != itemsScreen.transform.localPosition){
            itemsScreen.transform.localPosition = Vector2.Lerp(itemsScreen.transform.localPosition, itemScreenNextPos, itemScreenSpeed*Time.deltaTime);
        }else{
        }
    }
    public override void Reset()
    {
        SetNameText();
        this.xCount = 4;
        this.yCount = 5;
        unitButtons = new();
        itemsScreen.transform.localPosition = new(860, 180);
        itemScreenNextPos = itemsScreen.transform.localPosition;
        //DISPLAY INVENTORY ITEMS CODE
        for (int i = 0; i < InventoryManager.instance.ItemCount(); i++){
            var b = inventoryButtons[i];
            if (b is ItemButton){
                var ib = b as ItemButton;
                ib.SetItem(InventoryManager.instance.GetItem(i));
            }
        }
        var heroes = UnitManager.instance.GetAllHeroes();
        for (int i = 0; i < heroes.Count; i++){
            UnitSummaryMenu summary = unitSummaries[i];
            summary.Init(heroes[i]);
            unitButtons.Add(summary.weaponButton);
            for (int j = 0; j < 3; j++){
                unitButtons.Add(summary.itemButtons[j]);
            }
        }
        currentInventoryScreen = InventoryScreen.Units;
        buttons = unitButtons;
        base.Reset();
    }
    public override void Select()
    {
        //Debug.Log(buttonNameText.text);
    }
    public void ChangeInventoryScreen(){

    }
    private void SetNameText(){
        buttonNameText.text = InventoryManager.instance.GetItemName(buttonIndex);
    }

    internal void ShowUnits()
    {
        if (menuMoving){
            return;
        }
        itemScreenNextPos += new Vector3(1700, 0);
        buttons = unitButtons;
        this.xCount = 4;
        this.yCount = 5;
        buttonIndex = 0;
        StartCoroutine(WhileMoving());
    }

    internal void ShowItems()
    {
        if (menuMoving){
            return;
        }
        itemScreenNextPos += new Vector3(-1700, 0);
        this.xCount = 5;
        this.yCount = 5;
        buttons = inventoryButtons;
        buttonIndex = 0;
        StartCoroutine(WhileMoving());
    }
    IEnumerator WhileMoving(){
        menuMoving = true;
        highlighImage.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        highlighImage.gameObject.SetActive(true);
        menuMoving = false;
        if (currentInventoryScreen == InventoryScreen.Units){
            currentInventoryScreen = InventoryScreen.Items;
        }else{
            currentInventoryScreen = InventoryScreen.Units;
        }
        Move(Vector2.zero);
        yield return null;
    }
}
    public enum InventoryScreen{
        Units,
        Items,
    }