using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SkillManager : MonoBehaviour
{
    public Vector2 useDirection = Vector2.right;
    public static SkillManager instance;
    public bool selectingSkill = false;
    public bool skillFailed = false;
    public List<BaseTile> currentTiles;
    public BaseUnit user;
    public BaseSkill currentSkill;
    public Color activeSkillColor, passiveSkillColor;
    public void Awake(){
        instance = this;
    }

    public void ShowSkillPreview(){
        UnitManager.instance.RemoveAllValidMoves();
        selectingSkill = true;
        // foreach (BaseTile t in currentTiles){
        //     t.SetPossibleMove(false, user.occupiedTile);
        // }
        currentTiles = currentSkill.GetAffectedTiles(user);
        // if (currentSkill is ActiveSkill){
        //     if ((currentSkill as ActiveSkill).activeSkillType == ActiveSkillType.OnUnit){
        //         bool found = false;
        //         foreach(BaseTile tile in currentTiles){
        //             if (tile.occupiedUnit != null){
        //                 if (tile.occupiedUnit.faction != user.faction){
        //                     found = true;
        //                     break;
        //                 }
        //             }
        //         }
        //         if (!found){
        //             return;
        //         }
        //     }
        // }
        UnitManager.instance.RemoveAllValidMoves();

        foreach (BaseTile t in currentTiles){
            if (t == user.occupiedTile){
                continue;
            }
            t.moveType = currentSkill.tileMoveType;
            t.SetPossibleMove(true, user.occupiedTile);
        }
    }


    public void SwitchAS(BaseUnit u){
        var tiles = SkillManager.instance.currentTiles;
        BaseUnit switchUnit = null;
        foreach (BaseTile tile in tiles){
            if (tile.occupiedUnit != null && tile.occupiedUnit.faction == u.faction){
                switchUnit = tile.occupiedUnit;
                break;
            }
        }  
        if (switchUnit == null){
            skillFailed = true;
            return;
        }
        Debug.Log("switching...");
        BaseTile uTile = u.occupiedTile;
        BaseTile switchTile = switchUnit.occupiedTile;

        switchTile.occupiedUnit = u;
        u.occupiedTile = switchTile;
        u.transform.position = switchTile.transform.position;

        uTile.occupiedUnit = switchUnit;
        switchUnit.occupiedTile = uTile;
        switchUnit.transform.position = uTile.transform.position;
    }
    public void WhirlwindAS(BaseUnit u){
        int damage = 3;
        Debug.Log("Used Whirlwind...");
        var tiles = SkillManager.instance.currentTiles;
        foreach (BaseTile tile in tiles){
            BaseUnit unit = tile.occupiedUnit;
            if (unit != null && unit != u){
                unit.ReceiveDamage(damage);
            }
        }
    }
    //Upon ending unit's action or end of combat, adjacent allies are cleansed of all clearable debuffs.
    public void CleansePS(BaseUnit u){
        u.Cleanse();
        u.GetAdjacentUnits().ForEach(u => u.Cleanse());
    }
    public void CripplingAimPS(BaseUnit u){
        BattleUnit bu = BattleSceneManager.instance.GetBattleUnit(u);
        if (bu.damageDealt >= 10){
            BattleUnit otherBU = BattleSceneManager.instance.GetOtherBattleUnit(u);
            if (otherBU.assignedUnit != null){
                otherBU.assignedUnit.AddBuff(new CrippledBuff(u, otherBU.assignedUnit));
            }
        }
    }
    public void CrossfirePS(BaseUnit u){
        BattleUnit bu = BattleSceneManager.instance.GetBattleUnit(u);
        int damage = bu.damageDealt * 1/4;
        BattleUnit otherbu = BattleSceneManager.instance.GetOtherBattleUnit(u);

        var units = otherbu.assignedUnit.GetAdjacentUnits();
        units.ForEach(unit => unit.ReceiveDamage(damage));
    }

    public void AdrenalineBurstPS(BaseUnit u){
//      Adrenaline Burst should not be able to kill its unit
        if (u.health <= 2){
            u.health = 1;
        }else{
            u.ReceiveDamage(2);
        }
    }
    public void PoisonArrowsPS(BaseUnit u){
        BattleUnit otherbu = BattleSceneManager.instance.GetOtherBattleUnit(u);
        otherbu.assignedUnit.AddBuff(new PoisonBuff(u, otherbu.assignedUnit, 2));
    }
    //If unit moves 0 or 1 spaces, unit gains +2 defense and +1 attack during combat.
    //This effect stack for each consecutive turn in a row, up to three turns. 
    //Then, stays at 3 turns. Moving more than the amount of spaces resets the stats.
    public void HunkerDownPS(BaseUnit u){
        //Called After Movment
//        Debug.Log("hunkering down...");
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
        int healing = 4;
        //Called After Movment
        if (BattleSceneManager.instance.UnitAttacked(u)){
            u.RecoverHealth(healing);
        }
    }   
    public void CripplingJealousyPS (BaseUnit u){
        var units = UnitManager.instance.GetAllUnits();
        var coords = u.occupiedTile.coordiantes;
        //Debug.Log("searching for jealousy");

        foreach (var unit in units){
            if (u != unit && (coords.x == unit.occupiedTile.coordiantes.x || coords.y == unit.occupiedTile.coordiantes.y)){
                if (u.GetForesight().total > unit.GetForesight().total){
                    //Debug.Log("Applying jealousy");
                    unit.AddBuff(new JelousBuff(u, unit));
                }
            }
        }
    }
    public void WeakpointManipulationPS(BaseUnit u){
        var units = u.GetAdjacentUnits();
        foreach (BaseUnit unit in units){
            if (unit.faction == u.faction && (unit.weaponClass == WeaponClass.Magic || unit.unitClass == UnitClass.Mage)){
                u.AddBuff(new DualityBuff(u, u));
                return;
            }
        }
    }
    internal void Move(Vector2 moveVector)
    {
        AudioManager.instance.PlayMove();
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
        AudioManager.instance.PlayConfirm();
        currentSkill.OnUse(user);
    }
    internal void OnSkilEnd()
    {
        selectingSkill = false;
        currentTiles = new List<BaseTile>();
        UnitManager.instance.UnselectUnit();
    }
}