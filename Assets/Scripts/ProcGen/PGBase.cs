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
    public  Array2D<TileEditorType> array;
    public Array2D<LayerSize> riverArray, pondArray, mountainArray, forestArray;

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
    }

    public void SetHeight(int h)
    {
        array.Height = h;
        riverArray.Height = h;
        pondArray.Height = h;
        mountainArray.Height = h;
        forestArray.Height = h;
    }
    public void SetWidth(int w)
    {
        array.Width = w;
        riverArray.Width = w;
        pondArray.Width = w;
        mountainArray.Width = w;
        forestArray.Width = w;
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
}

public enum TileEditorType {
    None,
    Grass,
    Mountain,
    Water,
    Forest,
    Bridge,
}