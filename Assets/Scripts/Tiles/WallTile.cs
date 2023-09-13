using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallTile : Tile
{
    private void Awake(){
        isWalkable = false;
        isShootable = false;
    }
}
