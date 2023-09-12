using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        gameState = startState;
        newCameraPos = mainCamera.transform.position;
        ChangeState(gameState);
    }

    // Update is called once per frame
    void Update()
    {
        if (newCameraPos != mainCamera.transform.position){
            var trans = mainCamera.transform;
            trans.position = Vector3.Lerp(trans.position, newCameraPos, camAutoSpeed * Time.deltaTime);
        }
    }

    public void ChangeState(GameState newState){
        gameState = newState;
        Debug.Log(newState);
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
        if (MenuManager.instance.InPauseMenu()){
            return;
        }
        newCameraPos = (Vector3)v + new Vector3(0, 0, -10);
    }
    public void PanCameraInDirection(Vector2 v){
        if (MenuManager.instance.InPauseMenu()){
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
        mainCamera.orthographicSize += amount;
        if (mainCamera.orthographicSize <= 2 || mainCamera.orthographicSize >= 12){
            mainCamera.orthographicSize -= amount;
        }
    }
}

public enum GameState{
    MainMenu,
    GenerateGrid,
    SapwnHeroes,
    SpawnEnemies,
    HeroesTurn,
    EnemiesTurn
}
