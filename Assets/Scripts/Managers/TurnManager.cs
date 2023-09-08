using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager instance;
    public UnitFaction currentFaction;
    public List<BaseUnit> unitsAwaitingOrders;
    void Awake(){
        instance = this;
    }

    public void BeginHeroTurn(){
        currentFaction = UnitFaction.Hero;
        Debug.Log("hero turn!");
        UnitManager.instance.ResetUnitMovment();
        unitsAwaitingOrders = UnitManager.instance.GetAllHeroes();
        BaseUnit firstHero = unitsAwaitingOrders[0];
        GridManager.instance.SetHoveredTile(firstHero.occupiedTile);
    }

    public void BeginEnemyTurn(){
        currentFaction = UnitFaction.Enemy;
        Debug.Log("enemies turn!");
    }

    public void GetNextHero(BaseUnit previous){
        unitsAwaitingOrders.Remove(previous);
        previous.OnExhaustMovment();

        //if no units left to move, go onto the enemies turn
        if (unitsAwaitingOrders.Count == 0){
            GameManager.instance.ChangeState(GameState.EnemiesTurn);
            return;
        }
        GridManager.instance.SetHoveredTile(unitsAwaitingOrders[0].occupiedTile);
        
        //TODO: figure out why SetHoverTile does not update visually without another input !!!
        //ALSO GET RID OF THIS HACK GARBAGE:
        // GridManager.instance.MoveHoveredTile(new Vector2(0, 1));
        // GridManager.instance.MoveHoveredTile(new Vector2(0, -1));

        // GridManager.instance.MoveHoveredTile(new Vector2(0, -1));
        // GridManager.instance.MoveHoveredTile(new Vector2(0, 1));

        // GridManager.instance.MoveHoveredTile(new Vector2(1, 0));
        // GridManager.instance.MoveHoveredTile(new Vector2(-1, 0));

        // GridManager.instance.MoveHoveredTile(new Vector2(-1, 0));
        // GridManager.instance.MoveHoveredTile(new Vector2(1, 0));
        //^^ THIS FUCKING SUCKS ^^

        GridManager.instance.SelectHoveredTile();
    }
}
