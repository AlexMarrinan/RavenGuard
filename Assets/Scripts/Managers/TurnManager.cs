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
        if (UnitManager.instance.GetAllEnemies().Count <= 0){
            MenuManager.instance.ShowStartText("YOU WIN!");
        }
        MenuManager.instance.ShowStartText("Enemy's turn!");
        UnitManager.instance.ResetUnitMovment();
        StartCoroutine(MoveEnemy(UnitManager.instance.GetAllEnemies(), 0));
    }
    IEnumerator MoveEnemy(List<BaseUnit> list, int attemptNumber){
        var enemy = list[0];
        int maxMove = enemy.maxMoveAmount;
        GameManager.instance.PanCamera(enemy.transform.position);
        if (attemptNumber == 0){
            yield return new WaitForSeconds(0.8f);
        }
        int chosenMoveAmount = Random.Range(1, maxMove);
        int dir = Random.Range(0,4);
        Vector2 direction;
        switch(dir){
            case 0:
                direction = new Vector2(1, 0);
                break;
            case 1:
                direction = new Vector2(-1, 0);
                break;
            case 2:
                direction = new Vector2(0, 1);
                break;
            case 3:
                direction = new Vector2(0, -1);
                break;
            default:
                direction = new Vector2(0, 0);
                break;
        }
        Vector2 curr = enemy.occupiedTile.coordiantes;
        Tile currTile = enemy.occupiedTile;
        for (int i = 0; i < chosenMoveAmount; i++){
            curr += direction;
            Tile nextTile = GridManager.instance.GetTileAtPosition(curr);
            if (nextTile == null || !nextTile.walkable || nextTile.occupiedUnit != null){
                break;
            }
            currTile = nextTile;
        }
        if (currTile == enemy.occupiedTile || attemptNumber == 3){
            yield return MoveEnemy(list, attemptNumber + 1);
        }else{
            currTile.SetUnit(enemy);
            yield return new WaitForSeconds(0.35f);
            list.RemoveAt(0);
            if (list.Count > 0){
                Debug.Log(list.Count);
                yield return MoveEnemy(list, 0);
            }else{
                GameManager.instance.ChangeState(GameState.HeroesTurn);
            }
            yield return null;
        }
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
        if (previous.faction == UnitFaction.Enemy){
            return;
        }
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
