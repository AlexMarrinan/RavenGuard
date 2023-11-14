using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Unit", menuName = "Scriptable Unit")]
public class ScriptableUnit : ScriptableObject
{
    public UnitFaction faction;
    public BaseUnit unitPrefab;
}

public enum UnitFaction{
    Hero,
    Enemy,
    Both
}
