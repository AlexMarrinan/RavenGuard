using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "New Level Base", menuName = "Levels/Base")]
public class LevelBase : ScriptableObject
{
    public LayerSize size;
    public int height, width;
    public int numRivers, numPonds, numForests, numMountains;
    [HideInInspector]
    public  Array2D<TileEditorType> array;
    [HideInInspector]
    public Array2D<LayerSize> chestArray;
    [HideInInspector]
    public Array2D<SpawnFaction> spawnArray;

    public TileEditorType GetTileType(int layerX, int layerY)
    {
        return array.Get(layerX, layerY);
    }

    public void Resize()
    {
        chestArray = new Array2D<LayerSize>(width, height);
        array = new Array2D<TileEditorType>(width, height);
        spawnArray = new Array2D<SpawnFaction>(width, height);
    }

    public void SetHeight(int h)
    {
        array.Height = h;
        chestArray.Height = h;
        spawnArray.Height = h;
    }
    public void SetWidth(int w)
    {
        array.Width = w;
        chestArray.Width = w;
        spawnArray.Width = w;
    }
}
public enum LayerSize {
    None,
    Small,
    Medium,
    Large
}
public enum LEDrawLayer {
    Standard,
    Chest,
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
    BlueEither,
    BlueMelee,
    BlueRanged,
    OrangeEither,
    OrangeMelee,
    OrangeRanged,
    OrangeBoss,
}