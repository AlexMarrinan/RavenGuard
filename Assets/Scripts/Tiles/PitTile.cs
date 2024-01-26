using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PitTile : MultiLayerTile
{
    private void Awake(){
        isWalkable = false;
        isShootable = true;
    }
}
