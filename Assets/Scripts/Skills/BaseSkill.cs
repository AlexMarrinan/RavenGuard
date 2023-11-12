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
    public virtual void OnSelect(BaseUnit user){

    }
    public virtual void OnUse(BaseUnit user){

    }
    public virtual void SetMethod(){

    }
    public List<Tile> GetAffectedTiles(BaseUnit user){
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

    private List<Tile> GetRadiusTiles(BaseUnit user)
    {   
        //range 1 is radius
        //range 2 is distance away from unit, if not centered
        Tile t = user.occupiedTile;
        if (centered){
            Debug.Log("centered!");
            return GridManager.instance.GetRadiusTiles(t, range1);
        }
        Tile newT = GridManager.instance.GetTileAtPosition(t.coordiantes + range2*SkillManager.instance.useDirection);
        return GridManager.instance.GetRadiusTiles(newT, range1);
    }
    private List<Tile> GetRectangleTiles(BaseUnit user)
    {
        Tile t = user.occupiedTile;
        if (centered){
            Debug.Log("centered!");
            return GridManager.instance.GetRectangleTiles(t, range1, range2);
        }
        Tile newT = GridManager.instance.GetTileAtPosition(t.coordiantes + SkillManager.instance.useDirection);
        List<Tile> potentialTiles = GridManager.instance.GetRectangleTiles(newT, range1, range2);
        while (potentialTiles.Contains(user.occupiedTile)){
            newT = GridManager.instance.GetTileAtPosition(newT.coordiantes + SkillManager.instance.useDirection);
            potentialTiles = GridManager.instance.GetRectangleTiles(newT, range1, range2);
        }
        return potentialTiles;
    }

    private List<Tile> GetConeTiles(BaseUnit user)
    {
        return null;
    }
}


public enum SkillShape {   
    None,
    Radius,
    Rectangle,
    Cone,
}