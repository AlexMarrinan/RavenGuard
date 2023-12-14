using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "New PG Base", menuName = "Procedural Geneartion/Base")]
public class PGBase : ScriptableObject
{
    public LayerSize size;
    public int height, width;
    public int numRivers, numPonds, numForests, numMountains;
    [HideInInspector]
    public  Array2D<TileEditorType> array;
    [HideInInspector]
    public Array2D<LayerSize> riverArray, pondArray, mountainArray, forestArray;
    //[HideInInspector]
    public Array2D<SpawnFaction> spawnArray;

    public TileEditorType GetTileType(int layerX, int layerY)
    {
        return array.Get(layerX, layerY);
    }

    public void Resize()
    {
        array = new Array2D<TileEditorType>(width, height);
        riverArray = new Array2D<LayerSize>(width, height);
        pondArray = new Array2D<LayerSize>(width, height);
        mountainArray = new Array2D<LayerSize>(width, height);
        forestArray = new Array2D<LayerSize>(width, height);
        spawnArray = new Array2D<SpawnFaction>(width, height);
    }

    public void SetHeight(int h)
    {
        array.Height = h;
        riverArray.Height = h;
        pondArray.Height = h;
        mountainArray.Height = h;
        forestArray.Height = h;
        spawnArray.Height = h;
    }
    public void SetWidth(int w)
    {
        array.Width = w;
        riverArray.Width = w;
        pondArray.Width = w;
        mountainArray.Width = w;
        forestArray.Width = w;
        spawnArray.Width = w;
    }
}
public enum LayerSize {
    None,
    Small,
    Medium,
    Large
}
public enum PGDrawLayer {
    Standard,
    River,
    Pond,
    Forest,
    Mountain,
    Spawns
}

public enum TileEditorType {
    None,
    Grass,
    Mountain,
    Water,
    Forest,
    Bridge,
}

public enum SpawnFaction {
    None,
    Team1,
    Team2
}