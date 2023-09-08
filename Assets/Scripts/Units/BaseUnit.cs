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
    // [HideInInspector]
    public bool awaitingOrders;
    public UnitHealthBar healthBar;
    private SpriteRenderer spriteRenderer;

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
        healthBar.RenderHealth();
    }
    public void MoveToSelectedTile(Tile selectedTile){
        selectedTile.SetUnit(UnitManager.instance.selectedUnit);
        healthBar.RenderHealth();
    }
    public void MoveToClosestTile(Tile selectedTile){
        Tile adjTile = PathLine.instance.GetSecondLastTile();
        adjTile.SetUnit(UnitManager.instance.selectedUnit);
        healthBar.RenderHealth();
    }

    public void ResetMovment(){
        InitializeFaction();
        moveAmount = maxMoveAmount;
    }

    public void OnExhaustMovment(){
        moveAmount = 0;
        spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f);
        UnitManager.instance.SetSeclectedUnit(null);
        TurnManager.instance.GetNextHero(this);
    }

    public virtual TileMoveType GetMoveTypeAt(Tile otherTile){
        return TileMoveType.NotValid;
    }
}



