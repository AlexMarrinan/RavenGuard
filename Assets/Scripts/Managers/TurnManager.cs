using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    public void SkipTurn(){
        if (currentFaction == UnitFaction.Hero){
            GameManager.instance.ChangeState(GameState.EnemiesTurn);
        }else{
            GameManager.instance.ChangeState(GameState.HeroesTurn);
        }
    }
    public void BeginHeroTurn(){
        currentFaction = UnitFaction.Hero;
        MenuManager.instance.ShowStartText("Your turn!", false);
        UnitManager.instance.ResetUnitMovment();
        unitsAwaitingOrders = UnitManager.instance.GetAllHeroes();
        if (unitsAwaitingOrders.Count <= 0){
            MenuManager.instance.ShowStartText("GAME OVER", true);
            return;
        }
        BaseUnit firstHero = unitsAwaitingOrders[0];
        MenuManager.instance.highlightObject.SetActive(true);
        GridManager.instance.SetHoveredTile(firstHero.occupiedTile);
    }

    public void BeginEnemyTurn(){
        currentFaction = UnitFaction.Enemy;
        if (UnitManager.instance.GetAllEnemies().Count <= 0){
            MenuManager.instance.ShowStartText("YOU WIN!", true);
            return;
        }
        MenuManager.instance.ShowStartText("Enemy's turn!", false);
        UnitManager.instance.ResetUnitMovment();
        MenuManager.instance.highlightObject.SetActive(false);
        StartCoroutine(MoveEnemies(UnitManager.instance.GetAllEnemies()));
    }
    IEnumerator MoveEnemies(List<BaseUnit> list){
        BaseUnit enemy = list[0];
        MenuManager.instance.unitStatsMenu.gameObject.SetActive(true);
        MenuManager.instance.unitStatsMenu.SetUnit(enemy);
        GameManager.instance.PanCamera(enemy.transform.position);
        List<Tile> validMoves = UnitManager.instance.SetValidMoves(enemy);
        yield return new WaitForSeconds(0.8f);
        //yield return MoveWithAnimation(unit);
        BaseUnit heroToAttack = null;
        Tile final = null;
        foreach (Tile t in validMoves){
            if (t.occupiedUnit != null && t.occupiedUnit.faction == UnitFaction.Hero){
                var adjTiles = GridManager.instance.GetAdjacentTiles(t.coordiantes);
                if (adjTiles.Contains(enemy.occupiedTile)){
                    final = enemy.occupiedTile;
                    heroToAttack = t.occupiedUnit;
                    break;
                }
                foreach (Tile t2 in adjTiles){
                    if (validMoves.Contains(t2) && t2.occupiedUnit == null){
                        final = t2;
                        heroToAttack = t.occupiedUnit;
                        break;
                    }
                }
                if (final != null){
                    break;
                }
            }
        }
        if (final == null){
            int tileIndex = Random.Range(0, validMoves.Count);
            final = validMoves[tileIndex];
        }
        if (final != enemy.occupiedTile){
            final.SetUnit(enemy);
        }else{
            UnitManager.instance.RemoveAllValidMoves();
        }
        yield return new WaitForSeconds(0.35f);
        list.RemoveAt(0);
        if (heroToAttack != null){
            enemy.Attack(heroToAttack);
            while (MenuManager.instance.menuState == MenuState.Battle){
                yield return null;
            }
            yield return new WaitForSeconds(0.35f);
        }
        if (list.Count > 0){
            yield return MoveEnemies(list);
        }else{
            GameManager.instance.ChangeState(GameState.HeroesTurn);
        }
        yield return null;
    }


    IEnumerator MoveEnemyOld(List<BaseUnit> list, int attemptNumber){
        var enemy = list[0];
        int maxMove = enemy.maxMoveAmount;
        GameManager.instance.PanCamera(enemy.transform.position);
        if (attemptNumber == 0){
            yield return new WaitForSeconds(0.8f);
        }
        int chosenMoveAmount = Random.Range(1, maxMove);
        int dir = Random.Range(0,4);

        var direction = dir switch
        {
            0 => new Vector2(1, 0),
            1 => new Vector2(-1, 0),
            2 => new Vector2(0, 1),
            3 => new Vector2(0, -1),
            _ => new Vector2(0, 0),
        };

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
            yield return MoveEnemyOld(list, attemptNumber + 1);
        }else{
            currTile.SetUnit(enemy);
            yield return new WaitForSeconds(0.35f);
            list.RemoveAt(0);
            if (list.Count > 0){
                Debug.Log(list.Count);
                yield return MoveEnemyOld(list, 0);
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
        MenuManager.instance.menuState = MenuState.None;
        if (previous.faction == UnitFaction.Enemy){
            return;
        }
        unitsAwaitingOrders.Remove(previous);
        
        //if no units left to move, go onto the enemies turn
        if (unitsAwaitingOrders.Count == 0){
            UnitManager.instance.OnTurnEndSkills(previous);
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
