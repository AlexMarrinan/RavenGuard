using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "New PG Water", menuName = "Procedural Geneartion/Water")]
public class PGWater : PGBase
{
    public WaterType waterType;
    public int minBridges, maxBridges;
}

public enum WaterType {
    River,
    Pond,
}