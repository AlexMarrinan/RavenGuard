using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameState gameState;
    public Canvas mainCanvas;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        gameState = GameState.GenerateGrid;
        ChangeState(gameState);
    }

    // Update is called once per frame
    void Update()
    {
        //TODO: why this was here in the first place !!!
        //ChangeState(gameState);
    }

    public void ChangeState(GameState newState){
        gameState = newState;
        switch(newState){
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
}

public enum GameState{
    GenerateGrid,
    SapwnHeroes,
    SpawnEnemies,
    HeroesTurn,
    EnemiesTurn
}
