using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public Vector2 useDirection = Vector2.right;
    public static SkillManager instance;
    public bool selectingSkill = false;
    public List<Tile> currentTiles;
    public BaseUnit user;
    public BaseSkill currentSkill;
    public void Awake(){
        instance = this;
    }

    public void ShowSkillPreview(){
        UnitManager.instance.RemoveAllValidMoves();
        selectingSkill = true;
        foreach (Tile t in currentTiles){
            t.SetPossibleMove(false, user.occupiedTile);
        }
        currentTiles = currentSkill.GetAffectedTiles(user);
        foreach (Tile t in currentTiles){
            t.SetPossibleMove(true, user.occupiedTile);
        }
    }


    public void EarthQuakeAS(BaseUnit u){
        int damage = 3;
        Debug.Log("Used Earthquake...");
        var tiles = SkillManager.instance.currentTiles;
        foreach (Tile tile in tiles){
            BaseUnit unit = tile.occupiedUnit;
            if (unit != null && unit.faction == UnitFaction.Enemy){
                unit.ReceiveDamage(damage);
            }
        }
    }
    
    
    public void RushdownPS(BaseUnit u){
        int damage = 3;
        Debug.Log("Used Earthquake...");
        var tiles = SkillManager.instance.currentTiles;
        foreach (Tile tile in tiles){
            BaseUnit unit = tile.occupiedUnit;
            if (unit != null && unit.faction == UnitFaction.Enemy){
                unit.ReceiveDamage(damage);
            }
        }
    }
    public void GhostShieldAS(BaseUnit u){
        Debug.Log("Used Ghost Shield...");
    }

    //If unit moves 0 or 1 spaces, unit gains +2 defense and +1 attack during combat.
    //This effect stack for each consecutive turn in a row, up to three turns. 
    //Then, stays at 3 turns. Moving more than the amount of spaces resets the stats.
    public void HunkerDownPS(BaseUnit u){
        //Called After Movment
        var stats = u.GetStatChange("HunkerDown");
        if (stats == null){
            u.AddStatsChange("HunkerDown", UnitStatType.Defense, 0, 0, 3);
        }
        if (u.moveAmount <= 1){
            u.IncrementStatsChange("HunkerDown", + 1);
        }else{
            u.SetStatChange("HunkerDown", 0);
        }
    }   


    internal void Move(Vector2 moveVector)
    {
        if (moveVector.x != 0 && moveVector.y != 0){
            return;
        }
        if (moveVector.x == 0 && moveVector.y == 0){
            return;
        }
        useDirection = moveVector;
        ShowSkillPreview();
    }

    internal void Select()
    {
        currentSkill.OnUse(user);
    }
    internal void OnSkilEnd()
    {
        selectingSkill = false;
        currentTiles = new List<Tile>();
        UnitManager.instance.UnselectUnit();
    }
}