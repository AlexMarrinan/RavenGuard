using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MeleeUnit : BaseUnit
{
    public MeleeWeapon meleeWeapon;
    [SerializeField] private WeaponMeleeClass meleeWeaponClass;
    private void Awake() {
        ApplyWeapon();
        if (meleeWeaponClass == WeaponMeleeClass.SideArms){
            base.weaponClass = WeaponClass.SideArms;
        }else{
            base.weaponClass = WeaponClass.LongArms;
        }
    }
    public override void ApplyWeapon()
    {
        base.weapon = meleeWeapon;
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
        if (otherTile.moveType != TileMoveType.NotValid){
            return otherTile.moveType;
        }
        if (otherTile.occupiedUnit != null && otherTile.occupiedUnit.faction != TurnManager.instance.currentFaction){
            return TileMoveType.Attack;
        }
        return TileMoveType.Move;
    }

    
    public override void Attack(BaseUnit otherUnit){
        MoveToAttackTile(otherUnit);
        BattleSceneManager.instance.StartBattle(this, otherUnit);
    }

    public override List<(BaseTile, TileMoveType)> GetValidAttacks()
    {
        List<BaseTile> tiles = GridManager.instance.GetAdjacentTiles(occupiedTile.coordiantes);
        List<(BaseTile, TileMoveType)> returns = new();
        foreach (BaseTile tile in tiles){
            SetAttackMove(tile, returns);
        }
        return returns;
    }

    protected override void InitializeUnitClass(){
        if (this.meleeWeaponClass == WeaponMeleeClass.LongArms){
            this.weaponClass = WeaponClass.LongArms;
        }else{
            this.weaponClass = WeaponClass.SideArms;
        }
    }
}
