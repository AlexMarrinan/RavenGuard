using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New TileSet", menuName = "Tile Set")]

public class TileSet : ScriptableObject
{
    public List<Sprite> floors;
    public List<Sprite> floorXWalls;
    public List<Sprite> walls;
    public List<Sprite> water;
    public List<Sprite> bridge;
    public List<Sprite> forest;

    public Sprite GetRandomWall(){
        int randomIndex = UnityEngine.Random.Range(0, walls.Count);    
        return walls[randomIndex];
    }
    public Sprite GetRandomFloor(){
        int randomIndex = UnityEngine.Random.Range(0, floors.Count);    
        return floors[randomIndex];
    }
}
