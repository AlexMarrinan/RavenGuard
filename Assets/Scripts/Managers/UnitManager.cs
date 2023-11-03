using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager instance;
    private List<ScriptableUnit> unitPrefabs;
    private List<BaseUnit> units;
    public BaseUnit selectedUnit;
    public float unitMoveSpeed = .1f;
    void Awake(){
        instance = this;
        units = new List<BaseUnit>();
        unitPrefabs = Resources.LoadAll<ScriptableUnit>("Units").ToList();
    }

    public void SpawnHeroes(){
        var heroCount = 5;
        for (int i = 0; i < heroCount; i++){
            var randomPrefab = GetRandomUnit(UnitFaction.Hero);
            var spawnedHero = Instantiate(randomPrefab);
            units.Add(spawnedHero);
            var randomSpawnTile = GridManager.instance.GetHeroSpawnTile();
            randomSpawnTile.SetUnitStart(spawnedHero);
            
            //TODO: REMOVE AFTER PROVING LINE SHOWS UP
            //PathLine.instance.AddTile(randomSpawnTile);
        }
        GameManager.instance.ChangeState(GameState.SpawnEnemies);
    }

    public void SpawnEnemies(){
        var enemyCount = 5;
        for (int i = 0; i < enemyCount; i++){
            var randomPrefab = GetRandomUnit(UnitFaction.Enemy);
            var spawnedEnemy = Instantiate(randomPrefab);
            units.Add(spawnedEnemy);
            var randomSpawnTile = GridManager.instance.GetEnemySpawnTile();
            randomSpawnTile.SetUnitStart(spawnedEnemy);
        }
        GameManager.instance.ChangeState(GameState.HeroesTurn);
    }
    private BaseUnit GetRandomUnit(UnitFaction faction){
        var unit = unitPrefabs.OrderBy(o => Random.value).First().unitPrefab;
        unit.faction = faction;
        return unit;
    }
    public void DeleteUnit(BaseUnit unit){
        units.Remove(unit);
        Object.Destroy(unit.healthBar.gameObject);
        Object.Destroy(unit.gameObject);
        if (GetAllEnemies().Count <= 0){
            MenuManager.instance.ShowStartText("YOU WIN!", true);
            return;
        }
        if (GetAllHeroes().Count <= 0){
            MenuManager.instance.ShowStartText("GAME OVER", true);
            return;
        }
    }
    public void SetSeclectedUnit(BaseUnit unit){
        if (unit == null){
            UnselectUnit();
            return;
        }
        MenuManager.instance.SelectTile(unit.occupiedTile);
        selectedUnit = unit;
        RemoveAllValidMoves();
        SetValidMoves(unit);
        PathLine.instance.Reset();
        //MenuManager.instance.ShowSelectedUnit(unit);
    }

    public void UnselectUnit(){
        MenuManager.instance.UnselectTile();
        selectedUnit = null;
        PathLine.instance.Reset();
        RemoveAllValidMoves();
    }
    private List<BaseUnit> GetAllUnitsOfFaction(UnitFaction faction){
        return units.Where(u => u.faction == faction).ToList();
    }
    public List<BaseUnit> GetAllUnits(){
        var heroes = GetAllHeroes();
        var enemies = GetAllEnemies();
        return heroes.Concat(enemies).ToList();
    }
    public List<BaseUnit> GetAllHeroes(){
        return GetAllUnitsOfFaction(UnitFaction.Hero);
    }
    public List<BaseUnit> GetAllEnemies(){
        return GetAllUnitsOfFaction(UnitFaction.Enemy);
    }
    public List<Tile> SetValidMoves(BaseUnit unit){
        int max = unit.MaxTileRange();
        Tile tile = unit.occupiedTile;
        var visited = new Dictionary<Tile, int>();
        var next = tile.GetAdjacentTiles();
        next.ForEach(t => SVMHelper(1, max, t, visited, t, unit));
        var validMoves = visited.Keys.ToList();
        validMoves.ForEach(t => t.SetPossibleMove(true, unit.occupiedTile));
        return validMoves;
    }

    private void SVMHelper(int depth, int max, Tile tile, Dictionary<Tile, int> visited, Tile startTile, BaseUnit startUnit){
        if (depth >= max ){
            return;
        }
        //enemy's are valid moves but block movement
        if (tile != null && tile.occupiedUnit != null && tile.occupiedUnit.faction != startUnit.faction){
            visited[tile] = depth;
            return;
        }
        //if tile is not valid, continue
        if (tile == null || !tile.walkable || (visited.ContainsKey(tile) && visited[tile] == depth)){
            return;
        }

        //if tile is valid, add it to the list of visited tiles and continue
        visited[tile] = depth;
        var next = tile.GetAdjacentTiles();   
        next.ForEach(t => SVMHelper(depth + 1, max, t, visited, startTile, startUnit));
        return;
    }

    public void RemoveAllValidMoves(){
        GridManager.instance.GetAllTiles().ForEach(t => t.SetPossibleMove(false, null));
    }

    public void ResetUnitMovment(){
        foreach (BaseUnit unit in units){
            unit.ResetMovment();
        }
    }

    public void ShowUnitHealthbars(bool show){
        foreach (BaseUnit u in GetAllUnits()){
            u.healthBar.gameObject.SetActive(show);
        }
    }
    public IEnumerator AnimateUnitMove(BaseUnit unit, List<Tile> path, bool turnOver){
        Debug.Log(path.Count);
        Tile nextTile = path[0];
        Vector3 nextPos = nextTile.transform.position;
        float elapsedTime = 0;
        while (unit.transform.position != nextPos){
            unit.transform.position = Vector3.Lerp(unit.transform.position, nextPos, elapsedTime/unitMoveSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        path.RemoveAt(0);
        if (path.Count > 0){
            yield return AnimateUnitMove(unit, path, turnOver);
        }else{
            nextTile.occupiedUnit = unit;
            unit.occupiedTile = nextTile;
            if (turnOver){
                unit.OnExhaustMovment();
            }
            yield return new WaitForSeconds(0.45f);
        }
    }


    internal void OnTurnEndSkills(BaseUnit unit){
        var units = GetAllUnitsOfFaction(unit.faction);
        foreach (BaseUnit u in units){
            u.UsePassiveSkills(PassiveSkillType.OnTurnEnd);
        }
    }
}
