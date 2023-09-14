using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeUnit : BaseUnit
{
    public MeleeWeapon meleeWeapon;

    private void Awake() {
        base.weapon = meleeWeapon;
    }
    public override int MaxTileRange(){
        return base.moveAmount;
    }

    public override TileMoveType GetMoveTypeAt(Tile otherTile)
    {
        if (otherTile.occupiedUnit != null && otherTile.occupiedUnit.faction != TurnManager.instance.currentFaction){
            return TileMoveType.Attack;
        }
        return TileMoveType.Move;
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
    }
}
