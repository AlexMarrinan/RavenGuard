using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FenceTile : MultiLayerTile
{
    private void Awake(){
        isWalkable = false;
        isShootable = true;
    }
}
