using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GridManager : MonoBehaviour
{
    public static GridManager instance;
    [SerializeField] private int width, height;
    [SerializeField] private Tile grassTile, mountainTile;
    [SerializeField] private Transform cam;
    private Dictionary<Vector2, Tile> tiles;
    public Tile hoveredTile;

    void Awake(){
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {

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
        for (int x = 0; x < width; x++){
            for (int y = 0; y < height; y++){

                //TODO: MAKE ACTUALLY SPAWN TILE CORRECTLY
                var randomTile = Random.Range(0, 6) == 3 ? mountainTile : grassTile;

                var newTile = Instantiate(randomTile, new Vector3(x,y), Quaternion.identity);
                newTile.name = $"Tile {x} {y}";
                
                newTile.Init(x, y);
                var pos = new Vector2(x,y);
                tiles[pos] = newTile;
                newTile.coordiantes = pos;
            }
        }

        cam.transform.position = new Vector3((float)width/2 -0.5f, (float)height/2 -0.5f, -10);
        SelectHoveredTile(GetTileAtPosition(0,0));
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
            if (t.isValidMove){
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
        return tiles.Where(t => t.Key.x < width/2 && t.Value.walkable).OrderBy(t => Random.value).First().Value;
    }

    public Tile GetEnemySpawnTile(){
        return tiles.Where(t => t.Key.x > width/2 && t.Value.walkable).OrderBy(t => Random.value).First().Value;
    }
    public List<Tile> GetAllTiles(){
        return tiles.Values.ToList();
    }
    

    public void SelectHoveredTile(Tile newTile){
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
        BaseUnit sUnit = UnitManager.instance.selectedUnit;

        //if a unit is selected, dont move to tiles that arent valid moves
        if (sUnit != null && !newTile.isValidMove){
            //if the next tile is the tile occupied by the selected unit, move one past it
            if (newTile.occupiedUnit == sUnit){
                newTile = GetTileAtPosition(newTile.coordiantes + direction);
                if (newTile == null){
                    return;
                }
                //remove hovered tile from path
                PathLine.instance.Reset();
                PathLine.instance.AddTile(UnitManager.instance.selectedUnit.occupiedTile);
            }else{
                return;
            }
        }

        if (!newTile.isTerrainWalkable()){
            return;
        }
        SelectHoveredTile(newTile);
    }
    public void SelectHoveredTile(){
        hoveredTile.OnSelectTile();
    }
}
