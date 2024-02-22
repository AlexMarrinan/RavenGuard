using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameState gameState;
    public Canvas mainCanvas;
    public Camera mainCamera;
    private Vector3 newCameraPos;
    public float camAutoSpeed = 8f;
    public float cameraSensitivity = 2f;
    private bool usingMouse = false;
    public GameState startState;
    public LevelData levelData;
    public bool levelFinished = false;
    // Start is called before the first frame update
    void Start()
    {
        levelData = FindObjectOfType<LevelData>();
        DontDestroyOnLoad(gameObject);
        instance = this;
        gameState = startState;
        newCameraPos = mainCamera.transform.position;
        GridManager.instance.LoadAssets();
        ChangeState(gameState);
    }

    // Update is called once per frame
    void Update()
    {
        if (mainCamera != null && newCameraPos != mainCamera.transform.position){
            var trans = mainCamera.transform;
            trans.position = Vector3.Lerp(trans.position, newCameraPos, camAutoSpeed * Time.deltaTime);
        }
    }
    
    //Change game state
    public void ChangeState(GameState newState){
        gameState = newState;
        if (levelData.startLevel){
            LoadNextLevel();
            return;
        }
        switch(newState){
            case GameState.MainMenu:
                break;
            case GameState.GenerateGrid:
                GridManager.instance.GenerateGrid();
                break;
            case GameState.SapwnHeroes:
                UnitManager.instance.SpawnHeroes();
                break;
            case GameState.SpawnEnemies:
                UnitManager.instance.SpawnEnemies();
                break;
            case GameState.HeroesTurn:
                TurnManager.instance.BeginHeroTurn();
                break;
            case GameState.EnemiesTurn:
                TurnManager.instance.BeginEnemyTurn();
                break;
            default:
                break;
        }
    }

    public void SetUsingMouse(bool usingMouseNew){
        usingMouse = usingMouseNew;
    }

    public void PanCamera(Vector2 v){
        if (MenuManager.instance.InPauseMenu() || MenuManager.instance.menuState == MenuState.Battle){
            return;
        }
        newCameraPos = (Vector3)v + new Vector3(0, 0, -10);
    }
    public void PanCameraInDirection(Vector2 v){
        if (MenuManager.instance.InPauseMenu() || MenuManager.instance.menuState == MenuState.Battle){
            return;
        }
        newCameraPos = (Vector3)v*cameraSensitivity + mainCamera.transform.position;
    }

    public void LookCameraAtHighlight(){
        GameObject highlightObject = MenuManager.instance.highlightObject.gameObject;
        if (highlightObject.activeSelf && !usingMouse ){
           PanCamera(highlightObject.transform.position);
        }
    }

    public void ZoomCamera(float amount){
        if (MenuManager.instance.InPauseMenu() || gameState == GameState.BattleScene){
            return;
        }
        mainCamera.orthographicSize += amount;
        if (mainCamera.orthographicSize <= 2 || mainCamera.orthographicSize >= 12){
            mainCamera.orthographicSize -= amount;
        }
    }

    private IEnumerator LoadNextLevelAsync(){
        MenuManager.instance.ShowStartText("Loading level...", true);
        yield return new WaitForSeconds(0.8f);
        yield return SceneManager.LoadSceneAsync(levelData.nextLevelName);
        levelData = FindObjectOfType<LevelData>();
        // ChangeState(GameState.HeroesTurn);
        GridManager.instance.GenerateGrid();
        TurnManager.instance.BeginHeroTurn();
        yield return null;
        //mainCamera = FindObjectOfType<Camera>();
    }

    public void LoadNextLevel()
    {
        levelFinished = false;
        MenuManager.instance.CloseMenus();
        foreach (BaseUnit unit in UnitManager.instance.GetAllEnemies()){
            UnitManager.instance.DeleteUnit(unit, false);
        }
        foreach (BaseUnit unit in UnitManager.instance.GetAllHeroes()){
            unit.health = unit.maxHealth;
        }
        StartCoroutine(LoadNextLevelAsync());
    }
}

public enum GameState{
    MainMenu,
    GenerateGrid,
    SapwnHeroes,
    SpawnEnemies,
    HeroesTurn,
    EnemiesTurn,
    BattleScene    
}
