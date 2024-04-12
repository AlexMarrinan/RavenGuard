using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        var units = new List<BaseUnit>();
        unitsAwaitingOrders.ForEach(u => units.Add(u));
        foreach (BaseUnit unit in units){
            unit.moveAmount = 0;
            unit.UsePassiveSkills(PassiveSkillType.OnMovement);
            unit.FinishTurn();
        }
        // if (currentFaction == UnitFaction.Hero){
        //     GameManager.instance.ChangeState(GameState.EnemiesTurn);
        // }else{
        //     GameManager.instance.ChangeState(GameState.HeroesTurn);
        // }
    }
    public void BeginHeroTurn(){
        turnNumber++;
        currentFaction = UnitFaction.Hero;
        MenuManager.instance.ShowStartText("Your turn!", false);
        
        UnitManager.instance.ResetUnitMovment();
        unitsAwaitingOrders = UnitManager.instance.GetAllHeroes();
        if (unitsAwaitingOrders.Count <= 0){
            OnGameOver();
            return;
        }
        BaseUnit firstHero = unitsAwaitingOrders[0];
        MenuManager.instance.highlightObject.SetActive(true);
        GridManager.instance.SetHoveredTile(firstHero.occupiedTile);
        UnitManager.instance.DescrementBuffs(UnitFaction.Hero);
        UnitManager.instance.OnTurnStartSkills(UnitFaction.Hero);
    }
    public void OnGameOver(){
        MenuManager.instance.ShowStartText("GAME OVER", true);
        SceneManager.LoadScene("Town");
        return;
    }

    public void BeginEnemyTurn(){
        if (currentFaction == UnitFaction.Enemy){
            return;
        }
        currentFaction = UnitFaction.Enemy;
        // if (UnitManager.instance.GetAllEnemies().Count <= 0){
        //     MenuManager.instance.ToggleLevelEndMenu();
        //     return;
        // }
        MenuManager.instance.ShowStartText("Enemy's turn!", false);
        UnitManager.instance.ResetUnitMovment();
        MenuManager.instance.highlightObject.SetActive(false);
        UnitManager.instance.OnTurnStartSkills(UnitFaction.Enemy);
        UnitManager.instance.DescrementBuffs(UnitFaction.Enemy);
        StartCoroutine(MoveEnemies(UnitManager.instance.GetAllEnemies()));
    }
    IEnumerator MoveEnemies(List<BaseUnit> list){
        BaseUnit unit = list[0];
        if (unit != null){
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
//                    Debug.Log(move);
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
                    yield return currentMove.moveTile.MoveUnitAlongPath(unit);
                    yield return new WaitForSeconds(0.05f);
                    if (currentMove is AIAttack){
                        unit.Attack((currentMove as AIAttack).attackTile.occupiedUnit);
                        while (MenuManager.instance.menuState == MenuState.Battle){
                            yield return null;
                        }                
                    }
                    unit.hasMoved = true;
                }else{
                    Debug.Log("not moving!");
                }
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
        List<AIAttack> possibleAttacks = new();
        foreach (BaseTile move in moves){
            foreach ((BaseTile, TileMoveType) atk in unit.GetValidAttacks(move)){
                BaseTile adjTile = atk.Item1;
                TileMoveType adjType = atk.Item2;
                if (adjType != TileMoveType.Attack){
                    continue;
                }
                var attack = new AIAttack(move, adjTile);
                possibleAttacks.Add(attack);
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
            atk.rating += (otherUnit.health - otherUnitHP)*4/2;
        }
        //rating based on players remaining HP;
        int playerHealthPoints = 40 - otherUnitHP;
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
                atk.rating -= (unit.health - prediction.atkHealth)*4/3;
            }
        }else if (prediction.defenderCounterAttack && prediction.defender == unit){
            if (prediction.atkHealth <= 0){
                atk.rating += lethalBonus;
            }else{
                atk.rating += (unit.health - prediction.atkHealth)*1/2;
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
    
    public void GoToNextUnit(){
        GoToUnit(+1);
    }
    public void GoToPreviousUnit(){
        GoToUnit(-1);
    }
    private void GoToUnit(int offset){
        if (UnitManager.instance.GetAllEnemies().Count <= 0){
            return;
        }
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
            OnStageClear();
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
    public void OnStageClear(){
        MenuManager.instance.CloseMenus();
        MenuManager.instance.ToggleLevelEndMenu();
        SaveManager.instance.AddCopperCoins(100);
    }
    public void SetPreviousUnit(BaseUnit u){
        previousUnit = u;
    }
}

internal class AIMove {
    public int rating;
    public BaseTile moveTile;
}

//ANY AI MOVE THAT IS ATTACKING THE PLAYER UNITS
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


//ANY AI MOVE THAT IS NOT AN ATTACK
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