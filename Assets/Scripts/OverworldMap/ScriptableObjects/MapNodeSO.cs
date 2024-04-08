using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapNode", menuName = "OverworldMap/MapNode", order = 0)]
public class MapNodeSO : ScriptableObject
{
    public List<Sprite> unclearedSprites;
    public List<Sprite> clearedSprites;
    public List<Color> background;
}