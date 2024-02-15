using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;
    [SerializeField] private List<BaseItem> items;
    void Awake()
    {
        instance = this;
    }
    public BaseItem GetItem(int index){
        if (index >= items.Count){
            return null;
        }
        return items[index];
    }

    public string GetItemName(int index){
        if (index >= items.Count){
            return "<No Item>";
        }
        BaseItem item = items[index];
        if (item == null){
            return "<Null Item>";
        }
        if (item is BaseSkill){
            return (item as BaseSkill).skillName;
        }
        return (item as BaseWeapon).weaponName;
    }
    public int ItemCount(){
        return items.Count;
    }
    public void AddItem(BaseItem item){
        items.Add(item);
    }
    public void RemoveItem(BaseItem item){
        items.Remove(item);
    }


    public void SortInventory(BaseUnit unit, ItemType type)
    {
        //MOVE BUTTONS THAT ARE ON TO THE FRONT
        if (type == ItemType.Skill){
            items = items.OrderByDescending(i => unit.CanUseSkill(i)).ToList();
        }else{
            items = items.OrderByDescending(i => unit.CanUseWeapon(i)).ToList();
        }
    }
}
