using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        MoveToAttackTile();
        BattleSceneManager.instance.StartBattle(this, otherUnit);
    }
}
