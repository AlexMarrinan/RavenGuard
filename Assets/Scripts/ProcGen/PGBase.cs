using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "New PG Base", menuName = "Procedural Geneartion/Base")]
public class PGBase : ScriptableObject
{
    public int height, width;
    public TileEditorType[] grid;
    
}

public enum TileEditorType {
    None,
    Grass,
    Mountain,
    Water,
    Forest,
    Bridge,
}