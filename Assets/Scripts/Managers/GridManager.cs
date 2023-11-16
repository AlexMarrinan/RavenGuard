using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class GridManager : MonoBehaviour
{
    public static GridManager instance;
    [SerializeField] private int width, height;
    [SerializeField] private TileSet tileSet;

    //Predabs
    [SerializeField] private FloorTile floorPrefab;
    [SerializeField] private WallTile wallPrefab;

    [SerializeField] private Transform cam;
    private Dictionary<Vector2, BaseTile> tiles;
    private float[,] noiseMap;
    public BaseTile hoveredTile;
    private const int NOUSE_MAP_SIZE = 500;

    void Awake(){
        instance = this;
    }
    public int getWidth(){
        return width;
    }
    public int getHeight(){
        return height;
    }
    //Generates a grid of width x height of the tilePrefab
    public void GenerateGrid(){
        tiles = new Dictionary<Vector2, BaseTile>();
        this.noiseMap = GenerateNoiseMap(NOUSE_MAP_SIZE, NOUSE_MAP_SIZE, 8.0f);

        var startX = UnityEngine.Random.Range(0, NOUSE_MAP_SIZE-width);
        var startY = UnityEngine.Random.Range(0, NOUSE_MAP_SIZE-height);

        for (int x = 0; x < width; x++){
            for (int y = 0; y < height; y++){
                // float scale = 1f;//0.16f;
                // float actualX = (float)(x * scale);
                // float actualY = (float)(y * scale);

                //TODO: GET ANY FLOOR/WALL
                BaseTile randomTile = floorPrefab;
                randomTile.SetSprite(tileSet.GetRandomFloor());

                if (noiseMap[x+startX, y+startY] > 0.6f){
                    randomTile = wallPrefab;
                    randomTile.SetSprite(tileSet.walls[0]);
                }


                
                // if (randomTile is FloorTile){
                //     int idk = Random.Range(0, 50);
                //     if (idk == 0){
                //         randomTile = tileSet.pits[0];
                //     }else if (idk == 1){
                //         randomTile = tileSet.fences[0];
                //     }
                // }                
                
                // //TODO: MAKE ACTUALLY SPAWN TILE CORRECTLY
                // var randomTile = Random.Range(0, 6) == 3 ? mountainTile : grassTile;
                
                var newTile = Instantiate(randomTile, new Vector3(x,y), Quaternion.identity);
                newTile.name = $"Tile {x} {y}";
                
                newTile.Init(x, y);
                var pos = new Vector2(x,y);
                tiles[pos] = newTile;
                newTile.coordiantes = pos;
            }
        }

        foreach (BaseTile t in tiles.Values){
            if (t is FloorTile){
                SetFloorTileSprite(t as FloorTile);
            }else if (t is WallTile){
                SetWallTileSprite(t as WallTile);
            }
        }
        cam.transform.position = new Vector3((float)width/2 -0.5f, (float)height/2 -0.5f, -10);
        GameManager.instance.ChangeState(GameState.SapwnHeroes);
    }



    private void SetFloorTileSprite(FloorTile ft){
        int idx = GetFloorTileIndex(ft);
        if (idx == -1){
            ft.SetSprite(tileSet.GetRandomFloor());
        }else{
            ft.SetSprite(tileSet.floorXWalls[idx]);
        }
    }

    //TODO: MAKE NOT ASS HOLY SHIT
    private int GetFloorTileIndex(FloorTile wt){
        Vector2 pos = wt.coordiantes;
        var up = GetAdjecentTile((int)pos.x, (int)pos.y, 0, 1);
        var down = GetAdjecentTile((int)pos.x, (int)pos.y, 0, -1);
        var left = GetAdjecentTile((int)pos.x, (int)pos.y, -1, 0);
        var right = GetAdjecentTile((int)pos.x, (int)pos.y, 1, 0);

        var u = up != null && up is WallTile;
        var d = down != null && down is WallTile;
        var l = left != null && left is WallTile;
        var r = right != null && right is WallTile;

        //Single direction walls
        if (u && !d && !l && !r){
            return 7;
        }
        if (!u && d && !l && !r){
            return 1;
        }
        if (!u && !d && l && !r){
            return 5;
        }
        if (!u && !d && !l && r){
            return 3;
        }

        //UP and 
        if (u && d && !l && !r){
            return 18;
        }
        if (u && !d && l && !r){
            return 9;
        }
        if (u && !d && !l && r){
            return 10;
        }
        //DOWN and 
        if (!u && d && l && !r){
            return 12;
        }
        if (!u && d && !l && r){
            return 13;
        }
        //LEFT and RIGHT
        if (!u && !d && l && r){
            return 14;
        }

        //NOTS
        if (!u && d && l && r){
            return 17;
        }
        if (u && !d && l && r){
            return 11;
        }
        if (u && d && !l && r){
            return 16;
        }
        if (u && d && l && !r){
            return 15;
        }

        var upleft = GetAdjecentTile((int)pos.x, (int)pos.y, -1, 1);
        var downleft = GetAdjecentTile((int)pos.x, (int)pos.y, -1, -1);
        var upright = GetAdjecentTile((int)pos.x, (int)pos.y, 1, 1);
        var downright = GetAdjecentTile((int)pos.x, (int)pos.y, 1, -1);

        var ul = upleft != null && upleft is WallTile;
        var dl = downleft != null && downleft is WallTile;
        var ur = upright != null && upright is WallTile;
        var dr = downright != null && downright is WallTile;

        //single walls
        if (ul && !dl && !ur && !dr){
            return 8;
        }
        if (!ul && dl && !ur && !dr){
            return 2;
        }
        if (!ul && !dl && ur && !dr){
            return 6;
        }
        if (!ul && !dl && !ur && dr){
            return 0;
        }
        
        if (ul && dl && !ur && !dr){
            return 26;
        }
        if (ul && !dl && ur && !dr){
            return 19;
        }
        if (ul && !dl && !ur && dr){
            return 24;
        }
        if (!ul && dl && ur && !dr){
            return 21;
        }
        if (!ul && dl && !ur && dr){
            return 27;
        }
        if (!ul && !dl && ur && dr){
            return 23;
        }
        if (ul && dl && ur && !dr){
            return 20;
        }
        if (ul && dl && !ur && dr){
            return 28;
        }
        if (ul && !dl && ur && dr){
            return 25;
        }
        if (!ul && dl && ur && dr){
            return 29;
        }
        if (ul && dl && ur && dr){
            return 22;
        }
        return -1;
    }
    private void SetWallTileSprite(WallTile wt) {
        wt.SetSprite(tileSet.walls[0]);
    }
    // private int GetWallTileIndex(WallTile wt){
    //     Vector2 pos = wt.coordiantes;
    //     var up = GetAdjecentTile((int)pos.x, (int)pos.y, 0, -1);
    //     var down = GetAdjecentTile((int)pos.x, (int)pos.y, 0, 1);
    //     var left = GetAdjecentTile((int)pos.x, (int)pos.y, -1, 0);
    //     var right = GetAdjecentTile((int)pos.x, (int)pos.y, 1, 0);

    //     var u = up != null && up is WallTile;
    //     var d = down != null && down is WallTile;
    //     var l = left != null && left is WallTile;
    //     var r = right != null && right is WallTile;

    //     if (!u && !d && !l && !r){
    //         return 5;
    //     }
    //     if (u && !d && !l && !r){
    //         return 4;
    //     }
    //     return 0;
    // }
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
    
    public BaseTile GetHeroSpawnTile(){
        return tiles.Where(t => t.Key.x < width/2 && t.Value.walkable).OrderBy(t => UnityEngine.Random.value).First().Value;
    }

    public BaseTile GetEnemySpawnTile(){
        return tiles.Where(t => t.Key.x > width/2 && t.Value.walkable).OrderBy(t => UnityEngine.Random.value).First().Value;
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
            return;
        }
        MoveHoveredTile(newTile);
    }
    public void MoveHoveredTile(BaseTile newTile){
        
        BaseUnit sUnit = UnitManager.instance.selectedUnit;
        //if a unit is selected, dont move to tiles that arent valid moves
        if (sUnit != null && newTile.moveType == TileMoveType.NotValid){
            //if the next tile is the tile occupied by the selected unit, move one past it
            if (newTile.occupiedUnit == sUnit){
                // newTile = GetTileAtPosition(newTile.coordiantes + direction);
                // if (newTile == null){
                //     return;
                // }
                // //remove hovered tile from path
                // PathLine.instance.Reset();
                // PathLine.instance.AddTile(UnitManager.instance.selectedUnit.occupiedTile);
            }else{
                return;
            }
        }

        if (!newTile.IsTileSelectable()){
            return;
        }
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

  public List<BaseTile> ShortestPathBetweenTiles(BaseTile start, BaseTile end, bool withPathLine){
    if (start == end){
        return new List<BaseTile>{start};
    }
    if (end.moveType == TileMoveType.InAttackRange){
        return new();
    }
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
                if (finalCurr.moveType == TileMoveType.Move || finalCurr == start){
                    //ONLY add tile if its MOVE type;
                    //if its start space, also add
                    finalTiles.Add(finalCurr);
                }
                finalCurr = previousTiles[finalCurr];
            }
            if (start.occupiedUnit != null && start.occupiedUnit is RangedUnit && end.moveType == TileMoveType.Attack){
                RangedUnit rangedUnit = start.occupiedUnit as RangedUnit;
                int distance = end.DistanceFrom(start);
                Debug.Log("ranged ataack distance " + distance);
                // if (distance >= rangedUnit.maxMoveAmount){
                    int max = rangedUnit.moveAmount - 1;
                    int pathLength = distance - max;
                    Debug.Log("pathLength " + pathLength);

                    if (pathLength >= 0){
                        int range = rangedUnit.rangedWeapon.maxRange - distance; //+ rangedUnit.rangedWeapon.minRange;
                        if (range > finalTiles.Count){
                            range = finalTiles.Count;
                        }
                        finalTiles.RemoveRange(0, range);
                    }
                // }
            }
            return finalTiles;
        }
    } while (toVisit.Count > 0);
    return null;
  }
}