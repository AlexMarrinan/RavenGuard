using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager instance;
    public UnitFaction currentFaction;
    public List<BaseUnit> unitsAwaitingOrders;
    private BaseUnit previousUnit;
    void Awake(){
        instance = this;
    }

    public void BeginHeroTurn(){
        currentFaction = UnitFaction.Hero;
        MenuManager.instance.ShowStartText("Your turn!");
        UnitManager.instance.ResetUnitMovment();
        unitsAwaitingOrders = UnitManager.instance.GetAllHeroes();
        BaseUnit firstHero = unitsAwaitingOrders[0];
        GridManager.instance.SetHoveredTile(firstHero.occupiedTile);
    }

    public void BeginEnemyTurn(){
        currentFaction = UnitFaction.Enemy;
        MenuManager.instance.ShowStartText("Enemie's turn!");
        UnitManager.instance.ResetUnitMovment();
    }
    public void GoToNextUnit(){
        GoToUnit(+1);
    }
    public void GoToPreviousUnit(){
        GoToUnit(-1);
    }
    private void GoToUnit(int offset){
        if (GameManager.instance.gameState != GameState.HeroesTurn){
            return;
        }
        var index = unitsAwaitingOrders.IndexOf(previousUnit);
        index += offset;
        if (index < 0){
            index = unitsAwaitingOrders.Count - 1;
        }else if (index >= unitsAwaitingOrders.Count){
            index = 0;
        }
        GridManager.instance.SetHoveredTile(unitsAwaitingOrders[index].occupiedTile);
    }
    public void OnUnitDone(BaseUnit previous){
        unitsAwaitingOrders.Remove(previous);

        //if no units left to move, go onto the enemies turn
        if (unitsAwaitingOrders.Count == 0){
            GameManager.instance.ChangeState(GameState.EnemiesTurn);
            return;
        }
        previousUnit = unitsAwaitingOrders[0];
        GridManager.instance.SetHoveredTile(unitsAwaitingOrders[0].occupiedTile);
        //GridManager.instance.SelectHoveredTile();
    }

    public void SetPreviousUnit(BaseUnit u){
        previousUnit = u;
    }
}
