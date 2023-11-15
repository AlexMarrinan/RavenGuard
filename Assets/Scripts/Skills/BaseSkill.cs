using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;
using System.Reflection;

public class BaseSkill : BaseItem {
    public string skillName;
    public string description;
    public UnitClass unitClass;
    public WeaponClass weaponClass;
    protected MethodInfo methodInfo;
    public bool centered = false;
    public SkillShape shape;
    public int range1 = 1;
    public int range2 = 1;
    public TileMoveType tileMoveType;
    public virtual void OnSelect(BaseUnit user){

    }
    public virtual void OnUse(BaseUnit user){

    }
    public virtual void SetMethod(){

    }
    public List<BaseTile> GetAffectedTiles(BaseUnit user){
        switch (shape){
            case SkillShape.Radius:
                return GetRadiusTiles(user);
            case SkillShape.Rectangle:
                return GetRectangleTiles(user);
            case SkillShape.Cone:
                return GetConeTiles(user);
        }
        return null;
    }

    private List<BaseTile> GetRadiusTiles(BaseUnit user)
    {   
        //range 1 is radius
        //range 2 is distance away from unit, if not centered
        BaseTile t = user.occupiedTile;
        if (centered){
            Debug.Log("radius centered!");
            return GridManager.instance.GetRadiusTiles(t, range1);
        }
        BaseTile newT = GridManager.instance.GetTileAtPosition(t.coordiantes + range2*SkillManager.instance.useDirection);
        return GridManager.instance.GetRadiusTiles(newT, range1);
    }
    private List<BaseTile> GetRectangleTiles(BaseUnit user)
    {
        BaseTile t = user.occupiedTile;
        if (centered){
            Debug.Log("centered!");
            return GridManager.instance.GetRectangleTiles(t, range1, range2);
        }
        BaseTile newT = GridManager.instance.GetTileAtPosition(t.coordiantes + SkillManager.instance.useDirection);
        List<BaseTile> potentialTiles = GridManager.instance.GetRectangleTiles(newT, range1, range2);
        while (potentialTiles.Contains(user.occupiedTile)){
            newT = GridManager.instance.GetTileAtPosition(newT.coordiantes + SkillManager.instance.useDirection);
            potentialTiles = GridManager.instance.GetRectangleTiles(newT, range1, range2);
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
    Radius,
    Rectangle,
    Cone,
}