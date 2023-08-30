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

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddTile(Tile tile){
        tiles.Add(tile);
        RenderLine();
    }

    public void RemoveTile(Tile tile){
        int index = tiles.IndexOf(tile)+1;
        tiles.RemoveRange(index, tiles.Count() - index);
        RenderLine();
    }

    public void Reset(){
        tiles = new List<Tile>();
        RenderLine();
    }

    public bool IsOnPath(Tile tile){
        return tiles.Contains(tile);
    }

    public Tile GetSecondLastTile(){
        return GetPathTile(tiles.Count() - 2);
    }
    public Tile GetPathTile(int index){
        return tiles[index];
    }
    //TODO: MAKE IT CHANGE TO THE ACTUAL PATH OF HOVERED TILE, NOT JUST THE DIRECTION THE PLAYER MOVED IT !!!
    private void RenderLine(){
        Vector3[] vectors = tiles.Select(t => t.transform.position).ToArray();

        for (int i  = 0; i < vectors.Count(); i++){
            vectors[i].z = -5;
        }
        line.positionCount = vectors.Count();
        line.SetPositions(vectors.ToArray());
    }
}
