using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class GridManager : MonoBehaviour
{
    public static GridManager instance;
    private int width, height;
    [SerializeField] private TileSet tileSet;

    //Predabs
    [SerializeField] private FloorTile floorPrefab;
    [SerializeField] private WallTile wallPrefab;
    [SerializeField] private Transform cam;
    private Dictionary<Vector2, BaseTile> tiles;
    private Dictionary<Vector2, TileEditorType> tileTypes;
    public List<Vector2> team1spawns, team2spawns;

    public BaseTile hoveredTile;
    private const int NOUSE_MAP_SIZE = 500;
    public List<PGBase> ponds;
    public List<PGBase> rivers;
    public List<PGBase> mountains;
    public List<PGBase> forests;
    public List<PGBase> bases;
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
        bases =  Resources.LoadAll<PGBase>("ProcGen/Bases").ToList();
        ponds = Resources.LoadAll<PGBase>("ProcGen/Ponds").ToList();
        rivers =  Resources.LoadAll<PGBase>("ProcGen/Rivers").ToList();
        forests =  Resources.LoadAll<PGBase>("ProcGen/Forests").ToList();
        mountains =  Resources.LoadAll<PGBase>("ProcGen/Mountains").ToList();
    }
    //Generates a grid of width x height of the tilePrefab
    // public void GenerateGridOLD(){
    //     tiles = new Dictionary<Vector2, BaseTile>();
    //     this.noiseMap = GenerateNoiseMap(NOUSE_MAP_SIZE, NOUSE_MAP_SIZE, 8.0f);

    //     var startX = UnityEngine.Random.Range(0, NOUSE_MAP_SIZE-width);
    //     var startY = UnityEngine.Random.Range(0, NOUSE_MAP_SIZE-height);

    //     for (int x = 0; x < width; x++){
    //         for (int y = 0; y < height; y++){
    //             // float scale = 1f;//0.16f;
    //             // float actualX = (float)(x * scale);
    //             // float actualY = (float)(y * scale);

    //             //TODO: GET ANY FLOOR/WALL
    //             BaseTile randomTile = floorPrefab;
    //             randomTile.SetBGSprite(tileSet.GetRandomFloor());

    //             if (noiseMap[x+startX, y+startY] > 0.6f){
    //                 randomTile = wallPrefab;
    //                 randomTile.SetBGSprite(tileSet.walls[0]);
    //             }


                
    //             // if (randomTile is FloorTile){
    //             //     int idk = Random.Range(0, 50);
    //             //     if (idk == 0){
    //             //         randomTile = tileSet.pits[0];
    //             //     }else if (idk == 1){
    //             //         randomTile = tileSet.fences[0];
    //             //     }
    //             // }                
                
    //             // //TODO: MAKE ACTUALLY SPAWN TILE CORRECTLY
    //             // var randomTile = Random.Range(0, 6) == 3 ? mountainTile : grassTile;
                
    //             var newTile = Instantiate(randomTile, new Vector3(x,y), Quaternion.identity);
    //             newTile.name = $"Tile {x} {y}";
                
    //             newTile.Init(x, y);
    //             var pos = new Vector2(x,y);
    //             tiles[pos] = newTile;
    //             newTile.coordiantes = pos;
    //         }
    //     }

    //     foreach (BaseTile t in tiles.Values){
    //         if (t is FloorTile){
    //             SetGrassTileSprites(t as FloorTile);
    //         }else if (t is WallTile){
    //             SetMountainTileSprites(t as WallTile);
    //         }
    //     }
    //     cam.transform.position = new Vector3((float)width/2 -0.5f, (float)height/2 -0.5f, -10);
    //     GameManager.instance.ChangeState(GameState.SapwnHeroes);
    // }

    public void GenerateGrid()
    {
        tileTypes = new Dictionary<Vector2, TileEditorType>();

        Dictionary<Vector2, LayerSize> pondPositions = new();
        Dictionary<Vector2, LayerSize> riverPositions = new();
        Dictionary<Vector2, LayerSize> forestPositions = new();
        Dictionary<Vector2, LayerSize> mountainPositions = new();

        team1spawns = new();
        team2spawns = new();

        int randIndex = UnityEngine.Random.Range(0, bases.Count);
        PGBase pgb = bases[randIndex];
        var newArray = new Array2D<TileEditorType>(pgb.width, pgb.height);
        var newPondArray = new Array2D<LayerSize>(pgb.width, pgb.height);
        var newForestArray = new Array2D<LayerSize>(pgb.width, pgb.height);
        var newRiverArray = new Array2D<LayerSize>(pgb.width, pgb.height);
        var newMountainArray = new Array2D<LayerSize>(pgb.width, pgb.height);
        var newSpawnArray = new Array2D<SpawnFaction>(pgb.width, pgb.height);

        //        Debug.Log("new array" + (newArray.Width, newArray.Height));
        newArray.DeepCopy(pgb.array);
        newPondArray.DeepCopy(pgb.pondArray);
        newForestArray.DeepCopy(pgb.forestArray);
        newRiverArray.DeepCopy(pgb.riverArray);
        newMountainArray.DeepCopy(pgb.mountainArray);
        newSpawnArray.DeepCopy(pgb.spawnArray);
        //      Debug.Log("new array" + (newArray.Width, newArray.Height));
        //Randomly Flip Layout
        if (UnityEngine.Random.Range(0, 2) == 1)
        {
            newArray.FlipX();
            newPondArray.FlipX();
            newForestArray.FlipX();
            newRiverArray.FlipX();
            newMountainArray.FlipX();
            newSpawnArray.FlipX();
        }
        else if (UnityEngine.Random.Range(0, 2) == 1)
        {
            newArray.FlipY();
            newPondArray.FlipY();
            newForestArray.FlipY();
            newRiverArray.FlipY();
            newMountainArray.FlipY();
            newSpawnArray.FlipY();
        }

        //Randomly Rotate Layout
        int numRotates = UnityEngine.Random.Range(0, 4);
        for (int i = 0; i < numRotates; i++)
        {
            newArray.Rotate();
            newPondArray.Rotate();
            newForestArray.Rotate();
            newRiverArray.Rotate();
            newMountainArray.Rotate();
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
                var mtn = newMountainArray.Get(x, y);
                var riv = newRiverArray.Get(x, y);
                var pnd = newPondArray.Get(x, y);
                var frs = newForestArray.Get(x, y);
                var spawn = newSpawnArray.Get(x,y);
                Debug.Log(pnd);
                if (mtn != LayerSize.None)
                {
                    mountainPositions.Add(pos, mtn);
                }
                if (riv != LayerSize.None)
                {
                    riverPositions.Add(pos, mtn);
                }
                if (pnd != LayerSize.None)
                {
                    pondPositions.Add(pos, mtn);
                }
                if (frs != LayerSize.None)
                {
                    forestPositions.Add(pos, mtn);
                }
                if (spawn == SpawnFaction.Team1){
                    team1spawns.Add(pos);
                }else if (spawn ==  SpawnFaction.Team2){
                    team2spawns.Add(pos);
                }
                tileTypes[pos] = newArray.grid[newy * width + x];
            }
        }

        //        Debug.Log(rivers.Count);

        PlaceRandomLayer(pondPositions, ponds, pgb.numPonds);
        PlaceRandomLayer(riverPositions, rivers, pgb.numRivers);
        PlaceRandomLayer(forestPositions, forests, pgb.numForests);
        PlaceRandomLayer(mountainPositions, mountains, pgb.numMountains);

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

    private void PlaceRandomLayer(Dictionary<Vector2, LayerSize> layerPositions, List<PGBase> layers, int maxLayers)
    {
        int randomNum = UnityEngine.Random.Range(0, maxLayers + 1);
        if (layerPositions.Count <= 0){
            return;
        }
        for (int _ = 0; _ < randomNum; _++) {
            int layerIndex = UnityEngine.Random.Range(0, layers.Count);
            int posIndex = UnityEngine.Random.Range(0, layerPositions.Keys.Count);
            Debug.Log("Num: " + randomNum);
            Debug.Log("Layers: " + layerPositions.Count);
            PGBase layer = layers[layerIndex];
            OverlayLayer(layer, layerPositions.Keys.ToList()[posIndex]);
        }
    }

    //TODO: MAKE 2D ARRAY CLASS THAT DOES THESE THINGS IN AN ORGANAIZED WAY
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
    private BaseTile GetTileTypeAtPos(PGBase pgb, int x, int y)
    {
        TileEditorType type = pgb.GetTileType(x, y);
        return GetTileFromType(type);
    }

    private void OverlayPond(PGBase pond)
    {
        int randX = UnityEngine.Random.Range(2-pond.width, width-2);
        int randY = UnityEngine.Random.Range(2-pond.height, height-2);
        // Debug.Log("Pond at: " + (randX, randY));
        // Debug.Log("Total dem " + (width, height));
        // Debug.Log("Dem: " + (pond.width, pond.height));

        OverlayLayer(pond, randX, randY);
    }
    private void OverlayRiver(PGWater river)
    {
        int randX, randY;
        var tempList = new int[]{-2, -1, 0, 1, 2};
        if (river.horizontal){
            randX = UnityEngine.Random.Range(0, tempList.Length);
            randY = UnityEngine.Random.Range(2-river.height, height-2);
        }else{
            randX = UnityEngine.Random.Range(2-river.width, width-2);
            randY = UnityEngine.Random.Range(0, tempList.Length);
        }
        OverlayLayer(river, randX, randY);
    }
    private void OverlayLayer(PGBase layer, Vector2 pos){
        OverlayLayer(layer, (int)pos.x, (int)pos.y);
    }
    private void OverlayLayer(PGBase layer, int startX, int startY)
    {   
        Debug.Log(layer.name);
        for (int x = startX, layerX = 0; x < startX + layer.width && x < width && layerX < layer.width; x++, layerX++){
            if (x >= 0){
                for (int y = startY, layerY = 0; y < startY + layer.height && y < height && layerY < layer.height; y++, layerY++){
                    // Debug.Log("Pos: " + (x, y));
                    // Debug.Log("Layer Pos: " + (layerX, layerY));

                    if (y >= 0){
                        TileEditorType tileEditorType = layer.GetTileType(layerX, layerY);
//                        Debug.Log((layerX, layerY, tileEditorType));
                        if (tileEditorType != TileEditorType.None){ 
                            Vector2 pos = new(x, y);
                            tileTypes[pos] = tileEditorType;
                        }
                    }
                }
            }
        }
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
        int idx = GetBlendTileIndexOLD(wt);
        if (idx == -1){
            wt.SetFGSprite(tileSet.GetRandomWall());
        }else{
            wt.SetFGSprite(tileSet.floorXWalls[idx]);
            wt.SetBGSprite(tileSet.GetRandomFloor());
        }
    }
    //TODO: MAKE NOT ASS HOLY SHIT
    private int GetBlendTileIndexOLD(BaseTile bt){
        TileEditorType tileEditorType = bt.editorType;
        Vector2 pos = bt.coordiantes;
        var up = GetAdjecentTile((int)pos.x, (int)pos.y, 0, 1);
        var down = GetAdjecentTile((int)pos.x, (int)pos.y, 0, -1);
        var left = GetAdjecentTile((int)pos.x, (int)pos.y, -1, 0);
        var right = GetAdjecentTile((int)pos.x, (int)pos.y, 1, 0);

        var u = up != null && up.editorType != tileEditorType;
        var d = down != null && down.editorType != tileEditorType;
        var l = left != null && left.editorType != tileEditorType;
        var r = right != null && right.editorType != tileEditorType;

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

        var ul = upleft != null && upleft.editorType != tileEditorType;
        var dl = downleft != null && downleft.editorType != tileEditorType;
        var ur = upright != null && upright.editorType != tileEditorType;
        var dr = downright != null && downright.editorType != tileEditorType;

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
        ft.SetBGSprite(tileSet.forest[0]);
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
    
    public BaseTile GetSpawnTile(bool team1){
//        Debug.Log(tiles.Values.Count);
        List<Vector2> spawnPositions = GridManager.instance.team1spawns;
        if (!team1){
            spawnPositions = GridManager.instance.team2spawns;
        }
        int randomIndex = UnityEngine.Random.Range(0, spawnPositions.Count);
        Vector2 pos = spawnPositions[randomIndex];
        var tile = GridManager.instance.GetTileAtPosition(spawnPositions[randomIndex]);
        spawnPositions.Remove(pos);
        return tile;
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
                int max = rangedUnit.maxMoveAmount - 1;
                int extraPathLength = distance - max;
                Debug.Log("pathLength " + extraPathLength);

                if (extraPathLength >= 0){
                    max = rangedUnit.rangedWeapon.maxRange - 1;
                    int range = max - distance; //+ rangedUnit.rangedWeapon.minRange;
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