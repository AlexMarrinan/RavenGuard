using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseUnit : MonoBehaviour
{
    public string unitName;
    public BaseTile occupiedTile;
    public UnitFaction faction;
    public UnitClass unitClass;
    private ArmorType armorType;
    /*** Unit Stats ***/
    public int moveAmount;
    public int maxMoveAmount;
    public int health;
    public int maxHealth;
    [SerializeField] private int attack;
    [SerializeField] private int defense;
    [SerializeField] private int agility;
    [SerializeField] private int attunment;
    [SerializeField] private int foresight;
    [SerializeField] private int luck;
    [HideInInspector]
    public Dictionary<string, SkillStatChange> skillStatChanges = new();

    // [HideInInspector]
    public bool awaitingOrders;
    public UnitHealthBar healthBar;
    public SpriteRenderer spriteRenderer;
    public List<BaseSkill> skills;
    [HideInInspector] public BaseWeapon weapon;
    [HideInInspector] public WeaponClass weaponClass;
    public RuntimeAnimatorController animatorController;
    private bool isAggroed = false;
    public List<UnitStatMultiplier> tempStatChanges;
    public List<Buff> buffs = new();
    internal AttackEffect attackEffect;
    protected int reducedMovment = 0;
    public Dictionary<UnitStatType, int> duringCombatStats = new();

    void Start(){
        //RandomizeUnitClass();
        attackEffect = AttackEffect.None;
        buffs = new();
        ResetCombatStats();
        InitializeUnitClass();
        InitializeFaction();
        CreateHealthbar();
        SetSkillMethods();
    }

    public void ResetCombatStats(){
        foreach (UnitStatType ust in Enum.GetValues(typeof(UnitStatType))){
            Debug.Log(ust);
            duringCombatStats[ust] = 0;
        }
    }
    public void BuffAllCombatStats(int amount){
        foreach (UnitStatType ust in Enum.GetValues(typeof(UnitStatType))){
            duringCombatStats[ust] = amount;
        }
    }
    public void BuffCombatStat(UnitStatType type, int amount){
        duringCombatStats[type] = amount;
    }
    public void SetSkillMethods(){
        //TODO: ONLY SET SKILL METHODS ON GAME STARTUP
        foreach (var skill in skills){
//            Debug.Log(skill.skillName);
            skill.SetMethod();
        }
    }
    private void RandomizeUnitClass(){
        Array values = Enum.GetValues(typeof(UnitClass));
        var random = new System.Random();
        unitClass = (UnitClass)values.GetValue(random.Next(values.Length));
    }
    public BaseSkill GetBoringSkill(){
        return null;
    }
    public virtual void ApplyWeapon(){

    }
    private void InitializeUnitClass()
    {
        switch (unitClass){
            case UnitClass.Infantry:
                armorType = ArmorType.Medium;
                break;
            case UnitClass.Knight:
                armorType = ArmorType.Heavy;
                break;
            case UnitClass.Assassin:
                armorType = ArmorType.Light;
                break;
           case UnitClass.Mage:
                armorType = ArmorType.Medium;
                break;
            case UnitClass.Sage:
                armorType = ArmorType.Light;
                break;
            case UnitClass.Cavalry:
                armorType = ArmorType.Medium;
                break;
            case UnitClass.Paladin:
                armorType = ArmorType.Heavy;
                break;
        }
    }

    public virtual int MaxTileRange(){
        return 0;
    }
    private void InitializeFaction(){
        if (this.faction == UnitFaction.Hero){
            spriteRenderer.color = Color.cyan;
        }else{
            spriteRenderer.color = Color.red;
            spriteRenderer.transform.rotation = new Quaternion(0, 180, 0, 0);
        }
    }
    private void CreateHealthbar(){
        var prefab = Resources.Load("HealthBar");
        var go = (GameObject)Instantiate(prefab);
        ((GameObject)go).transform.SetParent(GameManager.instance.mainCanvas.transform);
        healthBar = go.GetComponent<UnitHealthBar>();
        healthBar.SetAttachedUnit(this);
    }
    public virtual void Attack(BaseUnit otherUnit){
        return;
    }
    public virtual void Heal(BaseUnit otherUnit){
        return;
    }
    public int GetDamage(BaseUnit otherUnit){
        int attackDmg = GetAttackDamage(otherUnit);
        int magicDmg = GetMagicDamage(otherUnit);
        bool attackHigher = attackDmg > magicDmg;

        if (attackEffect == AttackEffect.Duality){
            return attackHigher ? attackDmg : magicDmg;
        }
        if (attackEffect == AttackEffect.Confusion){
            return !attackHigher ? attackDmg : magicDmg;
        }
        if (weaponClass == WeaponClass.Magic){
            return magicDmg;
        }
        return attackDmg;
    }
    private int GetAttackDamage(BaseUnit otherUnit){
        int damage = this.GetAttack().total - otherUnit.GetDefense().total;
        if (damage <= 0){
            return 0;
        }
        if (tempStatChanges != null){
            foreach (UnitStatMultiplier mult in tempStatChanges){
                if (mult.statType == UnitStatType.Attack){
                    damage = (int)((float)damage * mult.multiplier);
                }
            }
        }
        return damage;
    }
    private int GetMagicDamage(BaseUnit otherUnit){
        int damage = this.GetAttuenment().total - otherUnit.GetForesight().total;
        if (damage <= 0){
            return 0;
        }
        if (tempStatChanges != null){
            foreach (UnitStatMultiplier mult in tempStatChanges){
                if (mult.statType == UnitStatType.Attunment){
                    damage = (int)((float)damage * mult.multiplier);
                }
            }
        }
        return damage;
    }
    public void ReceiveDamage(BaseUnit otherUnit){
        health -= otherUnit.GetDamage(this);;
    }
    public void ReceiveDamage(int damage){
        health -= damage;
        if (health <= 0){
            UnitManager.instance.DeleteUnit(this);
        }
    }
    public void RecoverHealth(int healing){
        health += healing;
        if (health > maxHealth){
            health = maxHealth;
        }
    }
    public void MoveToAttackTile(){
        if (TurnManager.instance.currentFaction == UnitFaction.Enemy){
            return;
        }
        BaseTile lastTile = PathLine.instance.GetLastTile();
        //GameManager.instance.PanCamera(adjTile.transform.position);
        UnitManager.instance.RemoveAllValidMoves();
        if (lastTile != null){
            lastTile.MoveUnitToTile(UnitManager.instance.selectedUnit, false);
        }
        //healthBar.RenderHealth();
    }
    public void MoveToTileAtDistance(int distance){
        BaseTile adjTile = PathLine.instance.GetPathTile(distance);
        adjTile.MoveUnitToTile(UnitManager.instance.selectedUnit);
        //healthBar.RenderHealth();
    }
    public void ResetMovment(){
        InitializeFaction();
        moveAmount = maxMoveAmount;
    }

    public void OnExhaustMovment(){
        // moveAmount = 0;
        spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f);
        UsePassiveSkills(PassiveSkillType.OnMovement);
        UnitManager.instance.SetSeclectedUnit(null);
        TurnManager.instance.OnUnitDone(this);
    }
    public virtual TileMoveType GetMoveTypeAt(BaseTile otherTile){
        return TileMoveType.NotValid;
    }  

    #region Stat Getters

    // TODO: ADD STAT BONUS CALCULATIONS !!!
    // XP, LEVELUPS, SKILLS, ETC.
    public UnitStat GetAttack(){
        //TODO: ADD WEAPON DAMAGE
        return new UnitStat(UnitStatType.Attack, attack, GetStatChangeOfType(UnitStatType.Attack));
    }
    public UnitStat GetDefense(){
        return new UnitStat(UnitStatType.Defense, defense, GetStatChangeOfType(UnitStatType.Defense));
    }
    public UnitStat GetAgility(){
        return new UnitStat(UnitStatType.Agility, agility, GetStatChangeOfType(UnitStatType.Agility));
    }
    public UnitStat GetAttuenment(){
        //TODO: ADD WEAPON DAMAGE
        return new UnitStat(UnitStatType.Attunment, attunment, GetStatChangeOfType(UnitStatType.Attunment));
    }
    public UnitStat GetForesight(){
        return new UnitStat(UnitStatType.Foresight, foresight, GetStatChangeOfType(UnitStatType.Foresight));
    }
    public UnitStat GetLuck(){
        return new UnitStat(UnitStatType.Luck, luck, GetStatChangeOfType(UnitStatType.Luck));
    }
    #endregion


    public Sprite GetSprite(){
        return spriteRenderer.sprite;
    }
    public Color GetColor(){
        return spriteRenderer.color;
    }

    internal BaseSkill GetSkill(int v)
    {
        if (v >= skills.Count){
            return null;
        }
        return skills[v];
    }
    // public void EquipSkill(BaseSkill newSkill){
    //     equippedSkill = newSkill;
    // }
    // public BaseSkill GetEquipedSkill(){
    //     return equippedSkill;
    // }
    // public void UnequipSkill(){
    //     equippedSkill = null;
    // }
    private int GetStatChangeOfType(UnitStatType type){
        int newAmount = 0;
        foreach (var change in skillStatChanges.Values){
            if (change.statType == type){
                newAmount += change.currentAmount;
            }
        }
        return newAmount + duringCombatStats[type];
    }
    public SkillStatChange GetStatChange(string name){
        if (skillStatChanges.ContainsKey(name)){
            return skillStatChanges[name];
        }
        return null;
    }
    public void AddStatsChange(string name, UnitStatType type, int startAmount, int minAmount, int maxAmount){
        if (GetStatChange(name) == null){
            var newStats = new SkillStatChange(type, startAmount, minAmount, maxAmount);
            skillStatChanges.Add(name, newStats);
        }else{
            SetStatChange(name, startAmount);
        }
    }
    public void IncrementStatsChange(string name, int amount){
        var stats = GetStatChange(name);
        if (stats == null){
            return;
        }
        stats.currentAmount += amount;
        if (stats.currentAmount > stats.maxAmount){
            stats.currentAmount = stats.maxAmount;
        }
    }
    public void SetStatChange(string name, int amount){
        var stats = GetStatChange(name);
        if (stats == null){
            return;
        }
        if (amount > stats.maxAmount || amount < stats.minAmount){
            return;
        }
        stats.currentAmount = amount;
    }
    
    public void RemoveStatChange(string name){
        var stats = GetStatChange(name);
        if (stats == null){
            return;
        }
        skillStatChanges.Remove(name);
    }
    public List<ActiveSkill> GetActiveSkills(){
        List<ActiveSkill> aSkills = new();
        foreach (BaseSkill s in skills){
            if (s is ActiveSkill){
                aSkills.Add(s as ActiveSkill);
            }
        }
        return aSkills;
    }
    public List<PassiveSkill> GetPassiveSkills(){
        List<PassiveSkill> pSkills = new();
        foreach (BaseSkill s in skills){
            if (s is PassiveSkill){
                pSkills.Add(s as PassiveSkill);
            }
        }
        return pSkills;
    }


    public List<CombatPassiveSkill> GetBattleSkills(){
        List<CombatPassiveSkill> pSkills = new();
        foreach (BaseSkill s in skills){
            if (s is CombatPassiveSkill){
                pSkills.Add(s as CombatPassiveSkill);
            }
        }
        return pSkills;
    }
    public void UseActiveSkill(ActiveSkillType type){
        var aSkills = GetActiveSkills();
        foreach (ActiveSkill aSkill in aSkills){
            if (aSkill.activeSkillType == type){
                aSkill.OnUse(this);
            }
        }
    }

    public void UsePassiveSkills(PassiveSkillType type){
        var pSkills = GetPassiveSkills();
        foreach (PassiveSkill pSkill in pSkills){
            if (pSkill is CombatPassiveSkill && !pSkill.HasMethod()){
                continue;
            }
            if (pSkill.passiveSkillType == type){
                pSkill.OnUse(this);
            }
        }
    }
    public List<BaseUnit> GetAdjacentUnits(){
        return GetAdjacentUnitsOfFaction(this.faction);
    }
    public List<BaseUnit> GetAdjacentUnitsOfFaction(UnitFaction faction){
        var tiles = GridManager.instance.GetAdjacentTiles(this.occupiedTile.coordiantes);
        List<BaseUnit> adjUnits = new();
        foreach (BaseTile tile in tiles){
            if (tile != null && tile.occupiedUnit != null && (tile.occupiedUnit.faction == faction || faction == UnitFaction.Both)){
                adjUnits.Add(tile.occupiedUnit);
            }
        }
        return adjUnits;
    }
    public void Cleanse(){
        buffs.RemoveAll(b => !b.positive);
    }

    public bool IsInjured(){
        return false;
        return health <= maxHealth * 0.2;
    }

    public bool IsAggroed(){
        return true;
        return isAggroed;
    }
    public void SetAggro(bool b){
        isAggroed = b;
    }

    public UnitFaction GetOtherFaction(){
        if (faction == UnitFaction.Hero){
            return UnitFaction.Enemy;
        }
        return UnitFaction.Hero;

    }
    internal bool AlliesInRange() {
        return UnitsInRange(faction);
    }
    internal bool OpponentInRange() {
        return UnitsInRange(GetOtherFaction());
    }

    public bool UnitsInRange(UnitFaction f){
        var tiles = GridManager.instance.GetRadiusTiles(this.occupiedTile, maxMoveAmount);
        foreach (BaseTile t in tiles){
            if (t.occupiedUnit != null && t.occupiedUnit.faction == f){
                return true;
            }
        }
        return false;
    }

    public bool UnitInRange(BaseUnit unitToFind){
        var tiles = GridManager.instance.GetRadiusTiles(this.occupiedTile, maxMoveAmount);
        foreach (BaseTile t in tiles){
            if (t.occupiedUnit != null && t.occupiedUnit == unitToFind){
                return true;
            }
        }
        return false;
    }

    public void ReverseBuffs()
    {
        Debug.Log("reversed buffs");
    }

    internal void GiveBuff(Buff buff)
    {
        
    }

    public void DecrementBuffs(){
        Debug.Log(buffs.Count);
        foreach (var buff in buffs){
            Debug.Log(buff.buffType);
            if (buff.buffType == BuffType.OnTurn){
                buff.ApplyEffect();
            }
            buff.ReduceCooldown();
        }
        buffs.RemoveAll(b => b.CooldownOver());
    }

    internal void AddBuff(Buff buff)
    {
        buffs.Add(buff);
        if (buff.buffType == BuffType.OnApply){
            buff.ApplyEffect();
        }
    }
    internal void RemoveBuff(Buff buff)
    {
        buffs.Remove(buff);
    }

    internal void ReduceNextMovemnt(int amount)
    {
        Debug.Log("reduced movement: " + amount);
        reducedMovment = amount;
    }
}


public enum UnitStatType {
    Health,
    Attack,
    Defense,
    Agility,
    Attunment,
    Foresight,
    Luck,
}

public enum UnitClass {
    None,
    Infantry,
    Knight,
    Assassin,
    Mage,
    Sage,
    Cavalry,
    Paladin
}

public enum ArmorType {
    Light,
    Medium,
    Heavy
}