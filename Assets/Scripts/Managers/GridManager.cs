using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    public static GridManager instance;
    private int width, height;
    [SerializeField] private TileSet tileSet;
    public LevelChest chestPrefab;
    //Predabs
    [SerializeField] private FloorTile floorPrefab;
    [SerializeField] private WallTile wallPrefab;
    [SerializeField] private Transform cam;
    private Dictionary<Vector2, BaseTile> tiles;
    private Dictionary<Vector2, TileEditorType> tileTypes;

    //true if melee, false if ranged
    public Dictionary<Vector2, UnitSpawnType> team1spawns, team2spawns;
    public List<Vector2> chestSpawns;
    public BaseTile hoveredTile;
    private const int NOUSE_MAP_SIZE = 500;
    public List<LevelBase> bases;
    [Range(0.0f, 1.0f)]
    public float chestSpawnRate = 0.3f;
    void Awake(){
        instance = this;
    }
    public int getWidth(){
        return width;
    }
    public int getHeight(){
        return height;
    }
    public void GenerateGrid()
    {
        tiles = new();
        var foundTiles = FindObjectsOfType<BaseTile>().ToList();
        
        team1spawns = new();
        team2spawns = new();
        chestSpawns = new();

        foreach (BaseTile bt in foundTiles){
            var pos = bt.transform.position;
            bt.coordiantes = pos;
            var spawn = bt.spawnTeam;
            Debug.Log(pos);
            if (tiles.ContainsKey(pos)){
                continue;
            }
            tiles.Add(pos, bt);

            if (spawn == SpawnFaction.BlueMelee){
                team1spawns.Add(pos, UnitSpawnType.Melee);
            } else if (spawn == SpawnFaction.BlueRanged){
                team1spawns.Add(pos, UnitSpawnType.Ranged);
            }else if (spawn == SpawnFaction.BlueEither){
                team1spawns.Add(pos, UnitSpawnType.Both);
            }
            else if (spawn == SpawnFaction.OrangeMelee){
                team2spawns.Add(pos, UnitSpawnType.Melee);
            } else if (spawn == SpawnFaction.OrangeRanged){
                team2spawns.Add(pos, UnitSpawnType.Ranged);
            }else if (spawn == SpawnFaction.OrangeEither){
                team2spawns.Add(pos, UnitSpawnType.Both);
            }

            if (bt.spawnChest){
                chestSpawns.Add(pos);
            }
        }
        if (chestSpawns.Count > 1){
            int numChests = UnityEngine.Random.Range(2, chestSpawns.Count);
            for (int i = 0; i < numChests; i++){
                Vector2 chestPos = chestSpawns[UnityEngine.Random.Range(0, chestSpawns.Count)];
                BaseTile chestTile = GetTileAtPosition(chestPos);
                var newChest = Instantiate(chestPrefab, chestPos, Quaternion.identity);
                newChest.PlaceChest(chestTile);
                chestSpawns.Remove(chestPos);
            }
        }
        GameManager.instance.ChangeState(GameState.SapwnHeroes);
    }
   
    public BaseTile GetTileAtPosition(Vector2 pos){
        if (tiles.TryGetValue(pos, out var tile)){
            return tile;
        }
        return null;
    }
    public BaseTile GetTileAtPosition(int x, int y){
        return GetTileAtPosition(new Vector2(x, y));
    }

    public BaseTile GetAdjacentValidTile(Vector2 startPos, Vector2 endPos){
        List<BaseTile> tiles = GetAdjacentTiles(endPos);
        BaseTile bestTile = null;
        foreach (BaseTile t in tiles){
            if (t.moveType != TileMoveType.NotValid){
                bestTile = t;
            }
            if (t.coordiantes.Equals(startPos)){
                return t;
            }
        }
        return bestTile;
    }

    public List<BaseTile> GetAdjacentTiles(UnityEngine.Vector2 pos){
        return GetAdjacentTiles((int)pos.x, (int)pos.y);
    }
    public List<BaseTile> GetAdjacentTiles(int x, int y){
        List<BaseTile> tiles = new List<BaseTile>();
        GetAdjecentTile(x, y, 1, 0, tiles);
        GetAdjecentTile(x, y, -1, 0, tiles);
        GetAdjecentTile(x, y, 0, 1, tiles);
        GetAdjecentTile(x, y, 0, -1, tiles);
        return tiles;
    }

    private BaseTile GetAdjecentTile(int x, int y, int xOff, int yOff, List<BaseTile> tiles=null){
        var t = GetTileAtPosition(x+xOff, y+yOff);
        if (tiles != null && t != null){
            tiles.Add(t);
        }
        return t;
    }
    
    public (BaseTile, UnitSpawnType) GetSpawnTile(bool team1){
//        Debug.Log(tiles.Values.Count);
        var spawnPositions = team1spawns;
        if (!team1){
            spawnPositions = team2spawns;
        }
        int randomIndex = UnityEngine.Random.Range(0, spawnPositions.Count);
        Vector2 pos = spawnPositions.Keys.ToList()[randomIndex];
        UnitSpawnType spawnType = spawnPositions[pos];
        var tile = GetTileAtPosition(pos);
        spawnPositions.Remove(pos);
        return (tile, spawnType);
    }
    public List<BaseTile> GetAllTiles(){
        return tiles.Values.ToList();
    }
    

    public void SetHoveredTile(BaseTile newTile){
        hoveredTile = newTile;
        hoveredTile.OnHover();
    }

    public void MoveHoveredTile(Vector2 direction){
        if (hoveredTile == null){
            return;
        }
        var newTile = GetTileAtPosition(hoveredTile.coordiantes + direction);
        if (newTile == null){
            AudioManager.instance.PlayBlock();
            return;
        }
        MoveHoveredTile(newTile);
    }
    public void MoveHoveredTile(BaseTile newTile){
        if (!newTile.IsTileSelectable()){
            AudioManager.instance.PlayBlock();
            return;
        }
        AudioManager.instance.PlayMove();
        SetHoveredTile(newTile);
    }
    public void SelectHoveredTile(){
        hoveredTile.OnSelectTile();
    }

    public List<BaseTile> GetRectangleTiles(BaseTile t, int maxX, int maxY){
        List<BaseTile> validTiles = new List<BaseTile>();
        Vector2 start = t.coordiantes;
        Vector2 direction = new (SkillManager.instance.useDirection.x, SkillManager.instance.useDirection.y);
        if (SkillManager.instance.useDirection == Vector2.up || SkillManager.instance.useDirection == Vector2.down){
            int temp = maxX;
            maxX = maxY;
            maxY = temp;
        }

        Vector2 topLeft = new((int)Math.Round(start.x - (maxX/2)), (int)Math.Round(start.y - maxY/2));
        // Debug.Log(t.coordiantes);
        // Debug.Log(topLeft);
        // bool notValid = true;
        // while (notValid){
        //     notValid = false;
            for (int x = 0; x < maxX; x++){
                // if (notValid){
                //     break;
                // }
                for (int y = 0; y < maxY; y++){
                    BaseTile tile = GetTileAtPosition(topLeft + new Vector2(x, y));
                    if (tile == null){
                        continue;
                    }
                    // if (tile == UnitManager.instance.selectedUnit.occupiedTile){
                    //     direction = new (0,0);
                    //     notValid = true;
                    //     break;
                    // }
                    validTiles.Add(tile);
                }
            }
        // }
        Debug.Log(validTiles.Count);
        return validTiles;
    }
    public List<BaseTile> GetRadiusTiles(BaseTile t, int maxDepth){
        var visited = new Dictionary<BaseTile, int>();
        visited[t] = 0;
        var next = t.GetAdjacentTiles();
        next.ForEach(t => GetRadiusTilesHelper(1, maxDepth, t, visited, t));
        var validMoves = visited.Keys.ToList();
        return validMoves;
    }

    private void GetRadiusTilesHelper(int depth, int max, BaseTile tile, Dictionary<BaseTile, int> visited, BaseTile startTile){
        if (depth >= max ){
            return;
        }
        //enemy's are valid moves but block movement
        if (tile != null && tile.occupiedUnit != null){
            visited[tile] = depth;
            return;
        }
        //if tile is not valid, continue
        if (tile == null || tile is WallTile || (visited.ContainsKey(tile) && visited[tile] == depth)){
            return;
        }

        //if tile is valid, add it to the list of visited tiles and continue
        visited[tile] = depth;
        var next = tile.GetAdjacentTiles();   
        next.ForEach(t => GetRadiusTilesHelper(depth + 1, max, t, visited, startTile));
        return;
    }
    public int Distance(BaseTile t1, BaseTile t2){
        return Distance(t1.coordiantes, t2.coordiantes);
    }
    public int Distance(Vector2 v1, Vector2 v2){
        int x = (int)Math.Abs(v1.x - v2.x);
        int y = (int)Math.Abs(v1.y - v2.y);
        return x + y;
    }
    /// <summary>
    /// Returns the shortest path between 2 BaseTiles
    /// </summary>
    /// <param name="start">Start Tile</param>
    /// <param name="end">End Tile</param>'
    /// <param name="withPathLine">Whether or not pathline is being drawn</param>'
    public List<BaseTile> ShortestPathBetweenTiles(BaseTile start, BaseTile end, bool withPathLine = false)
    {
        //if they are the same tile, return just the start tile
        if (start == end)
        {
            return new List<BaseTile> { start };
        }
        //if the end tile is in range but cannot be moved to, return an empty list
        if (end.moveType == TileMoveType.InAttackRange)
        {
            return new();
        }

        //tiles that were visted
        List<BaseTile> visited = new();

        //tiles that need adj tiles visited
        Queue<BaseTile> toVisitAdj = new();

        BaseUnit startUnit = start.occupiedUnit;

        //Dictionary describing possible paths to the end position
        Dictionary<BaseTile, BaseTile> previousTiles = new();

        BaseTile current = start;
        previousTiles.Add(current, null);
        do
        {
            var adjTiles = current.GetAdjacentTiles();
            //iterate through tiles to look through, and add to queue if is a valid tile
            foreach (BaseTile tile in adjTiles)
            {
                if (tile == null)
                {
                    continue;
                }
                //if the tile has already been looked through
                if (visited.Contains(tile))
                {
                    continue;
                }
                if (withPathLine)
                {
                    //if path line is being drawn DO NOT add the last tile if it is an attack tile, 
                    //so the unit moves one tile away from the enemy unit it will attack
                    if (tile.moveType == TileMoveType.NotValid || (startUnit is MeleeUnit && tile.moveType == TileMoveType.Attack))
                    {
                        continue;
                    }
                }
                else
                {
                    //dont add wall or occupied tiles to visit, since they cannot be moved to or through
                    if (tile is WallTile || tile.occupiedUnit != null)
                    {
                        continue;
                    }
                }

                //tile did not fail, add it to 
                visited.Add(tile);
                toVisitAdj.Enqueue(tile);
                if (!previousTiles.ContainsKey(tile))
                {
                    previousTiles.Add(tile, current);
                }
            }
            //if no more adjTiles to check, remove current from adj tile list
            if (toVisitAdj.Count > 0)
            {
                current = toVisitAdj.Dequeue();
            }

            //if we have reached the end tile succesfully
            if (current == end || toVisitAdj.Count == 0)
            {
                List<BaseTile> finalTiles = new();
                var finalCurr = current;
                //trace back through the tile history to get our shortest path
                while (finalCurr != null)
                {
                    finalTiles.Add(finalCurr);
                    finalCurr = previousTiles[finalCurr];
                }
                return finalTiles;
            }
            //if there are more tiles to visit, keep looping
        } while (toVisitAdj.Count > 0);
        return null;
    }
}

public enum UnitSpawnType {
    Both,
    Melee,
    Ranged
}