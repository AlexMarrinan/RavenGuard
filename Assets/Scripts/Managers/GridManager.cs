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
    [SerializeField] private Transform cam;
    private Dictionary<Vector2, Tile> tiles;
    private float[,] noiseMap;
    public Tile hoveredTile;
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
        tiles = new Dictionary<Vector2, Tile>();
        this.noiseMap = GenerateNoiseMap(NOUSE_MAP_SIZE, NOUSE_MAP_SIZE, 8.0f);

        var startX = UnityEngine.Random.Range(0, NOUSE_MAP_SIZE-width);
        var startY = UnityEngine.Random.Range(0, NOUSE_MAP_SIZE-height);

        for (int x = 0; x < width; x++){
            for (int y = 0; y < height; y++){
                
                //TODO: GET ANY FLOOR/WALL
                Tile randomTile = tileSet.floors[0];
                if (noiseMap[x+startX, y+startY] > 0.6f){
                    randomTile = tileSet.walls[0];
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

        cam.transform.position = new Vector3((float)width/2 -0.5f, (float)height/2 -0.5f, -10);
        GameManager.instance.ChangeState(GameState.SapwnHeroes);
    }

    
    public Tile GetTileAtPosition(Vector2 pos){
        if (tiles.TryGetValue(pos, out var tile)){
            return tile;
        }
        return null;
    }
    public Tile GetTileAtPosition(int x, int y){
        return GetTileAtPosition(new Vector2(x, y));
    }

    public Tile GetAdjacentValidTile(Vector2 startPos, Vector2 endPos){
        List<Tile> tiles = GetAdjacentTiles(endPos);
        Tile bestTile = null;
        foreach (Tile t in tiles){
            if (t.moveType != TileMoveType.NotValid){
                bestTile = t;
            }
            if (t.coordiantes.Equals(startPos)){
                return t;
            }
        }
        return bestTile;
    }

    public List<Tile> GetAdjacentTiles(UnityEngine.Vector2 pos){
        return GetAdjacentTiles((int)pos.x, (int)pos.y);
    }
    public List<Tile> GetAdjacentTiles(int x, int y){
        List<Tile> tiles = new List<Tile>();
        GetAdjecentTile(x, y, 1, 0, tiles);
        GetAdjecentTile(x, y, -1, 0, tiles);
        GetAdjecentTile(x, y, 0, 1, tiles);
        GetAdjecentTile(x, y, 0, -1, tiles);

        return tiles;
    }

    private void GetAdjecentTile(int x, int y, int xOff, int yOff, List<Tile> tiles=null){
        var t = GetTileAtPosition(x+xOff, y+yOff);
        if (tiles != null && t != null){
            tiles.Add(t);
        }
    }
    
    public Tile GetHeroSpawnTile(){
        return tiles.Where(t => t.Key.x < width/2 && t.Value.walkable).OrderBy(t => UnityEngine.Random.value).First().Value;
    }

    public Tile GetEnemySpawnTile(){
        return tiles.Where(t => t.Key.x > width/2 && t.Value.walkable).OrderBy(t => UnityEngine.Random.value).First().Value;
    }
    public List<Tile> GetAllTiles(){
        return tiles.Values.ToList();
    }
    

    public void SetHoveredTile(Tile newTile){
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
    public void MoveHoveredTile(Tile newTile){
        
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

    public List<Tile> GetRectangleTiles(Tile t, int maxX, int maxY){
        List<Tile> validTiles = new List<Tile>();
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
                    Tile tile = GetTileAtPosition(topLeft + new Vector2(x, y));
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
    public List<Tile> GetRadiusTiles(Tile t, int maxDepth){
        var visited = new Dictionary<Tile, int>();
        visited[t] = 0;
        var next = t.GetAdjacentTiles();
        next.ForEach(t => GetRadiusTilesHelper(1, maxDepth, t, visited, t));
        var validMoves = visited.Keys.ToList();
        return validMoves;
    }

    private void GetRadiusTilesHelper(int depth, int max, Tile tile, Dictionary<Tile, int> visited, Tile startTile){
        if (depth >= max ){
            return;
        }
        //enemy's are valid moves but block movement
        if (tile != null && tile.occupiedUnit != null){
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
        next.ForEach(t => GetRadiusTilesHelper(depth + 1, max, t, visited, startTile));
        return;
    }
    public int Distance(Tile t1, Tile t2){
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

  public List<Tile> ShortestPathBetweenTiles(Tile start, Tile end, bool withPathLine){
    if (start == end){
        return new List<Tile>{start};
    }
    List<Tile> visited = new();
    Queue<Tile> toVisit = new();
    Dictionary<Tile, Tile> previousTiles = new();
    Tile current = start;
    previousTiles.Add(current, null);
    do {
        var adjTiles = current.GetAdjacentTiles();
//        Debug.Log(adjTiles.Count);
        foreach (Tile tile in adjTiles){
            if (tile == null){
                continue;
            }
            if (visited.Contains(tile)){
                continue;
            }
            if (withPathLine){
                if (tile.moveType == TileMoveType.NotValid || tile.moveType == TileMoveType.Attack){
                    continue;
                }
            }else{
                if (tile is WallTile || tile.occupiedUnit != null){
                    continue;
                }
            }
            Debug.Log(tile);
            visited.Add(tile);
            toVisit.Enqueue(tile);
            previousTiles.Add(tile, current);
        }
        if (toVisit.Count > 0){
            current = toVisit.Dequeue();
        }
        if (current == end || toVisit.Count == 0){
            List<Tile> finalTiles = new();
            var finalCurr = current;
            while (finalCurr != null){
                if (finalCurr.moveType == TileMoveType.Move || finalCurr == start){
                    //ONLY add tile if its MOVE type;
                    //if its start space, also add
                    finalTiles.Add(finalCurr);
                }
                finalCurr = previousTiles[finalCurr];
            }
            return finalTiles;
        }
    } while (toVisit.Count > 0);
    return null;
  }
}
