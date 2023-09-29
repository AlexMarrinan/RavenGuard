using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BattleMenu : BaseMenu
{
     public List<BaseUnit> units;
    public UnitStatsMenu heroStats, enemyStats;
    public void Start(){
        for (int i = 0; i < units.Count; i++){
            var b = buttons[i];
            var u = units[i];
            b.image.sprite = u.spriteRenderer.sprite;
            u.ApplyWeapon();
            Debug.Log(units[i].weapon);
        }
        SetHeroStats();
        int enemyIndex = Random.Range(0, 4);
        SetEnemyStats(units[enemyIndex]);
    }
    public override void Move(Vector2 direction)
    {
        base.Move(direction);
        SetHeroStats();
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
        StartBattle();
    }
    private void StartBattle(){

    }
}
