using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallTile : BaseTile
{
    private void Awake(){
        isWalkable = false;
        isShootable = false;
    }
}
