using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BattleMenu : BaseMenu
{
     public List<BaseUnit> units;
    public UnitStatsMenu heroStats, enemyStats;
    private int enemyIndex = 0;
    public void Start(){
        for (int i = 0; i < units.Count; i++){
            var b = buttons[i];
            var u = units[i];
            b.image.sprite = u.spriteRenderer.sprite;
            u.ApplyWeapon();
        }
        SetHeroStats();
        SetRandomEnemy();
    }
    public override void Move(Vector2 direction) 
    {
        base.Move(direction);
        SetHeroStats();
    }
    public void SetRandomEnemy(){
        enemyIndex = Random.Range(1, 3);
        enemyIndex *= 2;
        enemyIndex -= 1;
        SetEnemyStats(units[enemyIndex]);
    }
    private void SetHeroStats(){
        heroStats.SetUnit(units[buttonIndex]);
    }
    private void SetEnemyStats(BaseUnit unit){
        enemyStats.SetUnit(unit);
    }

    public override void Select()
    {
        base.Select();
        BaseUnit hero = Object.Instantiate(units[buttonIndex]);
        hero.transform.position = new Vector3(-100, -100, 0);
        BaseUnit enemy = Object.Instantiate(units[enemyIndex]);
        enemy.faction = UnitFaction.Enemy;
        enemy.transform.position = new Vector3(-100, -100, 0);
        BattleSceneManager.instance.StartBattle(hero, enemy);
        this.gameObject.SetActive(false);
    }
}
