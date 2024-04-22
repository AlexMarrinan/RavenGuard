using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using System.Linq;
public class InventoryMenu : BaseMenu
{
    public TextMeshProUGUI buttonNameText;
    public List<UnitSummaryMenu> unitSummaries;
    public List<MenuButton> unitButtons;
    public List<MenuButton> inventoryButtons;
    public InventoryScreen currentInventoryScreen;
    public GameObject continutePrompt;
    public GameObject itemsScreen;
    public GameObject paragonInfoScreen;
    public TMP_Text paragonInfoText;
    private Vector3 itemScreenNextPos;
    public float itemScreenSpeed;
    private bool menuMoving;
    public BaseItem hoveredItem;
    public ItemButton hoveredItemButton;
    public DescriptionMenu descriptionMenu;
    public bool swapping;

    public override void Move(Vector2 direction)
    {
        ShowParagonSkills(false);
        if (menuMoving){
            return;
        }
        base.Move(direction);
        SetNameText();
        if (currentInventoryScreen == InventoryScreen.Items){
            HighlightUnits();
        }
    }
    public void ShowParagonSkills(bool active=true){
        paragonInfoScreen.SetActive(active);
        if (active){
            paragonInfoText.text = UnitManager.instance.GetPargonSkills().description;
        }
    }

    private void HighlightUnits()
    {
        var button = GetCurrentButton();
        if (button is not ItemButton){
            return;
        }
        BaseItem i = (button as ItemButton).GetItem();
        foreach (UnitSummaryMenu usm in unitSummaries){
            if (usm.unit == null){
                continue;
            }
            usm.unitHighlightImage.gameObject.SetActive(false);
            if (usm.unit.CanUseSkill(i) || usm.unit.CanUseWeapon(i)){
                usm.unitHighlightImage.gameObject.SetActive(true);
            }
        }
    }

    public void Update(){
        if (itemScreenNextPos != itemsScreen.transform.localPosition){
            itemsScreen.transform.localPosition = Vector2.Lerp(itemsScreen.transform.localPosition, itemScreenNextPos, itemScreenSpeed*Time.deltaTime);
        }else{
        }
    }
    public override void Reset()
    {
        ShowParagonSkills(false);
        SetNameText();
        hoveredItemButton.Reset();
        this.xCount = 3;
        this.yCount = UnitManager.instance.GetAllHeroes().Count;

        itemsScreen.transform.localPosition = new(860, 180);
        itemScreenNextPos = itemsScreen.transform.localPosition;
        menuMoving = false;
        ResetButtons();
        currentInventoryScreen = InventoryScreen.Units;
        buttons = unitButtons;
        base.Reset();
    }

    private void ResetButtons()
    {
        hoveredItemButton.gameObject.SetActive(false);
        hoveredItem = null;
        hoveredItemButton.Reset();
        unitButtons = new();
        //DISPLAY INVENTORY ITEMS CODE
        ResetItemButtons();
        var heroes = UnitManager.instance.GetAllHeroes();
        unitSummaries.ForEach(summary => summary.unit = null);
        unitSummaries.ForEach(summary => summary.gameObject.SetActive(false));
        for (int i = 0; i < heroes.Count; i++)
        {
            UnitSummaryMenu summary = unitSummaries[i];
            summary.gameObject.SetActive(true);
            summary.SetUnit(heroes[i]);
            for (int j = 0; j < 3; j++)
            {
                summary.itemButtons[j].SetOn(true);
                unitButtons.Add(summary.itemButtons[j]);
            }
        }
    }

    private void ResetItemButtons(bool start = true)
    {
        inventoryButtons.ForEach(ib => ib.SetOn(false));
        inventoryButtons.ForEach(ib => (ib as ItemButton).Reset());
        for (int i = 0; i < InventoryManager.instance.ItemCount(); i++)
        {
            var b = inventoryButtons[i];
            if (b is ItemButton)
            {
                var ib = b as ItemButton;
                ib.SetItem(InventoryManager.instance.GetItem(i));
            }
            b.SetOn(true);
        }
    }

    public override void Select()
    {
        if (!swapping){
            return;
        }
        MenuButton menuButton = buttons[buttonIndex];
        if (menuButton is not ItemButton){
            return;
        }
        ItemButton ib = menuButton as ItemButton;
        if (hoveredItem != null)
        {
            //SWAP ITEMS
            SwapWithHoveredItem(ib);
            return;
        }
        if (ib.GetItem() == null){
            return;
        }

        //TODO: ONLY HIGHLIGHT OBJECTS THAT CAN BE SWAPPED
        hoveredItem = ib.GetItem();
        //CHECK IF ANY UNITS CAN USE THIS ITEM
        bool noUnits = true;
        foreach (BaseUnit unit in UnitManager.instance.GetAllHeroes()){
            if (unit.CanUseSkill(hoveredItem) || unit.CanUseWeapon(hoveredItem)){
                noUnits = false;
                break;
            }
        }
        //IF NOT UNITS CAN USE IT, DO NOT SWAP MENUS
        if (noUnits && currentInventoryScreen == InventoryScreen.Items){
            buttonNameText.text = "No units found that can use that item!";
            hoveredItem = null;
            //Debug.Log("no units found that can use that item!");
            return;
        }
        hoveredItemButton.gameObject.SetActive(true);
        ChangeInventoryScreen();
        if (currentInventoryScreen == InventoryScreen.Units){
            hoveredItemButton.unit = ib.unit;
            //WHEN SWAPPING TO ITEMS
            if (hoveredItem is BaseWeapon){
                InventoryManager.instance.SortInventory(ib.unit, ItemType.Weapon);
                ResetItemButtons();
                hoveredItemButton.SetItem(hoveredItem, 3);
                HighlightWeapons(hoveredItem as BaseWeapon);
            }else{
                InventoryManager.instance.SortInventory(ib.unit, ItemType.Skill);
                ResetItemButtons();
                hoveredItemButton.SetItem(hoveredItem, ib.index);
                HighlightItemSkills(hoveredItem as BaseSkill);
            }
        }else{
            //WHEN SWAPPING TO UNITS
            hoveredItemButton.SetItem(hoveredItem);
            if (hoveredItem is BaseWeapon){
                HighlightWeapons(hoveredItem as BaseWeapon);
            }else{
                HighlightUnitSkills(hoveredItem as BaseSkill);
            }
        }
    }

    private void SwapWithHoveredItem(ItemButton ib)
    {
        BaseItem newItem;
        if (currentInventoryScreen == InventoryScreen.Units)
        {
            InventoryManager.instance.RemoveItem(hoveredItem);
            if (hoveredItem is BaseWeapon)
            {
                newItem = ib.unit.weapon;
                ib.unit.weapon = hoveredItem as BaseWeapon;
            }
            else
            {
                newItem = ib.unit.skills[ib.index];
                ib.unit.skills[ib.index] = hoveredItem as BaseSkill;
            }
            //ChangeInventoryScreen();
        }else{
            InventoryManager.instance.RemoveItem(ib.GetItem());
            if (hoveredItem is BaseWeapon)
            {
                newItem = hoveredItemButton.unit.weapon;
                hoveredItemButton.unit.weapon = ib.GetItem() as BaseWeapon;
            }
            else
            {
                newItem = hoveredItemButton.unit.GetSkill(hoveredItemButton.index);
                hoveredItemButton.unit.skills[hoveredItemButton.index] = ib.GetItem() as BaseSkill;
            }

        }
        if (newItem != null){
            InventoryManager.instance.AddItem(newItem);
        }
        UnhoverItem();
        int oldIndex = buttonIndex;
        ResetButtons();
        buttonIndex = oldIndex;
        SetHighlight();
    }
    private void HighlightWeapons(BaseWeapon hoveredWeapon){
        foreach (MenuButton mb in buttons){
            if (mb is not ItemButton){
                return;
            }
            ItemButton ib = mb as ItemButton;
            BaseItem item = ib.GetItem();
            if (item is BaseSkill){
                ib.SetOn(false);
                continue;
            }
            BaseWeapon weapon = item as BaseWeapon;
            if (ib.unit == null){
                // Debug.Log(ib.unit);
                // Debug.Log(hoveredWeapon);
                if (weapon != null && weapon.weaponClass == hoveredWeapon.weaponClass){
                    ib.SetOn(true);
                }else{
                    ib.SetOn(false);
                }
            }else {
                if (ib.unit.weaponClass == hoveredWeapon.weaponClass){
                    ib.SetOn(true);
                }else{
                    ib.SetOn(false);
                }
            }
        }
    }
    private void HighlightUnitSkills(BaseSkill hoveredSkill){
        foreach (MenuButton mb in buttons){
            if (mb is not ItemButton){
                return;
            }
            ItemButton ub = mb as ItemButton;
            BaseItem item = ub.GetItem();
            if (item is BaseWeapon){
                ub.SetOn(false);
                continue;
            }
            if (ub.unit.CanUseSkill(hoveredSkill)){
                ub.SetOn(true);
            }else{
                ub.SetOn(false);
            }
        }
    }
    private void HighlightItemSkills(BaseSkill hoveredSkill){
        foreach (MenuButton mb in buttons){
            if (mb is not ItemButton){
                return;
            }
            ItemButton ub = mb as ItemButton;
            BaseItem item = ub.GetItem();
            if (item is BaseWeapon){
                ub.SetOn(false);
                continue;
            }
            BaseSkill skill = item as BaseSkill;
            if (hoveredItemButton.unit.CanUseSkill(skill)){
                ub.SetOn(true);
            }else{
                ub.SetOn(false);
            }
        }
    }
    public void ChangeInventoryScreen(){
        ShowParagonSkills(false);
        if (currentInventoryScreen == InventoryScreen.Items){
            ShowUnits();
        }else{
            ShowItems();
        }
    }
    private void SetNameText(){
        buttonNameText.text = InventoryManager.instance.GetItemName(buttonIndex);
        MenuButton button = GetCurrentButton();
        if (button == null || button is not ItemButton){
            return;
        }
        ItemButton ib = button as ItemButton;
        descriptionMenu.SetItem(ib.item);
    }
    
    internal void ShowUnits()
    {
        if (menuMoving){
            return;
        }
        itemScreenNextPos += new Vector3(1700, 0);
        buttons = unitButtons;
        this.xCount = 3;
        this.yCount = UnitManager.instance.GetAllHeroes().Count;
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
        StartCoroutine(WhileMoving());
    }
    IEnumerator WhileMoving(){
        menuMoving = true;
        descriptionMenu.gameObject.SetActive(false);
        highlighImage.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        highlighImage.gameObject.SetActive(true);
        SetFirstIndex();
        SetHighlight();
        menuMoving = false;
        if (currentInventoryScreen == InventoryScreen.Units){
            currentInventoryScreen = InventoryScreen.Items;
        }else{
            currentInventoryScreen = InventoryScreen.Units;
        }
        Move(Vector2.zero);     
        yield return null;
    }

    internal void UnhoverItem()
    {
        hoveredItem = null;
        hoveredItemButton.Reset();
        ResetButtons();
    }

    internal void UnequipItem()
    {
        if (currentInventoryScreen == InventoryScreen.Items){
            return;
        }
        var menuButton = GetCurrentButton();
        if (menuButton is not ItemButton){
            return;
        }
        ItemButton ib = menuButton as ItemButton;

        var item = ib.GetItem();
        if (item == null){
            return;
        }
        InventoryManager.instance.AddItem(item);
        if (item is BaseWeapon){
            ib.unit.weapon = null;
        } else {
            ib.unit.skills[ib.index] = null;
        }
        ResetButtons();
    }

    internal void ToggleParagonSkills()
    {
        ShowParagonSkills(!paragonInfoScreen.activeSelf);
    }
}
    public enum InventoryScreen{
        Units,
        Items,
    }