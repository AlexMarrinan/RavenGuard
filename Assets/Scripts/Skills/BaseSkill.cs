using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;
using System.Reflection;
using Hub.Blacksmith;

public class BaseSkill : BaseItem
{

    [Header("Menu Info")] 
    public string skillName;
    public int skillLevel = 1;
    public string description;
    public Sprite menuIcon;
    public List<Sprite> skillIcons;
    public RarityFilter rarity;
    public SkillTypeFilter skillType;
    public SkillRestrictionsFilter skillRestrictions;

    [Header("Skill Progression")] public SkillProgressionGroup progressionGroup;
    
    [Header("General Info")]
    public UnitClass unitClass;
    public WeaponClass weaponClass;
    protected MethodInfo methodInfo;
    public bool centered = false;
    public SkillShape shape;
    public int range1 = 1;
    public int range2 = 1;
    public int skillParam1 = 0;
    public int skillParam2 = 0;
    public TileMoveType tileMoveType;
    public virtual void OnSelect(BaseUnit user){

    }
    public virtual void OnUse(BaseUnit user){

    }
    public virtual void SetMethod(){

    }
    public List<BaseTile> GetAffectedTiles(BaseUnit user){
        switch (shape){
            case SkillShape.None:
                return new (){user.occupiedTile};
            case SkillShape.Adjacent:
                return GetAdjTiles(user);
            case SkillShape.Radius:
                return GetRadiusTiles(user);
            case SkillShape.Rectangle:
                return GetRectangleTiles(user);
            case SkillShape.Cone:
                return GetConeTiles(user);
        }
        return null;
    }

    private List<BaseTile> GetAdjTiles(BaseUnit user)
    {
        return GridManager.instance.GetAdjacentTiles(user.occupiedTile.coordiantes);
    }

    private List<BaseTile> GetRadiusTiles(BaseUnit user)
    {   
        //range 1 is radius
        //range 2 is distance away from unit, if not centered
        int range = range1;
        if (user.HasPassiveSkill("PotentMagic")){
            range++;
        }
        BaseTile t = user.occupiedTile;
        if (centered){
            Debug.Log("radius centered!");
            return GetRadiusTiles(t, range);
        }
        BaseTile newT = GridManager.instance.GetTileAtPosition(t.coordiantes + range2*SkillManager.instance.useDirection);
        return GetRadiusTiles(newT, range);
    }
    private List<BaseTile> GetRadiusTiles(BaseTile t, int maxDepth){
        var visited = new Dictionary<BaseTile, int>();
        visited[t] = 0;
        var next = t.GetAdjacentTiles();
        next.ForEach(t => GetRadiusTilesHelper(1, maxDepth, t, visited, t));
        var validMoves = visited.Keys.ToList();
        return validMoves;
    }

    private void GetRadiusTilesHelper(int depth, int max, BaseTile tile, Dictionary<BaseTile, int> visited, BaseTile startTile){
        if (depth >= max ){
            return;
        }
        //if tile is not valid, continue
        if (tile == null || tile is WallTile || (visited.ContainsKey(tile) && visited[tile] == depth)){
            return;
        }

        //if tile is valid, add it to the list of visited tiles and continue
        visited[tile] = depth;
        var next = tile.GetAdjacentTiles();   
        next.ForEach(t => GetRadiusTilesHelper(depth + 1, max, t, visited, startTile));
        return;
    }
    private List<BaseTile> GetRectangleTiles(BaseUnit user)
    {
        BaseTile t = user.occupiedTile;
        int range = range1;
        if (user.HasPassiveSkill("PotentMagic")){
            range++;
        }
        if (centered){
            Debug.Log("centered!");
            return GridManager.instance.GetRectangleTiles(t, range, range2);
        }
        BaseTile newT = GridManager.instance.GetTileAtPosition(t.coordiantes + SkillManager.instance.useDirection);
        List<BaseTile> potentialTiles = GridManager.instance.GetRectangleTiles(newT, range, range2);
        while (potentialTiles.Contains(user.occupiedTile)){
            newT = GridManager.instance.GetTileAtPosition(newT.coordiantes + SkillManager.instance.useDirection);
            potentialTiles = GridManager.instance.GetRectangleTiles(newT, range, range2);
        }
        return potentialTiles;
    }

    private List<BaseTile> GetConeTiles(BaseUnit user)
    {
        return null;
    }
    internal bool HasMethod()
    {
        return methodInfo != null;
    }
}


public enum SkillShape {   
    None,
    Adjacent,
    Radius,
    Rectangle,
    Cone,
}