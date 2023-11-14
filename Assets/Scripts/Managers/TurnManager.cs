using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager instance;
    public UnitFaction currentFaction;
    public List<BaseUnit> unitsAwaitingOrders;
    private BaseUnit previousUnit;
    public int lethalBonus = 50;
    public int turnNumber = 0;
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
        turnNumber++;
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
        UnitManager.instance.DescrementBuffs(UnitFaction.Hero);
        UnitManager.instance.OnTurnStartSkills(UnitFaction.Hero);
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
        UnitManager.instance.OnTurnStartSkills(UnitFaction.Enemy);
        UnitManager.instance.DescrementBuffs(UnitFaction.Enemy);
        StartCoroutine(MoveEnemies(UnitManager.instance.GetAllEnemies()));
    }
    IEnumerator MoveEnemies(List<BaseUnit> list){
        BaseUnit unit = list[0];
        MenuManager.instance.unitStatsMenu.gameObject.SetActive(true);
        MenuManager.instance.unitStatsMenu.SetUnit(unit);
        GameManager.instance.PanCamera(unit.transform.position);
        List<BaseTile> validMoves = UnitManager.instance.SetValidMoves(unit);
        yield return new WaitForSeconds(0.35f);
        if (unit.IsInjured()){
            MoveInjuredEnemy(unit);
        } else if (unit.IsAggroed() || unit.OpponentInRange()){
            List<AIMove> moves = new();
            if (unit.OpponentInRange()){
                 moves = moves.Concat(RateAttacks(unit)).ToList();
            }
            if (unit.AlliesInRange()){
                moves = moves.Concat(RateSupports(unit)).ToList();
            }
            moves = moves.Concat(RateStandardMoves(unit)).ToList();
            List<AIMove> currentMoves = null;
            //Debug.Log(moves.Count);
            foreach (AIMove move in moves){
//                Debug.Log("Rating: " + move.rating + " Tile: " + move.moveTile.coordiantes);
                if (currentMoves == null){
                    currentMoves = new(){move};
                }
                if (move.rating == currentMoves[0].rating){
                    currentMoves.Add(move);
                }
                if (move.rating > currentMoves[0].rating){
                    currentMoves.Clear();
                    currentMoves.Add(move);
                }
            }
            if (currentMoves != null){
                int moveIdx = Random.Range(0, currentMoves.Count);
                var currentMove = currentMoves[moveIdx];
                // Debug.Log(currentMove.moveTile);
                // Debug.Log(currentMove.rating);
                PathLine.instance.RenderLine(unit.occupiedTile, currentMove.moveTile);
                yield return new WaitForSeconds(0.5f);
                currentMove.moveTile.MoveUnitToTile(unit);
                yield return new WaitForSeconds(0.5f);
                if (currentMove is AIAttack){
                    unit.Attack((currentMove as AIAttack).attackTile.occupiedUnit);
                    while (MenuManager.instance.menuState == MenuState.Battle){
                        yield return null;
                    }                
                }
            }else{
                Debug.Log("not moving!");
            }
        }
        list.RemoveAt(0);
        yield return new WaitForSeconds(0.25f);
        UnitManager.instance.RemoveAllValidMoves();
        if (list.Count > 0){
            yield return MoveEnemies(list);
        }else{
            GameManager.instance.ChangeState(GameState.HeroesTurn);
        }
        yield return null;
    }

    private void MoveInjuredEnemy(BaseUnit unit) {
        Debug.Log("Moving injured enemy!!!!");
    }

    private List<AIAttack> RateAttacks(BaseUnit unit){
        List<BaseTile> moves = UnitManager.instance.GetValidMoves(unit);
        List<BaseUnit> unitsInRandge = new();
        foreach (BaseUnit opp in UnitManager.instance.GetAllHeroes()){
            if (moves.Contains(opp.occupiedTile)){
                unitsInRandge.Add(opp);
            }
        }

        List<AIAttack> possibleAttacks = new();
        foreach (BaseUnit opp in unitsInRandge){
            foreach (BaseTile adjTile in opp.occupiedTile.GetAdjacentTiles()){
                if (/*adjTile.moveType == TileMoveType.Move &&*/ moves.Contains(adjTile)){
                    possibleAttacks.Add(new(adjTile, opp.occupiedTile));
                }
            }
        }
        foreach (AIAttack atk in possibleAttacks){
            RateAttack(unit, atk);
        }
        //Debug.Log("Possible attacks count: " + possibleAttacks.Count);
        return possibleAttacks;
    }

    private void RateAttack(BaseUnit unit, AIAttack atk){
        if (atk.activeSkill != null){
            RateActiveAttack(atk);
        }
        BattlePrediction prediction = new(unit, atk.attackTile.occupiedUnit);
        BaseUnit otherUnit = prediction.defender;
        int otherUnitHP = prediction.defHealth;
        int unitHP = prediction.atkHealth;
        if (prediction.defender == unit){
            otherUnit = prediction.attacker;
            otherUnitHP = prediction.atkHealth;
            unitHP = prediction.defHealth;
        }
        //if damage will be lethal
        if (prediction.defHealth <= 0){
            atk.rating += lethalBonus;
        }else{
            atk.rating += (otherUnit.health - otherUnitHP)/2;
        }
        //rating based on players remaining HP;
        int playerHealthPoints = 20 - otherUnitHP;
        if (playerHealthPoints < 0){
            playerHealthPoints = 0;
        }
        atk.rating += playerHealthPoints;
    
        //rating from turn count
        atk.rating += turnNumber;

        //TODO: add points based on alies in range;

        //if the player unit can counterattack...
        if (prediction.defenderCounterAttack && prediction.defender == otherUnit){
            if (prediction.atkHealth <= 0){
                atk.rating -= lethalBonus;
            }else{
                atk.rating -= (unit.health - prediction.atkHealth)/2;
            }
        }else if (prediction.defenderCounterAttack && prediction.defender == unit){
            if (prediction.atkHealth <= 0){
                atk.rating += lethalBonus;
            }else{
                atk.rating += (unit.health - prediction.atkHealth)/2;
            }
        }

        //rating based on AI's remaining HP;
        atk.rating -= (unit.health - unitHP)/2;
        
        //remove points based on number of enemeies targeting
        foreach (BaseUnit hero in UnitManager.instance.GetAllHeroes()){
            if (hero.UnitInRange(unit)){
                atk.rating -= hero.GetAttack().total - unit.GetDefense().total;
            }
        }
    }

    private void RateActiveAttack(AIAttack atk){
        atk.rating += 0;
    }
    private List<AISupport> RateSupports(BaseUnit unit){
        List<BaseTile> moves = UnitManager.instance.GetValidMoves(unit);
        List<AISupport> possibleSupports = new();
        foreach (BaseTile tile in moves){
            possibleSupports.Add(new(tile, tile));
        }
        foreach (AISupport sup in possibleSupports){
            RateSupport(unit, sup);
        }
        return possibleSupports;
    }

    private void RateSupport(BaseUnit unit, AISupport sup){
        // foreach (Tile t in GridManager.instance.GetRadiusTiles(sup.moveTile, unit.maxMoveAmount)){
        //     if (t.occupiedUnit != null){
        //         if (t.occupiedUnit.faction == UnitFaction.Hero){
        //             sup.rating -= 20;
        //         }else{
        //             sup.rating += 20;
        //         }
        //     }
        // }
        sup.rating = -100;
    }

    private List<AISupport> RateStandardMoves(BaseUnit unit){
        List<BaseTile> moves = UnitManager.instance.GetValidMoves(unit);
        List<AISupport> possibleMoves = new();
        foreach (BaseTile tile in moves){
            possibleMoves.Add(new(tile, tile));
        }
        foreach (AIMove move in possibleMoves){
            RateMove(unit, move);
        }
        return possibleMoves;
    }

    private void RateMove(BaseUnit unit, AIMove move){
        //moving points based on distance
        move.rating += GridManager.instance.Distance(unit.occupiedTile, move.moveTile)*2;

        //TODO: ADD DIFFERENT TILE TYPES TO USE DIFF POINTS !!!

        var rangeTiles = UnitManager.instance.GetPotentialValidMoves(unit, move.moveTile);

        //add points based on nearby allies
        var allies = UnitManager.instance.GetAllEnemies();
        foreach (BaseUnit ally in allies){
            if (rangeTiles.Contains(ally.occupiedTile)){
                int distance = GridManager.instance.Distance(ally.occupiedTile, move.moveTile);
                int ratingChnage = 6 - (2*(distance-1));
                if (ratingChnage < 0){
                    ratingChnage = 0;
                }
                move.rating += ratingChnage;
            }
        }

        //add points based on nearby oponents
        var opps = UnitManager.instance.GetAllEnemies();
        foreach (BaseUnit opp in opps){
            if (rangeTiles.Contains(opp.occupiedTile)){
                int distance = GridManager.instance.Distance(opp.occupiedTile, move.moveTile);
                int ratingChnage = 6 - (2*(distance-1));
                if (ratingChnage < 0){
                    ratingChnage = 0;
                }

                //remove points based on distance of enemy
                move.rating -= ratingChnage;

                //remove points based on attack power of enemy
                move.rating -= opp.GetAttack().total*2;
            }
        }

        //TODO: 
        //compare the closest player before the move to the closest player after the move

    }

    IEnumerator MoveEnemiesOLD(List<BaseUnit> list){
        BaseUnit enemy = list[0];
        MenuManager.instance.unitStatsMenu.gameObject.SetActive(true);
        MenuManager.instance.unitStatsMenu.SetUnit(enemy);
        GameManager.instance.PanCamera(enemy.transform.position);
        List<BaseTile> validMoves = UnitManager.instance.SetValidMoves(enemy);
        yield return new WaitForSeconds(0.8f);
        //yield return MoveWithAnimation(unit);
        BaseUnit heroToAttack = null;
        BaseTile final = null;
        foreach (BaseTile t in validMoves){
            if (t.occupiedUnit != null && t.occupiedUnit.faction == UnitFaction.Hero){
                var adjTiles = GridManager.instance.GetAdjacentTiles(t.coordiantes);
                if (adjTiles.Contains(enemy.occupiedTile)){
                    final = enemy.occupiedTile;
                    heroToAttack = t.occupiedUnit;
                    break;
                }
                foreach (BaseTile t2 in adjTiles){
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
            PathLine.instance.RenderLine(enemy.occupiedTile, final);
            yield return new WaitForSeconds(0.1f);
            final.MoveUnitToTile(enemy);

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

internal class AIMove {
    public int rating;
    public BaseTile moveTile;
}
internal class AIAttack : AIMove{
    public BaseTile attackTile;
    public ActiveSkill activeSkill;

    public AIAttack (BaseTile moveTile, BaseTile attackTile){
        rating = 0;
        this.moveTile = moveTile;
        this.attackTile = attackTile;
        this.activeSkill = null;
    }
    public AIAttack (BaseTile moveTile, BaseTile attackTile, ActiveSkill activeSkill){
        rating = 0;
        this.moveTile = moveTile;
        this.attackTile = attackTile;
        this.activeSkill = activeSkill;
    }
}

internal class AISupport : AIMove{
    public BaseTile supporTile;
    public ActiveSkill activeSkill;

    public AISupport (BaseTile moveTile, BaseTile attackTile){
        rating = 0;
        this.moveTile = moveTile;
        this.supporTile = attackTile;
        this.activeSkill = null;
    }
    public AISupport (BaseTile moveTile, BaseTile attackTile, ActiveSkill activeSkill){
        rating = 0;
        this.moveTile = moveTile;
        this.supporTile = attackTile;
        this.activeSkill = activeSkill;
    }
}