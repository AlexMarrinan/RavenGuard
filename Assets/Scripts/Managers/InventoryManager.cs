using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        if (item is BaseSkill){
            return (item as BaseSkill).skillName;
        }
        return (item as BaseWeapon).weaponName;
    }
    public int ItemCount(){
        return items.Count;
    }
}
