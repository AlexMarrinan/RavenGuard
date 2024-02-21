using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class PathLine : MonoBehaviour
{
    private LineRenderer line;
    private List<BaseTile> tiles = new List<BaseTile>();
    public float lineWidth;

    public static PathLine instance;
    void Awake()
    {
        instance = this;
        line = GetComponent<LineRenderer>();
        line.SetWidth(lineWidth, lineWidth);
    }

    public List<BaseTile> GetPath(){
        return tiles;
    }
    public void AddTile(BaseTile tile){
        if (tile.moveType != TileMoveType.Move && tile != UnitManager.instance.selectedUnit.occupiedTile){
            return;
        }
        tiles.Add(tile); 
    }

    public void RemoveTile(BaseTile tile){
        int index = tiles.IndexOf(tile)+1;
        tiles.RemoveRange(index, tiles.Count() - index);
    }

    public void Reset(){
        line.positionCount = 0;
        Vector3[] vectors = {};
        line.SetPositions(vectors);
    }

    public bool IsOnPath(BaseTile tile){
        return tiles.Contains(tile);
    }
    public BaseTile GetLastTile(){
        if (tiles.Count == 0){
            return null;
        }
        return GetPathTile(tiles.Count() - 1);
    }
    public BaseTile GetPathTile(int index){
//        Debug.Log(index);
//        Debug.Log(tiles.Count);
        return tiles[index];
    }
    public void RenderLine(BaseTile start, BaseTile end){
        //Vector3[] vectors = tiles.Select(t => t.transform.position).ToArray();
        tiles = GridManager.instance.ShortestPathBetweenTiles(start, end, true);
        Vector3[] vectors = tiles.Select(t => t.transform.position).ToArray();

        for (int i  = 0; i < vectors.Count(); i++){
            vectors[i].z = -5;
        }
        line.positionCount = vectors.Count();
        line.SetPositions(vectors.ToArray());

        tiles.Reverse();
    }
}
