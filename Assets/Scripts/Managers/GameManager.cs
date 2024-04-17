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
    public List<string> levelNames;
    public List<string> bossLevelNames;
    public int cameraMarginX = 4;
    public int cameraMarginY = 1;
    public Color heroColor;
    public Color enemyColor;
    // Start is called before the first frame update
    void Awake(){
        if (instance != null){
            Debug.Log("another instance");
            Destroy(instance.gameObject);
        }
        instance = this;
    }
    void Start()
    {
        levelData = FindObjectOfType<LevelData>();
        DontDestroyOnLoad(gameObject);
        gameState = startState;
        newCameraPos = mainCamera.transform.position;
        ChangeState(gameState);
    }

    // Update is called once per frame
    void Update()
    {
        if (mainCamera != null && newCameraPos != mainCamera.transform.position && !SkillManager.instance.selectingSkill){
            var trans = mainCamera.transform;
            trans.position = Vector3.Lerp(trans.position, newCameraPos, camAutoSpeed * Time.deltaTime);
        }
    }
    
    //Change game state
    public void ChangeState(GameState newState){
        gameState = newState;
        if (levelData == null){
            return;
        }
        if (levelData.startLevel){
//            Debug.Log("Start level found!");
            MenuManager.instance.ToggleUnitSelectionMenu();
            //LoadNextLevel();
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
        if (MenuManager.instance.InMenu() || MenuManager.instance.menuState == MenuState.Battle){
            return;
        }
        newCameraPos = (Vector3)v + new Vector3(0, 0, -10);
    }
    public void PanCameraInDirection(Vector2 v){
        if (MenuManager.instance.InMenu() || MenuManager.instance.menuState == MenuState.Battle){
            return;
        }
        newCameraPos = (Vector3)v*cameraSensitivity + mainCamera.transform.position;
    }
    public void SetCameraPos(Vector2 newPos){
        newCameraPos = newPos;
        mainCamera.transform.position = newCameraPos;
    }
    public void LookCameraAtHighlight(){
        GameObject highlightObject = MenuManager.instance.highlightObject.gameObject;

        if (highlightObject.activeSelf && !usingMouse ){
            Vector2 movePoint = highlightObject.transform.position;
            Vector2 cameraPos = mainCamera.transform.position;
            Vector2 min = GridManager.instance.minPos;
            Vector2 max = GridManager.instance.maxPos;

            //CLAMP X AXIS OF CAMERA; 
            if (movePoint.x - cameraMarginX < min.x){
                movePoint.x = min.x + cameraMarginX;
            }
            if (movePoint.x + cameraMarginX >= max.x){
                movePoint.x = max.x - cameraMarginX;
            }

            //CLAMP X AXIS OF CAMERA; 
            if (movePoint.y - cameraMarginY < min.y){
                movePoint.y = min.y + cameraMarginY;
            }
            if (movePoint.y + cameraMarginY >= max.y){
                movePoint.y = max.y - cameraMarginY;
            }
            PanCamera(movePoint);
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

    private IEnumerator LoadLevelAsync(string newScene){
        levelFinished = false;
        Debug.Log(newScene);
        MenuManager.instance.CloseMenus();
        MenuManager.instance.ShowStartText("Loading...", true);
        yield return new WaitForSeconds(0.1f);
        yield return SceneManager.LoadSceneAsync(newScene);
        if (newScene != "OverworldMap"){
            levelData = FindObjectOfType<LevelData>();
            GridManager.instance.GenerateGrid();
            TurnManager.instance.BeginHeroTurn();
            yield return null;
        }
        yield return null;
        //mainCamera = FindObjectOfType<Camera>();
    }

    public void LoadOverworldMap()
    {
        foreach (BaseUnit unit in UnitManager.instance.GetAllEnemies()){
            UnitManager.instance.DeleteUnit(unit, false);
        }
        foreach (BaseUnit unit in UnitManager.instance.GetAllHeroes()){
            unit.Reset();
        }
        UnitManager.instance.ShowUnitHealthbars(false);
        MenuManager.instance.CloseMenus();
        OverworldMapManager.instance.ShowMap();
        MenuManager.instance.EnableInventorySwapping();

    }
    public void LoadCombatLevel()
    {
        MenuManager.instance.DisableInventorySwapping();
        OverworldMapManager.instance.ShowMap(false);
        string levelName = GetRandomLevelName();
        StartCoroutine(LoadLevelAsync(levelName));
    }
    public void LoadShopLevel()
    {
       //TODO: ADD SHOP !!!
    }

    private string GetRandomLevelName(){
        Debug.Log(levelNames.Count);
        int index = UnityEngine.Random.Range(0, levelNames.Count);
        return levelNames[index];
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
