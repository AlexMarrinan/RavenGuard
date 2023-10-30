using System;
using System.Collections.Generic;
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
    
    public void GhostShieldAS(BaseUnit u){
        Debug.Log("Used Ghost Shield...");
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