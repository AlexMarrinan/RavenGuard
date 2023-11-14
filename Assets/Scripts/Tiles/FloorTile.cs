using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorTile : BaseTile
{
    [SerializeField] private Color baseColor, offsetColor;
    private void Awake(){
        isWalkable = true;
        isShootable = true;
    }
    public override void Init(int x, int y)
    {
        renderer.color = IsOffset(x, y) ? offsetColor : baseColor;
    }
    private bool IsOffset(int x, int y){
        return (x+y) % 2 == 1; 
    }
}
