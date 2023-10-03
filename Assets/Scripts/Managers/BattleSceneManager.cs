using System;
using System.Collections;
using System.Collections.Generic;
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
    void Awake()
    {
        instance = this;
    }

    void FixedUpdate(){
        leftBU.transform.position = Vector3.Lerp(leftBU.transform.position, leftNewPos, hitMoveSpeed * Time.deltaTime);
        rightBU.transform.position = Vector3.Lerp(rightBU.transform.position, rightNewPos, hitMoveSpeed * Time.deltaTime);
    }

    //TODO: Find better name
    public void StartBattleOld(){
        MenuManager.instance.menuState = MenuState.Battle;
        leftStartPos = leftBU.transform.position;
        leftNewPos = leftStartPos;
        rightStartPos = rightBU.transform.position;
        rightNewPos = rightStartPos;
    }

    public void StartBattle(BaseUnit first, BaseUnit second){
        MenuManager.instance.menuState = MenuState.Battle;
        leftBU.gameObject.SetActive(true);
        rightBU.gameObject.SetActive(true);
        leftBU.SetUnit(first);
        rightBU.SetUnit(second);
        leftBU.Attack();
        //StartCoroutine();
    }

    public void DisplayUnits(){

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
            HitRecoil(damaged, 0.25f);
        }
        else if (newHealth <= 0){
            //fatal blow
            HitRecoil(damaged, 4f);
        }
        else if (newHealth < health){
            if (newHealth*2 < health){
                //Big Hit
                HitRecoil(damaged, 2f);
            }else{
                //Normal hit
                HitRecoil(damaged, 1f);
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
    public void OnEnd(BattleUnit hitter){
        var damaged = leftBU;
        if (hitter == leftBU){
            damaged = rightBU;
        }
        if (damaged.assignedUnit.health <= 0){
            StartCoroutine(EndAnimationDeath(damaged, 1.5f));
            return;
        }
        StartCoroutine(EndAnaimtionWait(1.5f));
    }

    IEnumerator EndAnimationDeath(BattleUnit killed, float v){
        //TODO: ACTUAL KILL ANIMATION
        killed.Hide();        
        yield return new WaitForSeconds(v);
        Reset();
    }   

    IEnumerator EndAnaimtionWait(float secs){
        yield return new WaitForSeconds(secs);
        Reset();
    }
    private void Reset(){
        leftNewPos = leftStartPos;
        rightNewPos = rightStartPos;
        leftBU.Hide();
        rightBU.Hide();
        battleMenu.gameObject.SetActive(true);
        battleMenu.SetRandomEnemy();
    }
}
