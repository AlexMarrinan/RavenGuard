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
    public Dictionary<Vector2, LayerSize> chestSpawns;
    public BaseTile hoveredTile;
    private const int NOUSE_MAP_SIZE = 500;
    public List<LevelBase> bases;
    [Range(0.0f, 1.0f)]
    public float chestSpawnRate = 0.3f;
    public bool testMode = false, newMode = false;
    void Awake(){
        instance = this;
    }
    public int getWidth(){
        return width;
    }
    public int getHeight(){
        return height;
    }
    public void LoadAssets(){
        if (testMode){
            bases =  Resources.LoadAll<LevelBase>("Levels/TestBases").ToList();
        }else{
            bases =  Resources.LoadAll<LevelBase>("Levels/Bases").ToList();
        }
    }

    public void GenerateGrid()
    {
        //TODO: REMOVE OLD GRID GENERATION LOGIC
        if (newMode){
            tiles = new();
            var foundTiles = FindObjectsOfType<BaseTile>().ToList();
            
            team1spawns = new();
            team2spawns = new();
            chestSpawns = new();

            foreach (BaseTile bt in foundTiles){
                var pos = bt.transform.position;
                bt.coordiantes = pos;
                var spawn = bt.spawnTeam;
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
                    float randValue = UnityEngine.Random.value;
                //TODO: MAKE CHEST SPAWNS ASSOSIATED WITH LEVEL PROGRESSION, NOT PURELY RANDOM
                    if (randValue < chestSpawnRate){
                        var newChest = Instantiate(chestPrefab, new Vector3(bt.coordiantes.x, bt.coordiantes.y), Quaternion.identity);
                        newChest.PlaceChest(bt);
                    }
                }
            }
            GameManager.instance.ChangeState(GameState.SapwnHeroes);
            return;
        }
        tileTypes = new Dictionary<Vector2, TileEditorType>();

        team1spawns = new();
        team2spawns = new();
        chestSpawns = new();

        int randIndex = UnityEngine.Random.Range(0, bases.Count);
        LevelBase pgb = bases[randIndex];
        var newArray = new Array2D<TileEditorType>(pgb.width, pgb.height);
        var newChestArray = new Array2D<LayerSize>(pgb.width, pgb.height);
        var newSpawnArray = new Array2D<SpawnFaction>(pgb.width, pgb.height);

        //        Debug.Log("new array" + (newArray.Width, newArray.Height));
        newArray.DeepCopy(pgb.array);
        newChestArray.DeepCopy(pgb.chestArray);
        newSpawnArray.DeepCopy(pgb.spawnArray);
        //      Debug.Log("new array" + (newArray.Width, newArray.Height));
        //Randomly Flip Layout
        if (UnityEngine.Random.Range(0, 2) == 1)
        {
            newArray.FlipX();
            newChestArray.FlipX();
            newSpawnArray.FlipX();
        }
        else if (UnityEngine.Random.Range(0, 2) == 1)
        {
            newArray.FlipY();
            newChestArray.FlipY();
            newSpawnArray.FlipY();
        }

        //Randomly Rotate Layout
        int numRotates = UnityEngine.Random.Range(0, 4);
        for (int i = 0; i < numRotates; i++)
        {
            newArray.Rotate();
            newChestArray.Rotate();
            newSpawnArray.Rotate();
        }


        width = newArray.Width;
        height = newArray.Height;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var pos = new Vector2(x, y);
                int newy = height - 1 - y;
                LayerSize chest = newChestArray.Get(x, y);
                SpawnFaction spawn = newSpawnArray.Get(x,y);

                //TODO: SPAWNM CHESTS ACCORDING TO LEVEL PROGRESSION SYSTEM
                if (chest != LayerSize.None){
                    chestSpawns.Add(pos, chest);
                }
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
                tileTypes[pos] = newArray.grid[newy * width + x];
            }
        }

        tiles = new();
        foreach (Vector2 pos in tileTypes.Keys)
        {
            int x = (int)pos.x;
            int y = (int)pos.y;

            BaseTile randomTile = GetTileFromType(tileTypes[pos]);
            randomTile.SetBGSprite(tileSet.GetRandomFloor());

            var newTile = Instantiate(randomTile, new Vector3(x, y), Quaternion.identity);
            newTile.name = $"Tile {pos.x} {pos.y}";

            newTile.Init(x, y);
            newTile.coordiantes = pos;
            tiles[pos] = newTile;
            if (chestSpawns.ContainsKey(pos)){
                float randValue = UnityEngine.Random.value;
                //TODO: MAKE CHEST SPAWNS ASSOSIATED WITH LEVEL PROGRESSION, NOT PURELY RANDOM
                if (randValue < chestSpawnRate){
                    var newChest = Instantiate(chestPrefab, new Vector3(x, y), Quaternion.identity);
                    newChest.PlaceChest(newTile);
                }
            }
        }
        foreach (BaseTile t in tiles.Values)
        {
            if (t.editorType == TileEditorType.Grass)
            {
                SetGrassTileSprites(t as FloorTile);
            }
            else if (t.editorType == TileEditorType.Mountain)
            {
                SetMountainTileSprites(t as WallTile);
            }
            else if (t.editorType == TileEditorType.Forest)
            {
                SetForestTileSprites(t as FloorTile);
            }
            else if (t.editorType == TileEditorType.Bridge)
            {
                SetBridgeTileSprites(t as FloorTile);
            }
            else if (t.editorType == TileEditorType.Water)
            {
                SetWaterTileSprites(t as WallTile);
            }
        }
        cam.transform.position = new Vector3((float)width / 2 - 0.5f, (float)height / 2 - 0.5f, -10);
        GameManager.instance.ChangeState(GameState.SapwnHeroes);
    }
    private BaseTile GetTileFromType(TileEditorType type){
        // float scale = 1f;//0.16f;
        // float actualX = (float)(x * scale);
        // float actualY = (float)(y * scale);

        //TODO: GET ANY FLOOR/WALL
        BaseTile randomTile = null;
        switch (type)
        {
            case TileEditorType.Grass:
                randomTile = floorPrefab;
                break;
            case TileEditorType.Mountain:
                randomTile = wallPrefab;
                break;
            case TileEditorType.Forest:
                randomTile = floorPrefab;
                break;
            case TileEditorType.Water:
                randomTile = wallPrefab;
                break;
            case TileEditorType.Bridge:
                randomTile = floorPrefab;
                break;
            case TileEditorType.None:
                return null;
        }
        randomTile.editorType = type;
        return randomTile;
    }
    private BaseTile GetTileTypeAtPos(LevelBase pgb, int x, int y)
    {
        TileEditorType type = pgb.GetTileType(x, y);
        return GetTileFromType(type);
    }
    private void SetGrassTileSprites(FloorTile ft){
        // int idx = GetBlendTileIndex(ft);
        // if (idx == -1){
        ft.SetBGSprite(tileSet.GetRandomFloor());
        // }else{
        //     ft.SetBGSprite(tileSet.floorXWalls[idx]);
        // }
    }

    private void SetMountainTileSprites(WallTile wt){
        int idx = GetBlendTileIndex(wt);
        if (idx == -1){
            wt.SetFGSprite(tileSet.GetRandomWall());
        }else{
            wt.SetFGSprite(tileSet.walls[idx]);
            wt.SetBGSprite(tileSet.GetRandomFloor());
        }
    }

    private int GetBlendTileIndex(BaseTile bt){
        TileEditorType tileEditorType = bt.editorType;
        Vector2 pos = bt.coordiantes;
        var up = GetAdjecentTile((int)pos.x, (int)pos.y, 0, 1);
        var down = GetAdjecentTile((int)pos.x, (int)pos.y, 0, -1);
        var left = GetAdjecentTile((int)pos.x, (int)pos.y, -1, 0);
        var right = GetAdjecentTile((int)pos.x, (int)pos.y, 1, 0);

        var u = up != null && up.editorType != tileEditorType && up.editorType != TileEditorType.Bridge;
        var d = down != null && down.editorType != tileEditorType  && down.editorType != TileEditorType.Bridge;;
        var l = left != null && left.editorType != tileEditorType  && left.editorType != TileEditorType.Bridge;;
        var r = right != null && right.editorType != tileEditorType  && right.editorType != TileEditorType.Bridge;;

        var directions = u && d && l && r;
        var notDirections = !u && !d && !l && !r;

        var upleft = GetAdjecentTile((int)pos.x, (int)pos.y, -1, 1);
        var downleft = GetAdjecentTile((int)pos.x, (int)pos.y, -1, -1);
        var upright = GetAdjecentTile((int)pos.x, (int)pos.y, 1, 1);
        var downright = GetAdjecentTile((int)pos.x, (int)pos.y, 1, -1);

        var ul = upleft != null && upleft.editorType != tileEditorType && upleft.editorType != TileEditorType.Bridge;
        var dl = downleft != null && downleft.editorType != tileEditorType  && downleft.editorType != TileEditorType.Bridge;;
        var ur = upright != null && upright.editorType != tileEditorType && upright.editorType != TileEditorType.Bridge;;
        var dr = downright != null && downright.editorType != tileEditorType && downright.editorType != TileEditorType.Bridge;;


        var corners = ul && dl && ur && dr;
        var notCorners = !ul && !dl && !ur && !dr;

        if (notDirections){
            //4 corenrs
            if (corners){
                return 8;
            }
            //1 corner
            if (ul && !dl && !ur && !dr){
                return 27;
            }     
            if (!ul && dl && !ur && !dr){
                return 21;
            }     
            if (!ul && !dl && ur && !dr){
                return 26;
            }     
            if (!ul && !dl && !ur && dr){
                return 20;
            }     
            //2 corners
            if (ul && !dl && !ur && dr){
                return 28;
            }            
            if (!ul && dl && ur && !dr){
                return 34;
            }        

            //3 corners
            if (!ul && dl && ur && dr){
                
            }            
            if (ul && !dl && ur && dr){
                
            }
            if (ul && dl && !ur && dr){
                
            }     
            if (ul && !dl && ur && !dr){
                
            }
            return 5;
        }
        //Single direction walls
        if (u && !d && !l && !r){
            if (corners){
                return 24;
            }
            if (dr){
                return 33;
            }
            if (dl){
                return 39;
            }
            return 1;
        }
        if (!u && d && !l && !r){
            if (corners){
                return 19;
            }
            if (ur){
                return 32;
            }
            if (ul){
                return 38;
            }
            return 11;
        }
        if (!u && !d && l && !r){
            if (corners){
                return 25;
            }
            if (ur){
                return 30;
            }
            if (dr){
                return 31;
            }
            return 4;
        }
        if (!u && !d && !l && r){
            if (corners){
                return 18;
            }
            if (ul){
                return 36;
            }
            if (dl){
                return 37;
            }
            return 6;
        }

        //UP and 
        if (u && d && !l && !r){
            return 13;
        }
        if (u && !d && l && !r){
            if (dr){
                return 23;
            }
            return 0;
        }
        if (u && !d && !l && r){
            if (dl){
                return 22;
            }
            return 2;
        }
        //DOWN and 
        if (!u && d && l && !r){
            if (ur){
                return 17;
            }
            return 10;
        }
        if (!u && d && !l && r){
            if (ul){
                return 16;
            }
            return 12;
        }
        //LEFT and RIGHT
        if (!u && !d && l && r){
            return 15;
        }

        //NOTS
        if (!u && d && l && r){
            return 14;
        }
        if (u && !d && l && r){
            return 3;
        }
        if (u && d && !l && r){
            return 9;
        }
        if (u && d && l && !r){
            return 7;
        }

        // //single walls
        if (ul && !dl && !ur && !dr){
            return 27;
        }
        if (!ul && dl && !ur && !dr){
            return 21;
        }
        if (!ul && !dl && ur && !dr){
            return 26;
        }
        if (!ul && !dl && !ur && dr){
            return 20;
        }

        if (notCorners && u && !d && l && !r){
            return 21;
        }
        if (!u && !l && d && r && ur && dr && ul && dl){
            return 16;
        }
        if (u && !l && !d && r && ur && dr && ul && dl){
            return 20;
        }
        if (!u && l && d && !r && ur && dr && ul && dl){
            return 17;
        }
        // if (ul && dl && !ur && !dr){
        //     return 18;
        // }
        // if (ul && !dl && ur && !dr){
        //     return 19;
        // }
        // if (ul && !dl && !ur && dr){
        //     return 24;
        // }
        // if (!ul && dl && ur && !dr){
        //     return 21;
        // }
        // if (!ul && dl && !ur && dr){
        //     return 27;
        // }
        // if (!ul && !dl && ur && dr){
        //     return 23;
        // }
        // if (ul && dl && ur && !dr){
        //     return 20;
        // }
        // if (ul && dl && !ur && dr){
        //     return 28;
        // }
        // if (ul && !dl && ur && dr){
        //     return 25;
        // }
        // if (!ul && dl && ur && dr){
        //     return 29;
        // }
        // if (ul && dl && ur && dr){
        //     return 22;
        // }
        return 5;
    }
    private void SetForestTileSprites(FloorTile ft) {
        int idx = GetBlendTileIndex(ft);
        if (idx == -1){
            // wt.SetFGSprite(tileSet.GetRandomWall());
        }else{
            ft.SetFGSprite(tileSet.forest[idx]);
            ft.SetBGSprite(tileSet.GetRandomFloor());
        }    
    }
    private void SetBridgeTileSprites(FloorTile ft) {

        //TODO: MAKE EDITOR TYPE SEPERATE BG TILE SPRITE TYPE
        ft.editorType = TileEditorType.Water;        
        int idx = GetBlendTileIndex(ft);
        ft.SetBGSprite(tileSet.GetRandomFloor());
        ft.SetMidSprite(tileSet.water[idx]);
        ft.SetFGSprite(tileSet.bridge[0]);
    }
    private void SetWaterTileSprites(WallTile wt) {
        int idx = GetBlendTileIndex(wt);
        if (idx == -1){
            // wt.SetFGSprite(tileSet.GetRandomWall());
        }else{
            wt.SetFGSprite(tileSet.water[idx]);
            wt.SetBGSprite(tileSet.GetRandomFloor());
        }
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
        
        BaseUnit sUnit = UnitManager.instance.selectedUnit;
        //if a unit is selected, dont move to tiles that arent valid moves
        // if (sUnit != null && newTile.moveType == TileMoveType.NotValid){
        //     //if the next tile is the tile occupied by the selected unit, move one past it
        //     if (newTile.occupiedUnit == sUnit){
        //         // newTile = GetTileAtPosition(newTile.coordiantes + direction);
        //         // if (newTile == null){
        //         //     return;
        //         // }
        //         // //remove hovered tile from path
        //         // PathLine.instance.Reset();
        //         // PathLine.instance.AddTile(UnitManager.instance.selectedUnit.occupiedTile);
        //     }else{
        //         AudioManager.instance.PlayBlock();
        //         return;
        //     }
        // }

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

  public float[,] GenerateNoiseMap(int mapDepth, int mapWidth, float scale) {
                // create an empty noise map with the mapDepth and mapWidth coordinates
    float[,] noiseMap = new float[mapDepth, mapWidth];
    for (int zIndex = 0; zIndex < mapDepth; zIndex ++) {
      for (int xIndex = 0; xIndex < mapWidth; xIndex++) {
                                // calculate sample indices based on the coordinates and the scale
        float sampleX = xIndex / scale;
        float sampleZ = zIndex / scale;
                                // generate noise value using PerlinNoise
        float noise = Mathf.PerlinNoise (sampleX, sampleZ);
        noiseMap [zIndex, xIndex] = noise;
      }
    }
    return noiseMap;
  }

  public List<BaseTile> ShortestPathBetweenTiles(BaseTile start, BaseTile end, bool withPathLine = false){
    if (start == end){
//        Debug.Log("A0");
        return new List<BaseTile>{start};
    }
//    Debug.Log("A");
    if (end.moveType == TileMoveType.InAttackRange){
//        Debug.Log("B");
        return new();
    }
//    Debug.Log("C");
    List<BaseTile> visited = new();
    Queue<BaseTile> toVisit = new();
    BaseUnit startUnit = start.occupiedUnit;
    Dictionary<BaseTile, BaseTile> previousTiles = new();
    BaseTile current = start;
    previousTiles.Add(current, null);
    do {
        var adjTiles = current.GetAdjacentTiles();
//        Debug.Log(adjTiles.Count);
        foreach (BaseTile tile in adjTiles){
            if (tile == null){
                continue;
            }
            if (visited.Contains(tile)){
                continue;
            }
            if (withPathLine){
                //TOOD: MAKE IT SO IF ITS AN ATTACK TILE BREAKS RANGED UNIT PATHFINDING !!!
                if (tile.moveType == TileMoveType.NotValid || (startUnit is MeleeUnit && tile.moveType == TileMoveType.Attack) ){
                    continue;
                }
            }else{
                if (tile is WallTile || tile.occupiedUnit != null){
                    continue;
                }
            }
            visited.Add(tile);
            toVisit.Enqueue(tile);
            previousTiles.Add(tile, current);
        }
        if (toVisit.Count > 0){
            current = toVisit.Dequeue();
        }
        if (current == end || toVisit.Count == 0){
            List<BaseTile> finalTiles = new();
            var finalCurr = current;
            while (finalCurr != null){
                //if (finalCurr.moveType == TileMoveType.Move || finalCurr == start){
                    //ONLY add tile if its MOVE type;
                    //if its start space, also add
                    finalTiles.Add(finalCurr);
                // }
                finalCurr = previousTiles[finalCurr];
            }
            return finalTiles;
        }
    } while (toVisit.Count > 0);
    return null;
  }
}

public enum UnitSpawnType {
    Both,
    Melee,
    Ranged
}