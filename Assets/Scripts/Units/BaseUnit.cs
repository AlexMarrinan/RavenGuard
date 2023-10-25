using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseUnit : MonoBehaviour
{
    public string unitName;
    public Tile occupiedTile;
    public UnitFaction faction;
    public UnitClass unitClass;
    private ArmorType armorType;
    /*** Unit Stats ***/
    public int moveAmount;
    public int maxMoveAmount;
    public int health;
    public int maxHealth;
    [SerializeField] private int attack;
    [SerializeField] private int defense;
    [SerializeField] private int agility;
    [SerializeField] private int attunment;
    [SerializeField] private int foresight;
    [SerializeField] private int luck;

    // [HideInInspector]
    public bool awaitingOrders;
    public UnitHealthBar healthBar;
    public SpriteRenderer spriteRenderer;
    public ActiveSkill activeSkill = null;
    public PassiveSkill universalPassiveSkill = null;
    public PassiveSkill classPassiveSkill = null;
 
    [HideInInspector] public BaseWeapon weapon;
    [HideInInspector] public WeaponClass weaponClass;
    public RuntimeAnimatorController animatorController;
    void Start(){
        RandomizeUnitClass();
        InitializeUnitClass();
        InitializeFaction();
        CreateHealthbar();
    }
    private void RandomizeUnitClass(){
        Array values = Enum.GetValues(typeof(UnitClass));
        var random = new System.Random();
        unitClass = (UnitClass)values.GetValue(random.Next(values.Length));
    }

    public virtual void ApplyWeapon(){

    }
    private void InitializeUnitClass()
    {
        switch (unitClass){
            case UnitClass.Infantry:
                armorType = ArmorType.Medium;
                break;
            case UnitClass.Knight:
                armorType = ArmorType.Heavy;
                break;
            case UnitClass.Assassin:
                armorType = ArmorType.Light;
                break;
           case UnitClass.Mage:
                armorType = ArmorType.Medium;
                break;
            case UnitClass.Sage:
                armorType = ArmorType.Light;
                break;
            case UnitClass.Cavalry:
                armorType = ArmorType.Medium;
                break;
            case UnitClass.Paladin:
                armorType = ArmorType.Heavy;
                break;
        }
    }

    public virtual int MaxTileRange(){
        return 0;
    }
    private void InitializeFaction(){
        if (this.faction == UnitFaction.Hero){
            spriteRenderer.color = Color.cyan;
        }else{
            spriteRenderer.color = Color.red;
        }
    }
    private void CreateHealthbar(){
        var prefab = Resources.Load("HealthBar");
        var go = (GameObject)Instantiate(prefab);
        ((GameObject)go).transform.SetParent(GameManager.instance.mainCanvas.transform);
        healthBar = go.GetComponent<UnitHealthBar>();
        healthBar.SetAttachedUnit(this);
    }
    public virtual void Attack(BaseUnit otherUnit){
        return;
    }
    public virtual void Heal(BaseUnit otherUnit){
        return;
    }
    public void ReceiveDamage(BaseUnit otherUnit){
        int damage = otherUnit.GetAttack() - this.GetDefense();
        if (damage <= 0){
            return;
        }
        health -= damage;
        //healthBar.RenderHealth();
    }
    public void MoveToSelectedTile(Tile selectedTile){
        selectedTile.SetUnit(UnitManager.instance.selectedUnit);
        //healthBar.RenderHealth();
    }
    public void MoveToClosestTile(Tile selectedTile){
        Tile adjTile = PathLine.instance.GetLastTile();
        adjTile.SetUnit(UnitManager.instance.selectedUnit);
        //healthBar.RenderHealth();
    }
    public void MoveToTileAtDistance(int distance){
        Tile adjTile = PathLine.instance.GetPathTile(distance);
        adjTile.SetUnit(UnitManager.instance.selectedUnit);
        //healthBar.RenderHealth();
    }
    public void ResetMovment(){
        InitializeFaction();
        moveAmount = maxMoveAmount;
    }

    public void OnExhaustMovment(){
        moveAmount = 0;
        spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f);
        UnitManager.instance.SetSeclectedUnit(null);
        TurnManager.instance.OnUnitDone(this);
    }

    public virtual TileMoveType GetMoveTypeAt(Tile otherTile){
        return TileMoveType.NotValid;
    }  

    #region Stat Getters

    // TODO: ADD STAT BONUS CALCULATIONS !!!
    // XP, LEVELUPS, SKILLS, ETC.
    public int GetAttack(){
        return attack; //+ weapon.damage; 
    }
    public int GetDefense(){
        return defense;
    }
    public int GetAgility(){
        return agility;
    }
    public int GetAttuenment(){
        return attunment + weapon.damage; 
    }
    public int GetForesight(){
        return foresight; 
    }
    public int GetLuck(){
        return luck; 
    }
    #endregion


    public Sprite GetSprite(){
        return spriteRenderer.sprite;
    }
    public Color GetColor(){
        return spriteRenderer.color;
    }
    // public void EquipSkill(BaseSkill newSkill){
    //     equippedSkill = newSkill;
    // }
    // public BaseSkill GetEquipedSkill(){
    //     return equippedSkill;
    // }
    // public void UnequipSkill(){
    //     equippedSkill = null;
    // }
}
public enum UnitStatType {
    Health,
    Attack,
    Defense,
    Agility,
    Attunment,
    Foresight,
    Luck,
}

public enum UnitClass {
    Infantry,
    Knight,
    Assassin,
    Mage,
    Sage,
    Cavalry,
    Paladin
}

public enum ArmorType {
    Light,
    Medium,
    Heavy
}