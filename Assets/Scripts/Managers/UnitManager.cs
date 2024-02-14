using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class UnitManager : MonoBehaviour
{
    public static UnitManager instance;
    private List<ScriptableUnit> unitPrefabs;
    private List<BaseUnit> units;
    public BaseUnit selectedUnit;
    public List<UnitDot> heroDots, enemyDots;
    public GameObject heroDotHighlight, enemyDotHighlight;
    public float unitMoveSpeed = .1f;
    private bool team1heros = false;
    void Awake(){
        instance = this;
        units = new List<BaseUnit>();
        unitPrefabs = Resources.LoadAll<ScriptableUnit>("Units").ToList();
    }

    public void SpawnHeroes(){
        //returns 0 or 1
        team1heros = 0 == Random.Range(0, 2);
        if (GetAllHeroes().Count <= 0){
            var heroCount = 5;
            for (int i = 0; i < heroCount; i++){
                var randomSpawnTile = GridManager.instance.GetSpawnTile(team1heros);
                var randomPrefab = GetRandomUnit(UnitFaction.Hero, randomSpawnTile.Item2);
                var spawnedHero = Instantiate(randomPrefab, this.transform);
                units.Add(spawnedHero);
                randomSpawnTile.Item1.SetUnitStart(spawnedHero);
                spawnedHero.SetSkillMethods();
                SetDot(spawnedHero, i, UnitFaction.Hero);
            }
        }else{
            foreach (BaseUnit hero in GetAllHeroes()){
                var randomSpawnTile = GridManager.instance.GetSpawnTile(team1heros);
//                Debug.Log(randomSpawnTile.Item1.transform.position);
                randomSpawnTile.Item1.SetUnitStart(hero);
                hero.transform.position = randomSpawnTile.Item1.transform.position;
            }
        }
        GameManager.instance.ChangeState(GameState.SpawnEnemies);
    }

    public void SpawnEnemies(){
        var enemyCount = 2;
        for (int i = 0; i < enemyCount; i++){
            var randomSpawnTile = GridManager.instance.GetSpawnTile(!team1heros);
            var randomPrefab = GetRandomUnit(UnitFaction.Enemy, randomSpawnTile.Item2);
            var spawnedEnemy = Instantiate(randomPrefab);
            units.Add(spawnedEnemy);
            randomSpawnTile.Item1.SetUnitStart(spawnedEnemy);
            spawnedEnemy.SetSkillMethods();
            SetDot(spawnedEnemy, i, UnitFaction.Enemy);
        }
        GameManager.instance.ChangeState(GameState.HeroesTurn);
    }
    private BaseUnit GetRandomUnit(UnitFaction faction, UnitSpawnType spawnType)
    {
        var units = unitPrefabs.OrderBy(o => Random.value);
        var unit = units.First().unitPrefab;
        if (spawnType == UnitSpawnType.Ranged){
            unit = units.Where(u => u.unitPrefab is RangedUnit).First().unitPrefab;
        }else if (spawnType == UnitSpawnType.Melee){
            unit = units.Where(u => u.unitPrefab is MeleeUnit).First().unitPrefab;
        }

        //TODO: MAKE AI USE RANGED UNITS TOO
        if (faction == UnitFaction.Enemy){
            unit = units.Where(u => u.unitPrefab is MeleeUnit).First().unitPrefab;
        }
        unit.faction = faction;
        return unit;
    }
    public void DeleteUnit(BaseUnit unit, bool killed = true){
        units.Remove(unit);
        if (unit.uiDot != null){
            unit.uiDot.SetColor(Color.gray);
        }
        Object.Destroy(unit.healthBar.gameObject);
        Object.Destroy(unit.gameObject);
        if (GetAllEnemies().Count <= 0 && killed){
            MenuManager.instance.ShowStartText("LEVEL COMPLETE!", false);
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

        if (unit.faction == UnitFaction.Hero){
            AudioManager.instance.PlaySelect();
        }
        MenuManager.instance.SelectTile(unit.occupiedTile);
        selectedUnit = unit;
        RemoveAllValidMoves();
        if (unit.hasMoved){
            // UnselectUnit();
            return;
        }
        SetValidMoves(unit);
        PathLine.instance.Reset();
        //MenuManager.instance.ShowSelectedUnit(unit);
    }

    public void UnselectUnit(){
        if (selectedUnit == null){
            return;
        }
        MenuManager.instance.UnselectTile();
        selectedUnit = null;
        PathLine.instance.Reset();
        MenuManager.instance.otherUnitStatsMenu.gameObject.SetActive(false);
        if (GridManager.instance.hoveredTile.occupiedUnit == null){
            MenuManager.instance.unitStatsMenu.gameObject.SetActive(false);
        }else{
            MenuManager.instance.unitStatsMenu.SetUnit(GridManager.instance.hoveredTile.occupiedUnit);
        }
        RemoveAllValidMoves();
    }
    private List<BaseUnit> GetAllUnitsOfFaction(UnitFaction faction){
        var wantedUnits = units.Where(u => u.faction == faction).ToList();
        // Debug.Log(faction + ": " + wantedUnits.Count);
        return wantedUnits;
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
    public List<BaseTile> SetValidMoves(BaseUnit unit){
        var validMoves = GetValidMoves(unit);
        validMoves.ForEach(t => t.SetPossibleMove(true, unit.occupiedTile));
        if (unit is MeleeUnit){
            var adjValidMoves = new List<BaseTile>();
            foreach (var validMove in validMoves){
                if (validMove == null || validMove.moveType != TileMoveType.Move){
                    continue;
                }
                foreach (var adjTile in validMove.GetAdjacentTiles()){
                    if (adjTile == null ){
                        continue;
                    }
                    if (adjTile.moveType == TileMoveType.NotValid && adjTile.occupiedUnit != null && adjTile.occupiedUnit.faction != unit.faction){
                        adjValidMoves.Add(adjTile);
                    }
                }
            }
            adjValidMoves.ForEach(t => t.SetPossibleMove(true, unit.occupiedTile));
            validMoves = validMoves.Concat(adjValidMoves).ToList();
        }   
        return validMoves;
    }

    public List<BaseTile> GetPotentialValidMoves(BaseUnit unit,BaseTile newTile){
        int max = unit.MaxTileRange();
        var visited = new Dictionary<BaseTile, int>();

        //TODO: SHOULD START WITH START TILE, NOT STARTING ADJ TILES !!!
        var next = newTile.GetAdjacentTiles();
        next.ForEach(t => GVMHelper(1, max, t, visited, t, unit));
        var validMoves = visited.Keys.ToList();
        return validMoves;
    }
    public List<BaseTile> GetValidMoves(BaseUnit unit){
        int max = unit.MaxTileRange();
        BaseTile tile = unit.occupiedTile;
        var visited = new Dictionary<BaseTile, int>();

        //TODO: SHOULD START WITH START TILE, NOT STARTING ADJ TILES !!!
        var next = tile.GetAdjacentTiles();
        next.ForEach(t => GVMHelper(1, max, t, visited, t, unit));
        var validMoves = visited.Keys.ToList();
        return validMoves;
    }

    private void GVMHelper(int depth, int max, BaseTile tile, Dictionary<BaseTile, int> visited, BaseTile startTile, BaseUnit startUnit){
        if (depth >= max){
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
        next.ForEach(t => GVMHelper(depth + 1, max, t, visited, startTile, startUnit));
        return;
    }
    public void SetValidAttacks(BaseUnit unit){
        var validAttacks = unit.GetValidAttacks();
        foreach (var atk in validAttacks){
            atk.Item1.SetPossibleAttack(unit);
        }
    }

    private List<BaseTile> GetValidAttacks(BaseUnit unit)
    {
        return null;
    }

    public void RemoveAllValidMoves(){
        GridManager.instance.GetAllTiles().ForEach(t => t.SetPossibleMove(false, null));
    }

    public void ResetUnitMovment(){
        foreach (BaseUnit unit in GetAllUnits()){
            unit.ResetMovment();
        }
    }

    public void ShowUnitHealthbars(bool show){
        foreach (BaseUnit u in GetAllUnits()){
            u.healthBar.gameObject.SetActive(show);
        }
    }
    public IEnumerator AnimateUnitMove(BaseUnit unit, List<BaseTile> path, bool moveOver){

        if (unit == null){
            yield return null;
        }
        else if (path.Count > 0){
            BaseTile nextTile = path[0];
            yield return AudioManager.instance.PlayTileSound(unit, nextTile);
            Vector3 nextPos = nextTile.transform.position;
            float elapsedTime = 0;
            while (unit.transform.position != nextPos){
                unit.transform.position = Vector3.Lerp(unit.transform.position, nextPos, elapsedTime/unitMoveSpeed);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            path.RemoveAt(0);
            if (path.Count > 0){
                yield return AnimateUnitMove(unit, path, moveOver);
            }else{
                nextTile.occupiedUnit = unit;
                unit.occupiedTile = nextTile;
                if (unit.faction == UnitFaction.Hero && nextTile.attachedChest != null){
                    unit.occupiedTile.attachedChest.OpenChest();
                }
                if (moveOver){
                    unit.FinishMovement();
                }
                yield return new WaitForSeconds(0.45f);
            }
        }
    }
    internal void OnTurnStartSkills(UnitFaction faction){
        var units = GetAllUnitsOfFaction(faction);
        foreach (BaseUnit u in units){
            u.UsePassiveSkills(PassiveSkillType.OnTurnStart);
        }
    }

    internal void OnTurnEndSkills(BaseUnit unit){
        var units = GetAllUnitsOfFaction(unit.faction);
        foreach (BaseUnit u in units){
            u.UsePassiveSkills(PassiveSkillType.OnTurnEnd);
        }
    }

    internal void DescrementBuffs(UnitFaction faction)
    {
        var units = GetAllUnitsOfFaction(faction);
        foreach (BaseUnit u in units){
            u.DecrementBuffs();
        }
    }

    internal void SetDot(BaseUnit unit, int index, UnitFaction unitFaction)
    {
        if (unitFaction == UnitFaction.Hero){
            unit.uiDot = heroDots[index];
            heroDots[index].unit = unit;
        }else{
            unit.uiDot = enemyDots[index];
            enemyDots[index].unit = unit;
        }
    }

    internal void HighlightDot(UnitDot uiDot)
    {
        if (uiDot.unit.faction == UnitFaction.Enemy){
            return;
        }
        heroDotHighlight.transform.position = uiDot.transform.position;
    }
}
