using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;
using System;
using UnityEngine.UI;
using TMPro;

public abstract class Tile : MonoBehaviour
{
    [SerializeField] protected SpriteRenderer renderer; 
    [SerializeField] private GameObject validMoveHighlight;

    //Can a unit walk onto this tile
    protected bool isWalkable;
    //Can a unit shoot past this tile
    protected bool isShootable;
    public BaseUnit occupiedUnit;
    public bool walkable => (occupiedUnit == null && isWalkable) || (occupiedUnit != null && occupiedUnit.faction == OtherFaction());
    public TileMoveType moveType = TileMoveType.NotValid;
    public int depth = 0;
    public List<Tile> validPath = null;
    public TMP_Text depthText;
    public Vector2 coordiantes;
    private void FixedUpdate(){
        depthText.text = moveType.ToString();
    }
    public virtual void Init(int x, int y){

    }

    private UnitFaction OtherFaction()
    {
        if (this.occupiedUnit.faction == UnitFaction.Hero){
            return UnitFaction.Enemy;
        }
        return UnitFaction.Hero;
    }
    private void OnMouseEnter() {
        if (!InputManager.instance.enableMouse){
            return;
        }
        GameManager.instance.SetUsingMouse(true);
        OnHover();
        GridManager.instance.SetHoveredTile(this);
    }
    public void OnHover(){
        //if a unit is selected
        if (UnitManager.instance.selectedUnit != null){
            if (moveType == TileMoveType.NotValid && UnitManager.instance.selectedUnit.occupiedTile != this) {
                return;
            }
            ToggleLinePoint();
        }
        if (occupiedUnit != null && occupiedUnit.faction == UnitFaction.Hero && TurnManager.instance.unitsAwaitingOrders.Contains(occupiedUnit)){
            TurnManager.instance.SetPreviousUnit(occupiedUnit);
        }
        MenuManager.instance.HighlightTile(this);
    }
    private void OnMouseExit() {
        if (!InputManager.instance.enableMouse){
            return;
        }
        MenuManager.instance.UnhighlightTile();
    }
    public bool IsTileSelectable(){
        return true;//isWalkable || isShootable;
    }
    private void OnMouseDown(){
        if (!InputManager.instance.enableMouse){
            return;
        }
        OnSelectTile();
    }
    public void OnSelectTile(){
        if (GameManager.instance.gameState != GameState.HeroesTurn){
            return;
        }
        if (!IsTileSelectable()){
            return;
        }
        if (UnitManager.instance.selectedUnit != null && (moveType == TileMoveType.NotValid || moveType == TileMoveType.InAttackRange)){
            return;
        }

        //current pressed tile is occupied
        if (occupiedUnit != null){
            //if the unit is out of movment, do not allow selection
            if (!TurnManager.instance.unitsAwaitingOrders.Contains(occupiedUnit) && occupiedUnit.faction == TurnManager.instance.currentFaction){
                return;
            }
            //current unit is a hero, set as selected
            if (occupiedUnit.faction == UnitFaction.Hero){
                UnitManager.instance.SetSeclectedUnit(occupiedUnit);
                return;
            }
            //current unit is enemy, AND selected is enemy,
            else if (occupiedUnit.faction == UnitFaction.Enemy){
                if (UnitManager.instance.selectedUnit == null){
                  return;
                }
                if (UnitManager.instance.selectedUnit.faction == UnitFaction.Hero){
                    //move hero to enemy, kill enemy
                    UnitManager.instance.selectedUnit.Attack(occupiedUnit);
                }
            }
        
        //current pressed tile is NOT occupied
        }else{
            if (UnitManager.instance.selectedUnit != null){
                MoveToSelectedTile();
            }
        }
        PathLine.instance.Reset();
    }
    public void MoveToSelectedTile(){
        BaseUnit oldSelectedUnit = UnitManager.instance.selectedUnit;
        UnitManager.instance.SetSeclectedUnit(null);
        SetUnit(oldSelectedUnit);
    }
    public void SetUnitStart(BaseUnit unit){
        if (unit.occupiedTile != null){
            unit.occupiedTile.occupiedUnit = null;
        }
        unit.transform.position = this.transform.position;
        this.occupiedUnit = unit;
        unit.occupiedTile = this;
        unit.moveAmount -= depth;
    }
    public void SetUnit(BaseUnit unit){
        SetUnit(unit, true);
    }
    public void SetUnit(BaseUnit unit, bool turnOver){
        if (unit.occupiedTile != null){
            unit.occupiedTile.occupiedUnit = null;
        }
        unit.moveAmount = GridManager.instance.Distance(this, unit.occupiedTile);
        unit.transform.position = this.transform.position;
        this.occupiedUnit = unit;
        unit.occupiedTile = this;
        if (turnOver){
            occupiedUnit.OnExhaustMovment();
        }
    }
    public void SetPossibleMove(bool valid, Tile startPos){
        validMoveHighlight.SetActive(valid);
        if (valid){
            //determine if attack or move
            moveType = startPos.occupiedUnit.GetMoveTypeAt(this);
        
            //set color if attack or move
            var highlightSprite = validMoveHighlight.GetComponent<SpriteRenderer>();
            if (moveType == TileMoveType.Attack){
                highlightSprite.color = MenuManager.instance.attackColor;
            }else if (moveType == TileMoveType.Move){
                highlightSprite.color = MenuManager.instance.moveColor;
            } else if (moveType == TileMoveType.InAttackRange){
                highlightSprite.color = MenuManager.instance.inRangeColor;
            } else if (moveType == TileMoveType.Support){
                highlightSprite.color = MenuManager.instance.supportColor;
            }
        }else{
            moveType = TileMoveType.NotValid;
        }
        if (startPos == null){
            validPath = null;
        }else{
            validPath = GetPathFrom(startPos);
        }
    }
    public List<Tile> GetPathFrom(Tile startPos){
        List<Tile> path = new List<Tile>();
        var startCoordiantes = startPos.coordiantes;
        int x = (int)Mathf.Abs(startCoordiantes.x - coordiantes.x);
        int y = (int)Mathf.Abs(startCoordiantes.y - coordiantes.y);


        //TODO: THIS MAY TAKE PATHS THROUGH WALLS MAKE BETTER !!!!!
        depth = 111;
        validPath = GridManager.instance.ShortestPathBetweenTiles(startPos, this);
        if (validPath != null){
            depth = validPath.Count;
        }
        //Debug.Log(depth);
        //TOOD: actually make the path for drawing the line
        return path;
    }
     public int GetPathLengthFrom(Tile startPos){
        List<Tile> path = new List<Tile>();
        var startCoordiantes = startPos.coordiantes;
        int x = (int)Mathf.Abs(startCoordiantes.x - coordiantes.x);
        int y = (int)Mathf.Abs(startCoordiantes.y - coordiantes.y);

        //TODO: THIS MAY TAKE PATHS THROUGH WALLS MAKE BETTER !!!!!
        depth = x + y;
        
        //TOOD: actually make the path for drawing the line
        return depth;
    }
    public List<Tile> GetAdjacentTiles(){
        int left = (int)coordiantes.x - 1;
        int right = (int)coordiantes.x + 1;
        int up = (int)coordiantes.y - 1;
        int down = (int)coordiantes.y + 1;

        Vector2 l = new Vector2(left, coordiantes.y);
        Vector2 r = new Vector2(right, coordiantes.y);
        Vector2 u = new Vector2(coordiantes.x, up);
        Vector2 d = new Vector2(coordiantes.x, down);
        
        Vector2[] array = {l, r, u, d};
        List<Tile> newTiles = new List<Tile>();
        for (int i = 0; i < 4; i++){
            newTiles.Add(GridManager.instance.GetTileAtPosition(array[i]));
        }
        return newTiles;

    }

    private void ToggleLinePoint(){
        PathLine.instance.RenderLine(UnitManager.instance.selectedUnit.occupiedTile, this);
    }
}


public enum TileMoveType {
    NotValid,
    Move,
    Attack,
    InAttackRange,
    Support
}