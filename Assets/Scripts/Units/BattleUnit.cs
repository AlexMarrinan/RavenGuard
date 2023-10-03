using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class BattleUnit : MonoBehaviour
{
    public BaseUnit assignedUnit;
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public FaceDirection faceDirection = FaceDirection.Right;
    public HealthBarMenu healthBar;

    public void Start(){
        if (faceDirection == FaceDirection.Left){
            this.transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        }
    }

    public void SetUnit(BaseUnit unit){
        assignedUnit = unit;
        healthBar.gameObject.SetActive(true);
        healthBar.SetUnit(unit);
        animator.runtimeAnimatorController = assignedUnit.attackAnim;
        animator.speed = 0.0f;
        //animator.StartPlayback();
        //animator.StopPlayback();
    }
    public void Attack(){
        animator.runtimeAnimatorController = assignedUnit.attackAnim;
        animator.speed = 1.0f;
    }
    public void OnHit(){
        BattleSceneManager.instance.OnHit(this);
    }

    public void OnEnd(){
        BattleSceneManager.instance.OnEnd(this);
    }

    public void Hide(){
        healthBar.gameObject.SetActive(false);
        this.gameObject.SetActive(false);
    }
}
