using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelData: MonoBehaviour {
    //public int nextSceneID;
    public bool startLevel;
    public int numberOfEnemies;
    public LevelTheme levelTheme;
    public static bool hasStarted = false;
    public void Awake(){
        if (!startLevel && !hasStarted){
            SceneManager.LoadScene("StartScene");
        }else{
            hasStarted = true;
        }
    }
}

public enum LevelTheme {
    MountainRange,
    Castle
} 