using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager instance;
    private List<ScriptableUnit> units;
    public BaseUnit selectedUnit;
    void Awake(){
        instance = this;
        units = Resources.LoadAll<ScriptableUnit>("Units").ToList();
    }

    public void SpawnHeroes(){
        var heroCount = 5;
        for (int i = 0; i < heroCount; i++){
            var randomPrefab = GetRandomUnit<BaseHero>(UnitFaction.Hero);
            var spawnedHero = Instantiate(randomPrefab);
            var randomSpawnTile = GridManager.instance.GetHeroSpawnTile();
            randomSpawnTile.SetUnit(spawnedHero);
            
            //TODO: REMOVE AFTER PROVING LINE SHOWS UP
            //PathLine.instance.AddTile(randomSpawnTile);
        }
        GameManager.instance.ChangeState(GameState.SpawnEnemies);
    }

    public void SpawnEnemies(){
        var enemyCount = 5;
        for (int i = 0; i < enemyCount; i++){
            var randomPrefab = GetRandomUnit<BaseEnemy>(UnitFaction.Enemy);
            var spawnedEnemy = Instantiate(randomPrefab);
            var randomSpawnTile = GridManager.instance.GetEnemySpawnTile();
            randomSpawnTile.SetUnit(spawnedEnemy);
        }
        GameManager.instance.ChangeState(GameState.HeroesTurn);
    }
    private T GetRandomUnit<T>(UnitFaction faction) where T : BaseUnit{
        return (T) units.Where(u => u.faction == faction).OrderBy(o => Random.value).First().unitPrefab;
    }
    public void DeleteUnit(BaseUnit unit){
        Debug.Log("Deleted " + unit);
        Object.Destroy(unit.healthBar.gameObject);
        Object.Destroy(unit.gameObject);
    }
    public void SetSeclectedUnit(BaseUnit unit){
        if (unit == null){
            UnselectUnit();
            return;
        }
        MenuManager.instance.SelectTile(unit.occupiedTile);
        selectedUnit = unit;
        RemoveAllValidMoves();
        SetValidMovesBetter(unit);
        PathLine.instance.Reset();
        PathLine.instance.AddTile(unit.occupiedTile);
        //MenuManager.instance.ShowSelectedUnit(unit);
    }

    public void UnselectUnit(){
        MenuManager.instance.UnselectTile();
        selectedUnit = null;
        RemoveAllValidMoves();
    }

    public void SetValidMovesBetter(BaseUnit unit){
        int move = unit.moveAmount;
        Tile tile = unit.occupiedTile;
        var visited = new Dictionary<Tile, int>();
        var next = tile.GetAdjacentCoords();   
        next.ForEach(t => SVMHelper(1, move, t, visited));
        visited.Keys.ToList().ForEach(t => t.SetPossibleMove(true));
    }

    private void SVMHelper(int depth, int max, Tile tile, Dictionary<Tile, int> visited){

        //enemy's are valid moves but block attacks
        if (tile != null && tile.occupiedUnit != null && tile.occupiedUnit.faction == UnitFaction.Enemy){
            visited[tile] = depth;
            return;
        }
        //if tile is not valid, continue
        if (tile == null || !tile.walkable || depth >= max || (visited.ContainsKey(tile) && visited[tile] == depth)){
            return;
        }

        //if tile is valid, add it to the list of visited tiles and continue
        visited[tile] = depth;
        var next = tile.GetAdjacentCoords();   
        next.ForEach(t => SVMHelper(depth + 1, max, t, visited));
        return;
    }

    public void RemoveAllValidMoves(){
        GridManager.instance.GetAllTiles().ForEach(t => t.SetPossibleMove(false));
    }
}
