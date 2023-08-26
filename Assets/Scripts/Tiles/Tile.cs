using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class Tile : MonoBehaviour
{
    [SerializeField] protected SpriteRenderer renderer;
    [SerializeField] private GameObject validMoveHighlight;
    [SerializeField] private bool isWalkable;
    
    public BaseUnit occupiedUnit;
    public bool walkable => (occupiedUnit == null && isWalkable) || (occupiedUnit != null && occupiedUnit.faction == UnitFaction.Enemy);
    public bool isValidMove = false;
    public Vector2 coordiantes;
    public virtual void Init(int x, int y){

    }
    private void OnMouseEnter() {
        OnHover();
        GridManager.instance.SelectHoveredTile(this);
    }
    public void OnHover(){
        if (UnitManager.instance.selectedUnit != null && !isValidMove){
            return;
        }
        MenuManager.instance.EnableHighlight(this);
    }
    private void OnMouseExit() {
        MenuManager.instance.DisableHighlight();
    }
    public bool isTerrainWalkable(){
        return isWalkable;
    }
    private void OnMouseDown(){
        OnSelectTile();
    }
    public void OnSelectTile(){
        if (GameManager.instance.gameState != GameState.HeroesTurn){
            return;
        }
        if (!isTerrainWalkable()){
            return;
        }
        if (UnitManager.instance.selectedUnit != null && !isValidMove){
            return;
        }

        //current pressed tile is occupied
        if (occupiedUnit != null){
            //current unit is a hero, set as selected
            if (occupiedUnit.faction == UnitFaction.Hero){
                UnitManager.instance.SetSeclectedUnit(occupiedUnit);
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
    }
    public void MoveToSelectedTile(){
        SetUnit(UnitManager.instance.selectedUnit);
        UnitManager.instance.SetSeclectedUnit(null);
    }
    public void SetUnit(BaseUnit unit){
        if (unit.occupiedTile != null){
            unit.occupiedTile.occupiedUnit = null;
        }
        unit.transform.position = this.transform.position;
        this.occupiedUnit = unit;
        unit.occupiedTile = this;
    }

    public void SetPossibleMove(bool valid){
        validMoveHighlight.SetActive(valid);
        isValidMove = valid;
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
}
