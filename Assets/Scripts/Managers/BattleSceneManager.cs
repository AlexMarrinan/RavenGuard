using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public class BattleSceneManager : MonoBehaviour
{
    public static BattleSceneManager instance;
    public BattleMenu battleMenu;
    public BattleUnit leftBU;
    public BattleUnit rightBU;
    private Vector3 leftStartPos;
    private Vector3 rightStartPos;
    private Vector3 leftNewPos;
    private Vector3 rightNewPos;
    public float hitMoveSpeed = 10f;
    private BattleUnit attacker;
    private BaseUnit startingUnit;
    public CombatOrder combatOrder;
    public GameObject sceneBackground;
    [HideInInspector] public BattleSceneState state = BattleSceneState.FirstAttack;
    void Awake()
    {
        instance = this;
    }

    void FixedUpdate(){
        rightBU.animator.transform.position = Vector3.Lerp(rightBU.transform.position, rightNewPos, hitMoveSpeed * Time.deltaTime);
        leftBU.animator.transform.position = Vector3.Lerp(leftBU.transform.position, leftNewPos, hitMoveSpeed * Time.deltaTime);
        // leftBU.transform.position = new Vector3(0,0,0);
        // rightBU.transform.position = new Vector3(0,0,0);
    }

    // //TODO: Find better name
    // public void StartBattleOld(){
    //     Debug.Log("started battle old!");
    //     MenuManager.instance.menuState = MenuState.Battle;
    //     leftStartPos = leftBU.transform.position;
    //     leftNewPos = leftStartPos;
    //     rightStartPos = rightBU.transform.position;
    //     rightNewPos = rightStartPos;
    // }

    public void StartBattle(BaseUnit first, BaseUnit second){
        MenuManager.instance.menuState = MenuState.Battle;
        MenuManager.instance.unitStatsMenu.gameObject.SetActive(false);
        MenuManager.instance.highlightObject.SetActive(false);
        MenuManager.instance.selectedObject.SetActive(false);

        UnitManager.instance.ShowUnitHealthbars(false);

        sceneBackground.SetActive(true);
        leftBU.gameObject.SetActive(true);
        rightBU.gameObject.SetActive(true);

        leftStartPos = leftBU.parentTrans.position;
        leftNewPos = leftStartPos;
        leftBU.transform.position = leftStartPos;

        rightStartPos = rightBU.parentTrans.position;
        rightNewPos = rightStartPos;
        rightBU.transform.position = rightStartPos;
        
        leftBU.SetUnit(first);
        startingUnit = first;
        rightBU.SetUnit(second);
        leftBU.Attack();
        state = BattleSceneState.FirstAttack;
    }
    public void SetAttacker(BattleUnit unit){
        attacker = unit;
    }
    public void DisplayUnits(){

    }
    public bool UnitAttacked(BaseUnit unit){
        if (unit == leftBU.assignedUnit){
            return leftBU.attacked;
        }
        if (unit == rightBU.assignedUnit){
            return rightBU.attacked;
        }
        return false;
    }
    public void OnHit(BattleUnit hitter){
        var damaged = leftBU;
        if (hitter == leftBU){
            damaged = rightBU;
        }
        int health = damaged.assignedUnit.health;
        damaged.assignedUnit.ReceiveDamage(hitter.assignedUnit);
        int newHealth = damaged.assignedUnit.health;
        if (newHealth == health){
            //No damage done
            HitRecoil(damaged, 0.5f);
        }
        else if (newHealth <= 0){
            //fatal blow
            HitRecoil(damaged, 4f);
            damaged.spriteRenderer.color = new Color(1f, 0.15f, 0.15f);
        }
        else if (newHealth < health){
            if (newHealth*1.5 < health){
                //Big Hit
                HitRecoil(damaged, 2f);
                StartCoroutine(HitColor(damaged, 2.5f));
            }else{
                //Normal hit
                HitRecoil(damaged, 1f);
                StartCoroutine(HitColor(damaged, 1.25f));
            }
        }
    }
    private void HitRecoil(BattleUnit damaged, float amount){
        if (damaged.faceDirection == FaceDirection.Left){
            rightNewPos = damaged.transform.position + new Vector3(amount, 0, 0);
        }else{
            leftNewPos = damaged.transform.position + new Vector3(amount*-1, 0, 0);
        }
    }

    IEnumerator HitColor(BattleUnit unit, float damage)
    {
        unit.spriteRenderer.color = new Color(1f, 0.65f, 0.65f);
        yield return new WaitForSeconds(damage * 0.1f);
        unit.spriteRenderer.color = Color.white;
    }

    public void OnEnd(BattleUnit hitter){
        var damaged = leftBU;
        if (hitter == leftBU){
            damaged = rightBU;
        }
        if (damaged.assignedUnit.health <= 0){
            StartCoroutine(EndAnimationDeath(damaged, 1.5f));
            return;
        }
        StartCoroutine(NextAttack(hitter, damaged));
    }
    IEnumerator NextAttack(BattleUnit hitter, BattleUnit damaged){
        ResetBattleUnitsPos();
        yield return new WaitForSeconds(0.4f);
        if (state == BattleSceneState.FirstAttack && ((hitter.assignedUnit is RangedUnit && damaged.assignedUnit is RangedUnit) || 
            (hitter.assignedUnit is MeleeUnit && damaged.assignedUnit is MeleeUnit))){
                state = BattleSceneState.CounterAttack;
                damaged.Attack();
                yield return null;
        }
        else if (state == BattleSceneState.FirstAttack && (hitter.assignedUnit.GetAgility().total >= damaged.assignedUnit.GetAgility().total + 5)) {
            state = BattleSceneState.SecondAttack;
            hitter.Attack();
            yield return null;
        }
        else if (state == BattleSceneState.CounterAttack) {
            if (damaged.assignedUnit.GetAgility().total >= hitter.assignedUnit.GetAgility().total + 5) {
                state = BattleSceneState.SecondAttack;
                damaged.Attack();
                yield return null;
            }else if (hitter.assignedUnit.GetAgility().total >= damaged.assignedUnit.GetAgility().total + 5) {
                state = BattleSceneState.SecondAttack;
                hitter.Attack();
                yield return null;
            }else{
                yield return EndAnaimtionWait(1.0f);
            }
        }else {
            yield return EndAnaimtionWait(1.5f);
        }
    }
    IEnumerator EndAnimationDeath(BattleUnit killed, float v){
        //TODO: ACTUAL KILL ANIMATION
        killed.Hide();        
        yield return new WaitForSeconds(v);
        UnitManager.instance.DeleteUnit(killed.assignedUnit);
        OnBattlEnd();
    }   
    
    IEnumerator EndAnaimtionWait(float secs){
        yield return new WaitForSeconds(secs);
        OnBattlEnd();
    }
    private void OnBattlEnd(){
        MenuManager.instance.menuState = MenuState.None;
        if (startingUnit != null){
            //startingUnit.moveAmount = 0;
            startingUnit.OnExhaustMovment();
        }else{
            TurnManager.instance.GoToNextUnit();
        }
        if (leftBU.assignedUnit != null){
            leftBU.assignedUnit.UsePassiveSkills(PassiveSkillType.AfterCombat);
        }
        if (rightBU.assignedUnit != null){
            rightBU.assignedUnit.UsePassiveSkills(PassiveSkillType.AfterCombat);
        }
        UnitManager.instance.ShowUnitHealthbars(true);
        ResetBattleUnitsPos();
        leftBU.spriteRenderer.color = Color.white;
        rightBU.spriteRenderer.color = Color.white;
        state = BattleSceneState.FirstAttack;        
        leftBU.Hide();
        rightBU.Hide();
        sceneBackground.SetActive(false);
        UnitManager.instance.UnselectUnit();
        MenuManager.instance.highlightObject.SetActive(true);
        //battleMenu.gameObject.SetActive(true);
        //battleMenu.SetRandomEnemy();
    }
    private void ResetBattleUnitsPos(){
        leftNewPos = leftStartPos;
        rightNewPos = rightStartPos;
    }
}

public enum BattleSceneState{
    FirstAttack,
    CounterAttack,
    SecondAttack,
}