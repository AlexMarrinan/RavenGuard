using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
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
        //TODO: ADD ANIMATION TO SHOW UNIT GETTING ATTACKED
        BattleSceneManager.instance.StartBattle(this, otherUnit);
    }

    public override List<(BaseTile, TileMoveType)> GetValidAttacks(BaseTile tempTile){
        if (tempTile == null){
            return new();
        }
        var visited = new Dictionary<BaseTile, int>();

        //TODO: SHOULD START WITH START TILE, NOT STARTING ADJ TILES !!!
//        Debug.Log(occupiedTile);
//        Debug.Log(tempTile);
        var next = tempTile.GetAdjacentTiles();
//        Debug.Log(rangedWeapon.maxRange);
        next.ForEach(t => GVAHelper(1, this.rangedWeapon.maxRange, t, visited, t, this));
//        Debug.Log(visited.Count);
        List<(BaseTile, TileMoveType)> returns = new();
        foreach (BaseTile tile in visited.Keys){
            int distnace = GridManager.instance.ShortestPathBetweenTiles(tempTile, tile, false).Count();
//            Debug.Log(distnace);
            if (distnace >= rangedWeapon.minRange){
                SetAttackMove(tile, returns);
            }
        }
        List<BaseTile> adjTiles = GridManager.instance.GetAdjacentTiles(tempTile.coordiantes);
        foreach(var adj in adjTiles){
            var temp = (adj, TileMoveType.Attack);
            if (returns.Contains(temp)){
                returns.Remove(temp);
                continue;
            }
            var temp2 = (adj, TileMoveType.InAttackRange);
            if (returns.Contains(temp2)){
                returns.Remove(temp2);
            }
        }
        return returns;
    }

    private void GVAHelper(int depth, int max, BaseTile tile, Dictionary<BaseTile, int> visited, BaseTile startTile, BaseUnit startUnit){
        if (depth >= max){
            return;
        }
        if (tile == null){
            return;
        }
        // //enemy's are valid moves but block movement
        // if (tile != null && tile.occupiedUnit != null && tile.occupiedUnit.faction != startUnit.faction){
        //     visited[tile] = depth;
        //     return;
        // }
        // //if tile is not valid, continue
        // if (tile == null || !tile.walkable || (visited.ContainsKey(tile) && visited[tile] == depth)){
        //     return;
        // }

        //if tile is valid, add it to the list of visited tiles and continue
        visited[tile] = depth;
        var next = tile.GetAdjacentTiles();   
        next.ForEach(t => GVAHelper(depth + 1, max, t, visited, startTile, startUnit));
        return;
    }

    
    protected override void InitializeUnitClass(){
        if (this.rangedWeaponClass == WeaponRangedClasss.Archer){
            this.weaponClass = WeaponClass.Archer;
        }else{
            this.weaponClass = WeaponClass.Magic;
        }
        this.weapon = this.rangedWeapon;
    }
}
