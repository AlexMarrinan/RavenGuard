using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;
using System;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Tilemaps;

public abstract class BaseTile : MonoBehaviour
{
    [SerializeField] protected SpriteRenderer bgSprite, middleSprite, fgSprite; 
    [SerializeField] private GameObject validMoveHighlight;

    //Can a unit walk onto this tile
    protected bool isWalkable;
    //Can a unit shoot past this tile
    protected bool isShootable;
    public BaseUnit occupiedUnit;
    public bool walkable => (occupiedUnit == null && isWalkable) || (occupiedUnit != null && occupiedUnit.faction == OtherFaction());
    public TileMoveType moveType = TileMoveType.NotValid;
    public int depth = 0;
    public List<BaseTile> validPath;
    public TMP_Text depthText;
    public Vector2 coordiantes;
    public TileEditorType editorType;
    public LevelChest attachedChest;
    public SpawnFaction spawnTeam;
    public bool spawnChest;

    private ParticleSystem[] moveParticles;

    private void FixedUpdate(){
        //depthText.text = moveType.ToString();
    }
    public virtual void Init(int x, int y){
        moveParticles = GetComponentsInChildren<ParticleSystem>();
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
                PathLine.instance.Reset();
            }else{
                RerenderLine();
            }
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
//        Debug.Log("Selecting tile...");
        if (GameManager.instance.gameState != GameState.HeroesTurn){
            return;
        }
//        Debug.Log("1");
        if (!IsTileSelectable()){
            return;
        }
//       Debug.Log("2");
//        Debug.Log(moveType);
        if (UnitManager.instance.selectedUnit != null && (moveType == TileMoveType.NotValid || moveType == TileMoveType.InAttackRange)){
            return;
        }
 //       Debug.Log("3");
        //current pressed tile is occupied
        if (occupiedUnit != null){
            //if the unit is not awaiting orders, do not allow selection
            if (!TurnManager.instance.unitsAwaitingOrders.Contains(occupiedUnit) && occupiedUnit.faction == TurnManager.instance.currentFaction){
                return;
            }
            //current unit is a hero, set as selected
            if (occupiedUnit.faction == UnitFaction.Hero){
                UnitManager.instance.SetSelectedUnit(occupiedUnit);
                return;
            }
            //current unit is enemy, AND selected is enemy,
            else if (occupiedUnit.faction == UnitFaction.Enemy){
 //               Debug.Log("Selected unit is enemy!");
                if (UnitManager.instance.selectedUnit == null){
                  return;
                }
                if (UnitManager.instance.selectedUnit.faction == UnitFaction.Hero){
                    //move hero to enemy, kill enemy
                    AudioManager.instance.PlayConfirm();
                    UnitManager.instance.selectedUnit.Attack(occupiedUnit);
                }
            }
        
        //current pressed tile is NOT occupied
        }else{
            if (UnitManager.instance.selectedUnit != null){
                AudioManager.instance.PlayConfirm();
                MoveToSelectedTile();
            }
        }
        PathLine.instance.Reset();
    }
    public void MoveToSelectedTile(){
        BaseUnit oldSelectedUnit = UnitManager.instance.selectedUnit;
        UnitManager.instance.SetSelectedUnit(null);
        StartCoroutine(MoveUnitAlongPath(oldSelectedUnit));
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
    /// <summary>
    /// Moves the specified unit along the current PathLine
    /// </summary>
    /// <param name="unit">unit to move</param>
    public IEnumerator MoveUnitAlongPath(BaseUnit unit){
        yield return MoveUnitAlongPath(unit, true);
    }
    /// <summary>
    /// Moves the specified unit along the current PathLine
    /// </summary>
    /// <param name="unit">unit to move</param>
    /// <param name="turnOver">should the units turn end after they move</param>
    public IEnumerator MoveUnitAlongPath(BaseUnit unit, bool turnOver){

        if (unit == this.occupiedUnit){
            yield return null;
        }
        else if (unit.occupiedTile != null){
            unit.occupiedTile.occupiedUnit = null;
        }
        unit.moveAmount = GridManager.instance.Distance(this, unit.occupiedTile);

        var path = PathLine.instance.GetPath();

        yield return UnitManager.instance.AnimateUnitMove(unit, path, turnOver);
    }

    public void SetPossibleMove(bool valid, BaseTile startPos){
        validMoveHighlight.SetActive(valid);
        if (valid){
            //determine if attack or move
            moveType = startPos.occupiedUnit.GetMoveTypeAt(this);
//            Debug.Log(moveType);
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
            }else if (moveType == TileMoveType.NotValid){
                validMoveHighlight.SetActive(false);
            }
        }else{
            moveType = TileMoveType.NotValid;
        }
        if (startPos == null){
            validPath = null;
        }else{
            if (!SkillManager.instance.selectingSkill){
                validPath = GetPathFrom(startPos);
            }
        }
    }
    public void SetSkillMove(bool valid, BaseTile startPos){
        validMoveHighlight.SetActive(valid);
        if (valid){
            //determine if attack or move
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
            }else if (moveType == TileMoveType.NotValid){
                validMoveHighlight.SetActive(false);
            }
        }else{
            moveType = TileMoveType.NotValid;
        }
        if (startPos == null){
            validPath = null;
        }else{
            if (!SkillManager.instance.selectingSkill){
                validPath = GetPathFrom(startPos);
            }
        }
    }
    public void SetPossibleAttack(BaseUnit attacker){
        validMoveHighlight.SetActive(true);
        //determine if attack or move    
        //set color if attack or move
        var highlightSprite = validMoveHighlight.GetComponent<SpriteRenderer>();
        if (occupiedUnit != null && occupiedUnit.faction != attacker.faction){
            highlightSprite.color = MenuManager.instance.attackColor;
            moveType = TileMoveType.Attack;
        } else if (editorType == TileEditorType.Mountain){
            validMoveHighlight.SetActive(false);
            moveType = TileMoveType.NotValid;
        } else {
            highlightSprite.color = MenuManager.instance.inRangeColor;
            moveType = TileMoveType.InAttackRange;
        }
    }
    public List<BaseTile> GetPathFrom(BaseTile startPos){
        List<BaseTile> path = new List<BaseTile>();
        var startCoordiantes = startPos.coordiantes;
        int x = (int)Mathf.Abs(startCoordiantes.x - coordiantes.x);
        int y = (int)Mathf.Abs(startCoordiantes.y - coordiantes.y);


        //TODO: THIS MAY TAKE PATHS THROUGH WALLS MAKE BETTER !!!!!
        depth = 111;
        validPath = GridManager.instance.ShortestPathBetweenTiles(startPos, this, true);
        if (validPath != null){
            depth = validPath.Count;
        }
        return path;
    }
    /// <summary>
    /// Gets the tiles (up to 4) adjaecnt to this tile
    /// </summary>
    public List<BaseTile> GetAdjacentTiles(){
        int left = (int)coordiantes.x - 1;
        int right = (int)coordiantes.x + 1;
        int up = (int)coordiantes.y - 1;
        int down = (int)coordiantes.y + 1;

        Vector2 l = new Vector2(left, coordiantes.y);
        Vector2 r = new Vector2(right, coordiantes.y);
        Vector2 u = new Vector2(coordiantes.x, up);
        Vector2 d = new Vector2(coordiantes.x, down);
        
        Vector2[] array = {l, r, u, d};
        List<BaseTile> newTiles = new List<BaseTile>();
        for (int i = 0; i < 4; i++){
            newTiles.Add(GridManager.instance.GetTileAtPosition(array[i]));
        }
        return newTiles;

    }
    /// <summary>
    /// Render the PathLine from the current selected units tile to this tile
    /// </summary>
    private void RerenderLine(){
        if (UnitManager.instance.selectedUnit is MeleeUnit && this.moveType == TileMoveType.Attack){
            return;
        }
        PathLine.instance.RenderLine(UnitManager.instance.selectedUnit.occupiedTile, this);
        validPath = PathLine.instance.GetPath();
    }

    //TODO: REMOVE SINCE NO MORE PROCGEN
    public void SetBGSprite(Sprite s){
        bgSprite.sprite = s;
    }
    public void SetMidSprite(Sprite s){
        middleSprite.sprite = s;
    }
    public void SetFGSprite(Sprite s){
        fgSprite.sprite = s;
    }

    // Plays unit move particles
    public void PlayMoveParticles() {
        if (moveParticles == null) {
            Init(0, 0); // Band-aid to ensure this function is actually called
        }

        switch (editorType) {
            case TileEditorType.Grass:
                moveParticles[0].Play();
                break;
            case TileEditorType.Bridge:
                moveParticles[1].Play();
                break;
            case TileEditorType.Mountain:
                moveParticles[2].Play();
                break;
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