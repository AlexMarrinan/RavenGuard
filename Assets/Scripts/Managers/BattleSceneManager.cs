using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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
    private BaseUnit startingUnit;
    public GameObject sceneBackground;
    [HideInInspector] public BattleSceneState state = BattleSceneState.FirstAttack;
    private BattlePrediction prediction;
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

    public void StartBattle(BaseUnit first, BaseUnit second){
        MenuManager.instance.menuState = MenuState.Battle;
        MenuManager.instance.unitStatsMenu.gameObject.SetActive(false);
        MenuManager.instance.otherUnitStatsMenu.gameObject.SetActive(false);

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
        rightBU.SetUnit(second);
        prediction = new BattlePrediction(first, second);
//        Debug.Log("Prediction counter: " + prediction.defenderCounterAttack);
//        Debug.Log("Prediction atk followup: " + prediction.attackerSecondAttack);
//        Debug.Log("Prediction def followup: " + prediction.defenderSecondAttack);
//        Debug.Log("Prediction swapped: " + prediction.swappedAttackers);
//        Debug.Log("Prediction Attacker: " + prediction.attacker.unitClass);
        if (leftBU.assignedUnit == prediction.attacker){
//            Debug.Log("Left starting...");
            leftBU.Attack();
            startingUnit = first;
        }else{
 //           Debug.Log("Right starting...");
            rightBU.Attack();
            startingUnit = second;
        }
        state = BattleSceneState.FirstAttack;
    }
    public void DisplayUnits(){
        
    }
    public BattleUnit GetBattleUnit(BaseUnit unit){
        if (leftBU.assignedUnit == unit){
            return leftBU;
        }
        if (rightBU.assignedUnit == unit){
            return rightBU;
        }
        return null;
    }
    public BattleUnit GetOtherBattleUnit(BaseUnit unit){
        if (leftBU.assignedUnit == unit){
            return rightBU;
        }
        return leftBU;
    }
    public bool UnitAttacked(BaseUnit unit){
        BattleUnit bu = GetBattleUnit(unit);
        return bu != null && bu.attacked;
    }

    public void OnHit(BattleUnit hitter){
        var damaged = leftBU;
        if (hitter == leftBU){
            damaged = rightBU;
        }
        int health = damaged.assignedUnit.health;
        damaged.assignedUnit.ReceiveDamage(hitter.assignedUnit);
        int newHealth = damaged.assignedUnit.health;
        hitter.damageDealt += health - newHealth;
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
        if (state == BattleSceneState.FirstAttack && prediction.defenderCounterAttack){
            Debug.Log("counter attack!");
            state = BattleSceneState.CounterAttack;
            damaged.Attack();
            yield return null;
        }
        else if (state == BattleSceneState.FirstAttack && prediction.attackerSecondAttack){
            Debug.Log("attacker second attack!");
            state = BattleSceneState.SecondAttack;
            hitter.Attack();
            yield return null;
        }
        else if (state == BattleSceneState.CounterAttack) {
            if (prediction.attackerSecondAttack) {
                Debug.Log("attacker second attack!");
                state = BattleSceneState.SecondAttack;
                damaged.Attack();
                yield return null;
            }else if (prediction.defenderSecondAttack) {
                Debug.Log("defender second attack!");
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
        OnBattlEnd();
    }   
    
    IEnumerator EndAnaimtionWait(float secs){
        yield return new WaitForSeconds(secs);
        OnBattlEnd();
    }
    private void OnBattlEnd(){
        leftBU.assignedUnit.tempStatChanges = null;
        rightBU.assignedUnit.tempStatChanges = null;

        MenuManager.instance.menuState = MenuState.None;
        if (startingUnit.faction == TurnManager.instance.currentFaction){
            //startingUnit.moveAmount = 0;
            startingUnit.FinishTurn();
        }else if (GetOtherBattleUnit(startingUnit).assignedUnit.faction  == TurnManager.instance.currentFaction){
            GetOtherBattleUnit(startingUnit).assignedUnit.FinishTurn();
        }else{
            TurnManager.instance.GoToNextUnit();
        }
        if (leftBU.assignedUnit.health > 0){
            leftBU.assignedUnit.UsePassiveSkills(PassiveSkillType.AfterCombat);
        }
        if (rightBU.assignedUnit.health > 0){
            rightBU.assignedUnit.UsePassiveSkills(PassiveSkillType.AfterCombat);
        }
        if (leftBU.assignedUnit.health <= 0){
            UnitManager.instance.DeleteUnit(leftBU.assignedUnit);
        }
        if (rightBU.assignedUnit.health <= 0){
            UnitManager.instance.DeleteUnit(rightBU.assignedUnit);
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