using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseUnit : MonoBehaviour
{
    public string unitName;
    public Tile occupiedTile;
    public UnitFaction faction;
    public int moveAmount;
    public int health;
    public int maxHealth;

    public UnitHealthBar healthBar;

    void Awake(){
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
}
