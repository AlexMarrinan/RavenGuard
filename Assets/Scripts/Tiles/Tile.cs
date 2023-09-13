using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;

public abstract class Tile : MonoBehaviour
{
    [SerializeField] protected SpriteRenderer renderer; 
    [SerializeField] private GameObject validMoveHighlight;

    //Can a unit walk onto this tile
    protected bool isWalkable;
    //Can a unit shoot past this tile
    protected bool isShootable;
    public BaseUnit occupiedUnit;
    public bool walkable => (occupiedUnit == null && isWalkable) || (occupiedUnit != null && occupiedUnit.faction == UnitFaction.Enemy);
    public TileMoveType moveType = TileMoveType.NotValid;
    public int depth = 0;
    public List<Tile> validPath = null;
    public Vector2 coordiantes;
    public virtual void Init(int x, int y){

    }
    private void OnMouseEnter() {
        GameManager.instance.SetUsingMouse(true);
        OnHover();
        GridManager.instance.SetHoveredTile(this);
    }
    public void OnHover(){
        //if a unit is selected
        if (UnitManager.instance.selectedUnit != null){
            if (moveType == TileMoveType.NotValid) {
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
        MenuManager.instance.UnhighlightTile();
    }
    public bool isTileSelectable(){
        return isWalkable || isShootable;
    }
    private void OnMouseDown(){
        OnSelectTile();
    }
    public void OnSelectTile(){
        if (GameManager.instance.gameState != GameState.HeroesTurn){
            return;
        }
        if (!isTileSelectable()){
            return;
        }
        if (UnitManager.instance.selectedUnit != null && (moveType == TileMoveType.NotValid || moveType == TileMoveType.InAttackRange)){
            return;
        }

        //current pressed tile is occupied
        if (occupiedUnit != null){
            //if the unit is out of movment, do not allow selection
            if (occupiedUnit.moveAmount <= 1){
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
    public void SetUnit(BaseUnit unit){
        if (unit.occupiedTile != null){
            unit.occupiedTile.occupiedUnit = null;
        }
        unit.moveAmount -= depth;
        unit.transform.position = this.transform.position;
        this.occupiedUnit = unit;
        unit.occupiedTile = this;
        //unit.healthBar.RenderHealth();

        if (unit.moveAmount <= 1){
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
        depth = x + y;
        
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
    public List<Tile> GetAdjacentCoords(){
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
        bool onPath = PathLine.instance.IsOnPath(this);
        if (!onPath){
            PathLine.instance.AddTile(this);
        }else{
            PathLine.instance.RemoveTile(this);
        }
    }
}


public enum TileMoveType {
    NotValid,
    Move,
    Attack,
    InAttackRange,
    Support
}