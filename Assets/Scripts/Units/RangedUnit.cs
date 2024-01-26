using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedUnit : BaseUnit
{
    public RangedWeapon rangedWeapon;
    [SerializeField] private WeaponRangedClasss rangedWeaponClass;
    private void Awake() {
        this.ApplyWeapon();
        base.weapon = rangedWeapon;
        if (rangedWeaponClass == WeaponRangedClasss.Archer){
            base.weaponClass = WeaponClass.Archer;
        }else{
            base.weaponClass = WeaponClass.Magic;
        }
    }
    public override void ApplyWeapon()
    {
        base.weapon = rangedWeapon;
    }
    public override int MaxTileRange(){
        if (reducedMovment != 0){
            int newMove = moveAmount - reducedMovment;
            return newMove < 1 ? 1 : newMove;
        }
        return moveAmount;
    }
    public override TileMoveType GetMoveTypeAt(BaseTile otherTile)
    {
        // int distance = otherTile.DistanceFrom(base.occupiedTile);
        // TileMoveType tempType;
        // if (distance < base.moveAmount){
        //     tempType = TileMoveType.Move;
        // }else{
        //     tempType = TileMoveType.InAttackRange;
        // }
        if (otherTile.occupiedUnit != null && otherTile.occupiedUnit.faction != TurnManager.instance.currentFaction){
            return TileMoveType.NotValid;
        }
        return TileMoveType.Move;
    }
    public override void Attack(BaseUnit otherUnit){
        // int distance = otherUnit.occupiedTile.GetPathLengthFrom(base.occupiedTile);

        // if (distance >= base.moveAmount){
        //     base.MoveToTileAtDistance(distance - base.moveAmount);
        // }
        MoveToAttackTile(otherUnit);
        BattleSceneManager.instance.StartBattle(this, otherUnit);
    }
}
