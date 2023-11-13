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
        return  rangedWeapon.maxRange - (base.maxMoveAmount - base.moveAmount);
    }
    public override TileMoveType GetMoveTypeAt(BaseTile otherTile)
    {
        int distance = otherTile.GetPathLengthFrom(base.occupiedTile);
        TileMoveType tempType;
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
        otherUnit.ReceiveDamage(this);
        int distance = otherUnit.occupiedTile.GetPathLengthFrom(base.occupiedTile);

        if (otherUnit.health <= 0){
            UnitManager.instance.DeleteUnit(otherUnit);
        }
        if (distance >= base.moveAmount){
            base.MoveToTileAtDistance(distance - base.moveAmount);
        }
        OnExhaustMovment();
        UnitManager.instance.SetSeclectedUnit(null);
        // yield return null;
    }
}
