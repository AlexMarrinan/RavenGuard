using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MultiLayerTile : BaseTile
{
    [SerializeField] protected SpriteRenderer backgroundRenderer;
    [SerializeField] private Color baseColor, offsetColor;

    private void Awake(){
        isWalkable = false;
        isShootable = true;
    }
    public override void Init(int x, int y)
    {
        base.Init(x, y);
        backgroundRenderer.color = IsOffset(x, y) ? offsetColor : baseColor;
    }
    private bool IsOffset(int x, int y){
        return (x+y) % 2 == 1; 
    }
}
