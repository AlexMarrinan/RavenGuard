using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedUnit : BaseUnit
{

    public RangedWeapon weapon;

    public override int MaxTileRange(){
        return  weapon.maxRange - (base.maxMoveAmount - base.moveAmount);
    }
    public override TileMoveType GetMoveTypeAt(Tile otherTile)
    {
        int distance = otherTile.GetPathLengthFrom(base.occupiedTile);
        Debug.Log(distance);
        var tempType = TileMoveType.Move;
        if (distance < base.moveAmount){
            tempType = TileMoveType.Move;
        }else{
            tempType = TileMoveType.InAttackRange;
        }
        if (otherTile.occupiedUnit != null && otherTile.occupiedUnit.faction != TurnManager.instance.currentFaction){
            tempType = TileMoveType.Attack;
        }
        return tempType;
    }
    public override void Attack(BaseUnit otherUnit){
        otherUnit.ReceiveDamage(weapon.damage);
        if (otherUnit.health <= 0){
            UnitManager.instance.DeleteUnit(otherUnit);
            MoveToSelectedTile(otherUnit.occupiedTile);
        }else{
            MoveToClosestTile(otherUnit.occupiedTile);
        }
        OnExhaustMovment();
        UnitManager.instance.SetSeclectedUnit(null);
    }
}
