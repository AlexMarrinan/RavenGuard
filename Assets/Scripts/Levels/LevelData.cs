using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;

public class LevelData: MonoBehaviour {
    //public int nextSceneID;
    public string nextLevelName;
    public bool startLevel;
    public int numberOfEnemies;
    public List<BaseUnit> possibleEnemies;
}
