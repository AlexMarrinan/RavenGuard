using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class PathLine : MonoBehaviour
{
    private LineRenderer line;
    private List<Tile> tiles = new List<Tile>();
    public float lineWidth;

    public static PathLine instance;
    void Awake()
    {
        instance = this;
        line = GetComponent<LineRenderer>();
        line.SetWidth(lineWidth, lineWidth);
    }

    public void AddTile(Tile tile){
        if (tile.moveType != TileMoveType.Move && tile != UnitManager.instance.selectedUnit.occupiedTile){
            return;
        }
        tiles.Add(tile); 
    }

    public void RemoveTile(Tile tile){
        int index = tiles.IndexOf(tile)+1;
        tiles.RemoveRange(index, tiles.Count() - index);
    }

    public void Reset(){
        line.positionCount = 0;
        Vector3[] vectors = {};
        line.SetPositions(vectors);

    }

    public bool IsOnPath(Tile tile){
        return tiles.Contains(tile);
    }
    public Tile GetLastTile(){
        return GetPathTile(tiles.Count() - 1);
    }
    public Tile GetPathTile(int index){
        Debug.Log(index);
        Debug.Log(tiles.Count);
        return tiles[index];
    }
    //TODO: MAKE IT CHANGE TO THE ACTUAL PATH OF HOVERED TILE, NOT JUST THE DIRECTION THE PLAYER MOVED IT !!!
    public void RenderLine(Tile start, Tile end){
        //Vector3[] vectors = tiles.Select(t => t.transform.position).ToArray();
        tiles = GridManager.instance.ShortestPathBetweenTiles(start, end);
        Debug.Log(tiles);
        Vector3[] vectors = tiles.Select(t => t.transform.position).ToArray();

        for (int i  = 0; i < vectors.Count(); i++){
            vectors[i].z = -5;
        }
        line.positionCount = vectors.Count();
        line.SetPositions(vectors.ToArray());

        tiles.Reverse();
    }
}
