using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "New PG Base", menuName = "Procedural Geneartion/Base")]
public class PGBase : ScriptableObject
{
    public int height, width;
    public int numRivers, numPonds, numForests;
    public TileEditorType[] grid;

    public TileEditorType GetType(Vector2 pos){
        return GetType((int)pos.x, (int)pos.y);
    }
    public TileEditorType GetType(int x, int y){
        //Flips height 
        //(bottom left corner is 0,0 in game);
        //(top left corner is 0,0 in editor);
        int newy = this.height-1-y;
        return grid[newy*width+x];
    }
    
}

public enum TileEditorType {
    None,
    Grass,
    Mountain,
    Water,
    Forest,
    Bridge,
}