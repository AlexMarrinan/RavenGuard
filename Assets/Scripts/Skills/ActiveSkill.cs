using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;


[CreateAssetMenu(fileName = "[Name]AS", menuName = "Skill/Active", order = 0)]
public class ActiveSkill : BaseSkill {
    public int cooldown = 0;
    public ActiveSkillType activeSkillType;
    /// <summary>
    /// Sets the backend method for the given skill based on its name
    /// </summary>
    public override void SetMethod(){
        var mng = SkillManager.instance;
        var type = mng.GetType();
        methodInfo = type.GetMethod(base.skillName + "AS");
    }
    /// <summary>
    /// Called when this active skill is selected in the UnitActionMenu
    /// </summary>
    /// <param name="user">unit that is using the skill</param>
     public override void OnSelect(BaseUnit user){
        Debug.Log("on select");
        MenuManager.instance.ToggleUnitActionMenu();
        SkillManager.instance.currentSkill = this;
        SkillManager.instance.user = user;
        SkillManager.instance.selectingSkill = true;
        SkillManager.instance.ShowSkillPreview();
    }
     /// <summary>
    /// If succesful, calls the active skills backend method to perform the skill
    /// </summary>
    /// <param name="user">unit that is using the skill</param>
    public override void OnUse(BaseUnit user){
        var tile = GridManager.instance.hoveredTile;
        if (tile.moveType != tileMoveType){
            return;
        }
        var mng = SkillManager.instance;
        var param = new object[1];
        SkillManager.instance.selectedTile = GridManager.instance.hoveredTile;
        param[0] = user;
        methodInfo.Invoke(mng, param);
        if (SkillManager.instance.skillFailed){
            SkillManager.instance.skillFailed = false;
            return;
        }
        user.PutSkillOnCooldown(this);
        SkillManager.instance.OnSkilEnd();
    }
    private void RangeHelper(int depth, int max, BaseTile tile, Dictionary<BaseTile, int> visited, BaseTile startTile, BaseUnit startUnit){
        if (depth >= max){
            return;
        }
        if (tile == null){
            return;
        }
        // //enemy's are valid moves but block movement
        // if (tile != null && tile.occupiedUnit != null && tile.occupiedUnit.faction != startUnit.faction){
        //     visited[tile] = depth;
        //     return;
        // }
        // //if tile is not valid, continue
        // if (tile == null || !tile.walkable || (visited.ContainsKey(tile) && visited[tile] == depth)){
        //     return;
        // }

        //if tile is valid, add it to the list of visited tiles and continue
        visited[tile] = depth;
        var next = tile.GetAdjacentTiles();   
        next.ForEach(t => RangeHelper(depth + 1, max, t, visited, startTile, startUnit));
        return;
    }
}

public enum ActiveSkillType {
    OnSelf,
    OnUnit,
    OnHero,
    OnEnemy,
    OnTile,
}
