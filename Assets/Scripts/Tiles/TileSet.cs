using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New TileSet", menuName = "Tile Set")]

public class TileSet : ScriptableObject
{
    public List<FloorTile> floors;
    public List<WallTile> walls;
    public List<PitTile> pits;
    public List<FenceTile> fences;
}
