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

    /// <summary>
    /// Resets the paths tiles, as a list
    /// </summary>
    public List<BaseTile> GetPath(){
        return tiles;
    }

    /// <summary>
    /// Resets the path line
    /// </summary>
    public void Reset(){
        line.positionCount = 0;
        Vector3[] vectors = {};
        line.SetPositions(vectors);
    }

    /// <summary>
    /// Gets the last tile on the path
    /// </summary>
    public BaseTile GetLastTile(){
        if (tiles.Count == 0){
            return null;
        }
        return GetPathTile(tiles.Count() - 1);
    }

    /// <summary>
    /// Gets a tile at a specific index along the path
    /// </summary>
    /// <param name="index">index of tile</param>
    public BaseTile GetPathTile(int index){
        return tiles[index];
    }

    /// <summary>
    /// Render the pathline between a start and end BaseTile
    /// </summary>
    /// <param name="start">Start Tile</param>
    /// <param name="end">End Tile</param>'
    public void RenderLine(BaseTile start, BaseTile end){
        
        //Gets the shortest path between the start and end tile
        tiles = GridManager.instance.ShortestPathBetweenTiles(start, end, true);

        //Get the positions of each tile in the path
        Vector3[] positions = tiles.Select(t => t.transform.position).ToArray();

        for (int i  = 0; i < positions.Count(); i++){
            //Hacky way to get the line to draw behind other objects, i think?
            positions[i].z = -5;
        }

        //Set the positions of each tile on the path
        line.positionCount = positions.Count();
        line.SetPositions(positions.ToArray());

        //Reverse the tiles from the shortest path to save for later reference
        tiles.Reverse();
    }
}
