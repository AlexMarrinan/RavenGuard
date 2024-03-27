using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SkillManager : MonoBehaviour
{
    public Vector2 useDirection = Vector2.right;
    public static SkillManager instance;
    public bool selectingSkill = false;
    public bool skillFailed = false;
    public List<BaseTile> currentTiles;
    public BaseUnit user;
    public ActiveSkill currentActiveSkill;
    public BaseTile selectedTile;
    public Color activeSkillColor, passiveSkillColor;
    public void Awake(){
        instance = this;
    }

    public void ShowSkillPreview(){
        UnitManager.instance.RemoveAllValidMoves();
        if (selectingSkill == true && currentActiveSkill.shape == SkillShape.None 
            && currentActiveSkill.activeSkillType == ActiveSkillType.OnSelf){
            Debug.Log("using self skill");
            currentActiveSkill.OnUse(user);
            return;
        }
        currentTiles = currentActiveSkill.GetAffectedTiles(user);

        if (currentTiles.Contains(user.occupiedTile) && currentActiveSkill.activeSkillType != ActiveSkillType.OnSelf){
            currentTiles.Remove(user.occupiedTile);
        }
        UnitManager.instance.RemoveAllValidMoves();

        foreach (BaseTile t in currentTiles){

            t.moveType = GetSkillMoveType(t);
            t.SetSkillMove(true, user.occupiedTile);
            
        }
    }
    private TileMoveType GetSkillMoveType(BaseTile tile){
        var activeSkill = currentActiveSkill as ActiveSkill;
        if (activeSkill.activeSkillType == ActiveSkillType.OnTile){
            return currentActiveSkill.tileMoveType;
        }
        bool hasUnit = tile.occupiedUnit != null;
        if (activeSkill.activeSkillType == ActiveSkillType.OnUnit && hasUnit){
            return currentActiveSkill.tileMoveType;
        }
        bool hasSelf = hasUnit && tile.occupiedUnit == UnitManager.instance.selectedUnit;
        if (activeSkill.activeSkillType == ActiveSkillType.OnSelf){ //&& hasSelf){
            return currentActiveSkill.tileMoveType;
        }
        if (activeSkill.activeSkillType != ActiveSkillType.OnSelf && hasSelf){
            return TileMoveType.NotValid;
        }

        bool hasHero = hasUnit && tile.occupiedUnit.faction == UnitFaction.Hero;
        if (activeSkill.activeSkillType == ActiveSkillType.OnHero && hasHero){
            return currentActiveSkill.tileMoveType;
        }

        bool hasEnemy = hasUnit && tile.occupiedUnit.faction == UnitFaction.Enemy;
        if (activeSkill.activeSkillType == ActiveSkillType.OnEnemy && hasEnemy){
            return currentActiveSkill.tileMoveType;
        }

        return TileMoveType.InAttackRange;
    }
    public BaseUnit SelectUnitFromTiles(UnitFaction faction){
        var tiles = currentTiles;
        foreach (BaseTile tile in tiles){
            if (tile.occupiedUnit != null && tile.occupiedUnit.faction == faction){
                return tile.occupiedUnit;
            }
        }  
        return null;
    }
    public void CoverAS(BaseUnit u){
        
        BaseUnit target = selectedTile.occupiedUnit;
        if (target == null){
            return;
        }
        Debug.Log("switching...");
        BaseTile uTile = u.occupiedTile;
        BaseTile targetTile = target.occupiedTile;
        
        Vector2 direction = uTile.coordiantes - targetTile.coordiantes;
        BaseTile newTile = GridManager.instance.GetTileAtPosition(uTile.coordiantes + direction);
        if (newTile.occupiedUnit != null || newTile is WallTile){
            Debug.Log("new tile is occupied!");
            skillFailed = true;
            return;
        }

        //Move target to new tile
        newTile.occupiedUnit = target;
        target.occupiedTile = newTile;
        target.transform.position = newTile.transform.position;

        targetTile.occupiedUnit = null;
    }
    public void SwitchAS(BaseUnit u){
        
        BaseUnit switchUnit = selectedTile.occupiedUnit;
        if (switchUnit == null){
            return;
        }
        Debug.Log("switching...");
        BaseTile uTile = u.occupiedTile;
        BaseTile switchTile = switchUnit.occupiedTile;

        switchTile.occupiedUnit = u;
        u.occupiedTile = switchTile;
        u.transform.position = switchTile.transform.position;

        uTile.occupiedUnit = switchUnit;
        switchUnit.occupiedTile = uTile;
        switchUnit.transform.position = uTile.transform.position;
    }
    public void PainTransferAS(BaseUnit u){
        BaseUnit transferUnit = selectedTile.occupiedUnit;
        if (transferUnit == null){
            return;
        }
        int healthToGive = currentActiveSkill.skillParam1;
        int newHealth = healthToGive - u.health + (healthToGive * (currentActiveSkill.skillParam2 / 100));
        Debug.Log("new health... " + newHealth);

        if (newHealth > 0){
            healthToGive += -newHealth - 1;
        }
        Debug.Log("transfering... " + healthToGive);
        u.ReceiveDamage(healthToGive * (100-currentActiveSkill.skillParam2) / 100);
        transferUnit.RecoverHealth(healthToGive);
    }

    //Does damage to the selected unit, damage provided by skill used
    private void DamageHelper(BaseUnit u, int damage, bool lethal = false){
        BaseUnit otherUnit = selectedTile.occupiedUnit;
        if (otherUnit == null){
            return;
        }
        if (lethal){
            otherUnit.ReceiveDamage(damage);
        }else{
            otherUnit.ReciveNonlethalDamage(damage);
        }
    }

    public void BurstAS(BaseUnit u){
        int damage = u.GetForesight().total * currentActiveSkill.skillParam1 / 100;
        Debug.Log("Used Burst...");
        DamageHelper(u, damage);
    }
    
    public void BashAS(BaseUnit u){
        int damage = u.GetDefense().total * currentActiveSkill.skillParam1 / 100;
        Debug.Log("Used Burst...");
        DamageHelper(u, damage);
    }
    public void PhantomSlashAS(BaseUnit u){
        if (selectedTile == null){
            return;
        }
        BaseUnit target = selectedTile.occupiedUnit;
        if (target == null || target == u){
            return;
        }
        int damage = u.GetAttack().total - (target.GetDefense().total * currentActiveSkill.skillParam1 / 100);
        Debug.Log("Used PhantomSlash...");

        if (currentActiveSkill.skillParam1 == 0){
            target.ReciveNonlethalDamage(damage);
        }else{
            target.ReceiveDamage(damage);
        }
    }
    public void EnforceAS(BaseUnit u){
        if (currentActiveSkill.skillLevel == 1){
            BaseUnit otherUnit = selectedTile.occupiedUnit;
            if (otherUnit == null){
                return;
            }
            otherUnit.AddStatsChange("EnforceDEF", UnitStatType.Defense, 4, 4, 4, 1);
            otherUnit.AddStatsChange("EnforceFOR", UnitStatType.Foresight, 4, 4, 4, 1);
        }else{
            foreach (BaseTile tile in currentTiles){
                BaseUnit target = tile.occupiedUnit;
                if (target == null || target == u || target.faction != u.faction){
                    continue;
                }
                target.AddStatsChange("EnforceDEF", UnitStatType.Defense, 4, 4, 4, 1);
                target.AddStatsChange("EnforceFOR", UnitStatType.Foresight, 4, 4, 4, 1);
            }
        }
    }
    public void GuardAS(BaseUnit u){
        u.AddStatsChange("GuardATK", UnitStatType.Attack, -5, -5, -5, 1);
        u.AddStatsChange("GuardDEF", UnitStatType.Defense, 10, 10, 10, 1);
    }

    public void WhirlwindAS(BaseUnit u){

        Debug.Log("Used Whirlwind...");
        var tiles = SkillManager.instance.currentTiles;
        foreach (BaseTile tile in tiles){
            BaseUnit target = tile.occupiedUnit;
            if (target == null || target == u){
                continue;
            }
            int damage = u.GetAttack().total - target.GetDefense().total;
            if (damage < 0){
                damage = 0;
            }
            target.ReciveNonlethalDamage(damage);
        }
    }
    //Upon ending unit's action or end of combat, adjacent allies are cleansed of all clearable debuffs.
    public void CleansePS(BaseUnit u){
        u.Cleanse();
        u.GetAdjacentUnits().ForEach(u => u.Cleanse());
    }
    public void CripplingAimPS(BaseUnit u){
        BattleUnit bu = BattleSceneManager.instance.GetBattleUnit(u);
        if (bu.damageDealt >= 10){
            BattleUnit otherBU = BattleSceneManager.instance.GetOtherBattleUnit(u);
            if (otherBU.assignedUnit != null){
                otherBU.assignedUnit.AddBuff(new CrippledBuff(u, otherBU.assignedUnit));
            }
        }
    }
    public void CrossfirePS(BaseUnit u){
        BattleUnit bu = BattleSceneManager.instance.GetBattleUnit(u);
        int damage = bu.damageDealt * 1/4;
        BattleUnit otherbu = BattleSceneManager.instance.GetOtherBattleUnit(u);

        var units = otherbu.assignedUnit.GetAdjacentUnits();
        units.ForEach(unit => unit.ReceiveDamage(damage));
    }

    public void AdrenalineBurstPS(BaseUnit u){
//      Adrenaline Burst should not be able to kill its unit
        if (u.health <= 2){
            u.health = 1;
        }else{
            u.ReceiveDamage(2);
        }
    }
    public void PoisonArrowsPS(BaseUnit u){
        BattleUnit otherbu = BattleSceneManager.instance.GetOtherBattleUnit(u);
        otherbu.assignedUnit.AddBuff(new PoisonBuff(u, otherbu.assignedUnit, 2));
    }
    //If unit moves 0 or 1 spaces, unit gains +2 defense and +1 attack during combat.
    //This effect stack for each consecutive turn in a row, up to three turns. 
    //Then, stays at 3 turns. Moving more than the amount of spaces resets the stats.
    public void HunkerDownPS(BaseUnit u){
        //Called After Movment
//        Debug.Log("hunkering down...");
        var defStats = u.GetStatChange("HunkerDownDef");
        if (defStats == null){
            u.AddStatsChange("HunkerDownDef", UnitStatType.Defense, 0, 0, 6);
        }
        var atkStats = u.GetStatChange("HunkerDownAttack");
        if (atkStats == null){
            u.AddStatsChange("HunkerDownAttack", UnitStatType.Attack, 0, 0, 3);
        }
        if (u.moveAmount <= 1){
            u.IncrementStatsChange("HunkerDownDef", + 2);
            u.IncrementStatsChange("HunkerDownAttack", + 1);
        }else{
            u.SetStatChange("HunkerDownDef", 0);
            u.SetStatChange("HunkerDownAttack", 0);
        }
    }   


    //Unit heals 7 HP after combat if unit attacked.
    public void RecoveryPS(BaseUnit u){
        int healing = 4;
        //Called After Movment
        if (BattleSceneManager.instance.UnitAttacked(u)){
            u.RecoverHealth(healing);
        }
    }   
    public void CripplingJealousyPS (BaseUnit u){
        var units = UnitManager.instance.GetAllUnits();
        var coords = u.occupiedTile.coordiantes;
        //Debug.Log("searching for jealousy");

        foreach (var unit in units){
            if (u != unit && (coords.x == unit.occupiedTile.coordiantes.x || coords.y == unit.occupiedTile.coordiantes.y)){
                if (u.GetForesight().total > unit.GetForesight().total){
                    //Debug.Log("Applying jealousy");
                    unit.AddBuff(new JelousBuff(u, unit));
                }
            }
        }
    }
    public void WeakpointManipulationPS(BaseUnit u){
        var units = u.GetAdjacentUnits();
        foreach (BaseUnit unit in units){
            if (unit.faction == u.faction && (unit.weaponClass == WeaponClass.Magic || unit.unitClass == UnitClass.Mage)){
                u.AddBuff(new DualityBuff(u, u));
                return;
            }
        }
    }
    internal void Move(Vector2 moveVector)
    {
        AudioManager.instance.PlayMove();
        if (moveVector.x != 0 && moveVector.y != 0){
            return;
        }
        if (moveVector.x == 0 && moveVector.y == 0){
            return;
        }
        useDirection = moveVector;
        ShowSkillPreview();
    }

    internal void Select()
    {
        Debug.Log(currentActiveSkill);
        AudioManager.instance.PlayConfirm();
        currentActiveSkill.OnUse(user);
    }
    internal void OnSkilEnd()
    {
        Debug.Log("Ending skill");
        selectingSkill = false;
        currentTiles = new List<BaseTile>();
        UnitManager.instance.RemoveAllValidMoves();
        user.FinishTurn();
    }
}