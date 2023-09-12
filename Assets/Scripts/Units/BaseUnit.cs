using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseUnit : MonoBehaviour
{
    public string unitName;
    public Tile occupiedTile;
    public UnitFaction faction;
    public int moveAmount;
    public int maxMoveAmount;
    // [HideInInspector]
    public int health;
    public int maxHealth;
    private int agility;
    private int defense;

    // [HideInInspector]
    public bool awaitingOrders;
    public UnitHealthBar healthBar;
    private SpriteRenderer spriteRenderer;
    public ActiveSkill activeSkill = null;
    public PassiveSkill universalPassiveSkill = null;
    public PassiveSkill classPassiveSkill = null;

    void Awake(){
        spriteRenderer = GetComponent<SpriteRenderer>();
        InitializeFaction();
        CreateHealthbar();
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
    public void ReceiveDamage(int damage){
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

    public int GetAgility(){
        return agility; //TODO: ADD STAT BONUS CALCULATIONS
    }
    public int GetDefense(){
        return defense; //TODO: ADD STAT BONUS CALCULATIONS
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
public enum UnitClassType {
    Melee,
    Ranged,
}