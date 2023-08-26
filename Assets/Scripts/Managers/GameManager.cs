using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameState gameState;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        gameState = GameState.GenerateGrid;
    }

    // Update is called once per frame
    void Update()
    {
        ChangeState(gameState);
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
                break;
            case GameState.EnemiesTurn:
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
