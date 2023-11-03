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
    
    //Upon ending unit's action or end of combat, adjacent allies are cleansed of all clearable debuffs.
    public void CleansePS(BaseUnit u){
        u.Cleanse();
        u.GetAdjacentUnits().ForEach(u => u.Cleanse());
    }

    //If unit moves 0 or 1 spaces, unit gains +2 defense and +1 attack during combat.
    //This effect stack for each consecutive turn in a row, up to three turns. 
    //Then, stays at 3 turns. Moving more than the amount of spaces resets the stats.
    public void HunkerDownPS(BaseUnit u){
        //Called After Movment
        var defStats = u.GetStatChange("HunkerDownDef");
        if (defStats == null){
            u.AddStatsChange("HunkerDownDef", UnitStatType.Defense, 0, 0, 6);
        }
        var atkStats = u.GetStatChange("HunkerDownAttack");
        if (atkStats == null){
            u.AddStatsChange("HunkerDownAttack", UnitStatType.Attack, 0, 0, 3);
        }
        if (u.moveAmount <= 1){
            u.IncrementStatsChange("HunkerDownDef", + 2);
            u.IncrementStatsChange("HunkerDownAttack", + 1);
        }else{
            u.SetStatChange("HunkerDownDef", 0);
            u.SetStatChange("HunkerDownAttack", 0);

        }
    }   


    //Unit heals 7 HP after combat if unit attacked.
    public void RecoveryPS(BaseUnit u){
        int healing = 7;
        //Called After Movment
        if (BattleSceneManager.instance.UnitAttacked(u)){
            u.RecoverHealth(healing);
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