using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;
public class BaseUnit : MonoBehaviour
{
    public string unitName;
    public BaseTile occupiedTile;
    public UnitFaction faction;
    public UnitClass unitClass;
    /*** Unit Stats ***/
    public int moveAmount;
    public int maxMoveAmount;
    public int health;
    public int maxHealth;
    public int currentXP = 0;
    public int maxXP = 100;
    [SerializeField] 
    private int droppedXP = 100;
    public int level = 1;
    public bool hasMoved;
    [SerializeField] private int attack;
    [SerializeField] private int defense;
    [SerializeField] private int agility;
    [SerializeField] private int attunment;
    [SerializeField] private int foresight;
    [SerializeField] private int luck;
    [HideInInspector]
    public Dictionary<string, SkillStatChange> skillStatChanges = new();

    // [HideInInspector]
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
    [SerializeField]
    private AudioSource audioSource;
    public UnitDot uiDot;
    public int[] activeSkillCooldowns;
    public SkillProgressionGroup paragonSkillProgression = null;

    void Start(){
        InitUnit();
    }
    public void InitUnit(){
        //RandomizeUnitClass();
        attackEffect = AttackEffect.None;
        buffs = new();
        ResetCooldowns();    
        ResetCombatStats();
        InitializeFaction();
        CreateHealthbar();
        SetSkillMethods();
        InitializeUnitClass();
    }
    public void ReduceCooldown(){
        Debug.Log("reduced cooldowns");
        for (int i = 0; i < 3; i++){
            BaseSkill skill = skills[i];
            if (skill == null || skill is ActiveSkill){
                continue;
            }
            activeSkillCooldowns[i] -= 1;
        }
    }
    public void ResetCooldowns(){
        activeSkillCooldowns = new int[3] {0, 0, 0};
    }
    public void ResetCombatStats(){
        foreach (UnitStatType ust in Enum.GetValues(typeof(UnitStatType))){
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
            if (skill == null){
                continue;
            }
//            Debug.Log(skill.skillName);
            skill.SetMethod();
        }
    }
        public virtual void ApplyWeapon(){

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
        var prefab = Resources.Load("Prefabs/UI/OverworldHealthBar");
        var go = (GameObject)Instantiate(prefab);
        ((GameObject)go).transform.SetParent(GameManager.instance.mainCanvas.transform);
        healthBar = go.GetComponent<UnitHealthBar>();
        healthBar.SetAttachedUnit(this);
        healthBar.transform.SetAsFirstSibling();
    }
    public virtual void Attack(BaseUnit otherUnit){
        return;
    }
    public virtual void Heal(BaseUnit otherUnit){
        return;
    }
    protected virtual void InitializeUnitClass()
    {

    }
    public int GetDamage(BaseUnit otherUnit){
        int attackDmg = GetAttackDamage(otherUnit);
        int magicDmg = GetMagicDamage(otherUnit);
        bool attackHigher = attackDmg > magicDmg;

        WeaponClass hitterEffective = UnitManager.instance.strongAgainst[this.weaponClass];
        WeaponClass defenderEffective = UnitManager.instance.strongAgainst[otherUnit.weaponClass];

        if (attackEffect == AttackEffect.Duality){
            return attackHigher ? attackDmg : magicDmg;
        }
        if (attackEffect == AttackEffect.Confusion){
            return !attackHigher ? attackDmg : magicDmg;
        }
        if (weaponClass == WeaponClass.Magic){
            if (hitterEffective == otherUnit.weaponClass){
                Debug.Log("Effective hit!");
                return (int)(magicDmg * 1.2f);
            }else if (defenderEffective == this.weaponClass){
                Debug.Log("Weak hit!");
                return (int)(magicDmg * 0.8f);
            }
            return magicDmg;
        }
        float value = 1;
        if (BattleSceneManager.instance.prediction == null){
            if (hitterEffective == otherUnit.weaponClass){
                Debug.Log("Effective hit!");
                return (int)(attackDmg * 1.2f);
            }else if (defenderEffective == this.weaponClass){
                Debug.Log("Weak hit!");
                return (int)(attackDmg * 0.8f);
            }
            return attackDmg;
        }
        if (BattleSceneManager.instance.prediction.attacker == this){
            foreach (UnitStatMultiplier multi in BattleSceneManager.instance.prediction.attackerStatMultipliers){
                if (multi.statType == UnitStatType.Attack){
                    value *= multi.multiplier;
                }
            }
        }
        else if (BattleSceneManager.instance.prediction.defender == this){
            foreach (UnitStatMultiplier multi in BattleSceneManager.instance.prediction.defenderStatMultiplers){
                if (multi.statType == UnitStatType.Attack){
                    value *= multi.multiplier;
                }
            }
        }
        if (hitterEffective == otherUnit.weaponClass){
            Debug.Log("Effective hit!");
            return (int)(attack * 1.2f * value);
        }else if (defenderEffective == this.weaponClass){
            Debug.Log("Weak hit!");
            return (int)(attackDmg * 0.8f * value);
        }
        return (int)(attackDmg * value);
        //return (int)((float)attackDmg * value);
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
    public void ReciveNonlethalDamage(int damage){
        health -= damage;
        if (health <= 0){
            health = 1;
        }
    }
    public void RecoverHealth(int healing){
        health += healing;
        if (health > maxHealth){
            health = maxHealth;
        }
    }
    public void MoveToAttackTile(BaseUnit otherUnit){
        if (TurnManager.instance.currentFaction == UnitFaction.Enemy){
            return;
        }
        BaseTile lastTile = PathLine.instance.GetLastTile();
        // if (this is RangedUnit){
        //     int distance = otherUnit.occupiedTile.GetPathLengthFrom(occupiedTile);

        //     if (distance > maxMoveAmount){
        //         lastTile = PathLine.instance.GetPathTile(distance - maxMoveAmount);
        //     }
        // }
        //GameManager.instance.PanCamera(adjTile.transform.position);
        UnitManager.instance.RemoveAllValidMoves();
        if (lastTile != null){
            StartCoroutine(lastTile.MoveUnitAlongPath(UnitManager.instance.selectedUnit, false));
        }
        //healthBar.RenderHealth();
    }
    public void ResetMovment(){
        InitializeFaction();
        ResetUIDot();
        moveAmount = maxMoveAmount;
        hasMoved = false;
    }

    private void ResetUIDot()
    {
        if (uiDot == null){
            return;
        }
        if (faction == UnitFaction.Hero){
//            Debug.Log(uiDot);
            uiDot.SetColor(Color.cyan);
        }else{
            uiDot.SetColor(Color.red);
        }
    }

    public void FinishMovement(){
        // moveAmount = 0;
//        Debug.Log("movment over");
        hasMoved = true;
        UsePassiveSkills(PassiveSkillType.OnMovement);
        UnitManager.instance.UnselectUnit();
        //TODO ADD OTHER END CONDITIONS:
        //No active skills ready
        //No avaliable attacks
        PathLine.instance.Reset();
        if (!AfterMoveActions() || this.faction == UnitFaction.Enemy){
            FinishTurn();
        }else{
            GridManager.instance.SetHoveredTile(this.occupiedTile);
        }
    }
    public void FinishTurn(){
        // moveAmount = 0;
        spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f);
        
        if (uiDot != null){
            uiDot.SetColor(new Color(1.0f, 1.0f, 1.0f));
        }
//        Debug.Log("turn over");
        UnitManager.instance.SetSelectedUnit(null);
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

    public int GetBaseATK(){
        return attack;
    }
    public int GetBaseDEF(){
        return defense;
    }
    public int GetBaseAGL(){
        return agility;
    }
    public int GetBaseATU(){
        return attunment;
    }
    public int GetBaseFOR(){
        return foresight;
    }
    public int GetBaseLCK(){
        return luck;
    }

    public void SetBaseATK(int value){
        attack = value;
    }
    public void SetBaseDEF(int value){
        defense = value;
    }
    public void SetBaseAGL(int value){
        agility = value;
    }    
    public void SetBaseATU(int value){
        attunment = value;
    }    
    public void SetBaseFOR(int value){
        foresight = value;
    }    
    public void SetBaseLCK(int value){
        luck = value;
    }


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
        if (!duringCombatStats.ContainsKey(type)){
            return newAmount;
        }
        return newAmount + duringCombatStats[type];
    }
    public SkillStatChange GetStatChange(string name){
        if (skillStatChanges.ContainsKey(name)){
            return skillStatChanges[name];
        }
        return null;
    }
    public void AddStatsChange(string name, UnitStatType type, int startAmount, int minAmount, int maxAmount, int cooldown = -1){
        if (GetStatChange(name) == null){
            var newStats = new SkillStatChange(type, startAmount, minAmount, maxAmount, cooldown);
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
        // Debug.Log(buffs.Count);
        foreach (var buff in buffs){
            //Debug.Log(buff.buffType);
            if (buff.buffType == BuffType.OnTurn){
                buff.ApplyEffect();
            }
            buff.ReduceCooldown();
        }
        buffs.RemoveAll(b => b.CooldownOver());

        List<string> doneStatNames = new();

        foreach (string statName in skillStatChanges.Keys){
            var stat = skillStatChanges[statName];
            stat.cooldown -= 1;
            if (stat.cooldown == 0){
                doneStatNames.Add(statName);
            }
        }
        foreach (string statName in doneStatNames){
            skillStatChanges.Remove(statName);
        }
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

    internal void ReduceNextMovemnt(int amount){
        Debug.Log("reduced movement: " + amount);
        reducedMovment = amount;
    }

    public IEnumerator PlaySound(AudioClip audioClip, float volume){
        audioSource.PlayOneShot(audioClip, volume * AudioManager.instance.audioVolume);
        yield return null;
    }
    public IEnumerator PlayRandomPitchSound(AudioClip audioClip, float volume){
        audioSource.pitch = UnityEngine.Random.Range(0.6f, 1.4f);
        audioSource.PlayOneShot(audioClip, volume * AudioManager.instance.audioVolume);
        yield return null;
    }

    internal void HighlightDot()
    {
        if (uiDot == null){
            return;
        }
        UnitManager.instance.HighlightDot(this.uiDot);
    }
    public virtual List<(BaseTile, TileMoveType)> GetValidAttacks(BaseTile tempTile)
    {
        return new ();
    }
    public List<(BaseTile, TileMoveType)> GetValidAttacks()
    {
        return GetValidAttacks(this.occupiedTile);
    }
    public int NumValidAttacks(){
        return GetValidAttacks().Where(atk => atk.Item2 == TileMoveType.Attack).Count();
    }

    public bool AfterMoveActions(){
        if (NumValidAttacks() > 0){
            return true;
        }
        foreach (int i in activeSkillCooldowns){
            if (i <= 0){
                return true;
            }
        }
        return false;
    }

    protected void SetAttackMove(BaseTile tile, List<(BaseTile, TileMoveType)> returns)
    {
        if (tile.occupiedUnit != null && tile.occupiedUnit.faction != this.faction){
            returns.Add((tile, TileMoveType.Attack));
        }else if (tile.walkable || tile.occupiedUnit == null){
            returns.Add((tile, TileMoveType.InAttackRange));
        }
    }

    public void GainXP(int amount){
        if (this.faction == UnitFaction.Enemy){
            BattleSceneManager.instance.waitForXP = false;
            return;
        }
        int newXP = currentXP + amount;
        int levelsGained = 0;
        while (newXP >= maxXP){
            StartCoroutine(LevelUpBarAnimation());
            levelsGained++;
            newXP -= maxXP;
        }
        currentXP = newXP;
        StartCoroutine(AnimateXPBar(newXP));
        if (levelsGained == 0){
            BattleSceneManager.instance.CloseBattleScene();
        }
    }

    private IEnumerator AnimateXPBar(int xp)
    {
        yield return new WaitForSeconds(1f);
    }

    private IEnumerator LevelUpBarAnimation()
    {
        yield return new WaitForSeconds(0.2f);
        LevelUp();
    }

    private void LevelUp(){
//        Debug.Log(this.ToString() + " LEVEL UP!");
        level++;
        var menu = MenuManager.instance.levelupMenu;
        menu.Reset();
        menu.SetUnit(this);
        menu.gameObject.SetActive(true);
    }

    public int GetDroppedXP(){
        return droppedXP;
    }

    public bool CanUseSkill(BaseItem item){
        if (item == null || item is not BaseSkill){
            return false;
        }
        return CanUseSkill(item as BaseSkill);
    }
    public bool CanUseSkill(BaseSkill newSkill){
        if (newSkill == null || skills.Contains(newSkill)){
            return false;
        }
        if (newSkill.weaponClass != this.weaponClass && newSkill.weaponClass != WeaponClass.Any){
            return false;
        }
        if (newSkill.unitClass != this.unitClass && newSkill.unitClass != UnitClass.Any){
            return false;
        }
        return true;
    }

    public bool CanUseWeapon(BaseItem item){
        if (item == null || item is not BaseWeapon){
            return false;
        }
        return CanUseWeapon(item as BaseWeapon);
    }
    public bool CanUseWeapon(BaseWeapon newWeapon){
        if (newWeapon == null){
            return false;
        }        
        return newWeapon.weaponClass == this.weaponClass;
    }

    internal void PutSkillOnCooldown(ActiveSkill activeSkill)
    {
        for (int i = 0; i < 3; i++){
            if (skills[i] == activeSkill){
                activeSkillCooldowns[i] = activeSkill.cooldown;
                return;
            }
        }
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
    Any,
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