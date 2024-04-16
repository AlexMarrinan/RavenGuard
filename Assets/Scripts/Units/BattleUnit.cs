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
    public Transform parentTrans;
    public AudioSource audioSource;
    public bool attacked = false;
    public int damageDealt;

    private ParticleSystem[] particleSystems;
    /* [1]: Normal Hit
     * [2]: Big Hit
     * [3]: Fatal Hit
     * [4]: Magic Cast
     * [5]: Bow Cast
     */

    private void Awake() { // Get Particle Systems
        particleSystems = GetComponentsInChildren<ParticleSystem>();
    }
    public void Start(){
        if (faceDirection == FaceDirection.Left){
            parentTrans.localScale = new Vector3(parentTrans.localScale.x * -1, parentTrans.localScale.y, parentTrans.localScale.z);
        }
    }

    public void SetUnit(BaseUnit unit){
        assignedUnit = unit;
        healthBar.gameObject.SetActive(true);
        healthBar.SetUnit(unit);
        if (assignedUnit.idleBattleSprite == null){
            spriteRenderer.sprite = assignedUnit.spriteRenderer.sprite;
        }else{
            spriteRenderer.sprite = assignedUnit.idleBattleSprite;
        }
        //animator.StartPlayback();
        //animator.StopPlayback();
    }
    public void Attack(){
        attacked = true;
        SetAnimator();
        animator.Rebind();
        animator.speed = 1.0f;

        if (assignedUnit.weapon.weaponClass == WeaponClass.Archer) {
            PlayCastParticles(7); // Bow particles
        }
        else if (assignedUnit is not MeleeUnit) {
            PlayCastParticles(8); // Magic particles
        }
    }
    private void SetAnimator(){
        animator.runtimeAnimatorController = assignedUnit.animatorController;
    }
    public void OnCast(){
        BattleSceneManager.instance.OnCast(this);
    }
    public void OnHit(){
        BattleSceneManager.instance.OnHit(this);
    }

    public void OnEnd(){
        spriteRenderer.sprite = assignedUnit.spriteRenderer.sprite;
        animator.runtimeAnimatorController = null;
        BattleSceneManager.instance.OnEnd(this);
    }

    public void Hide(){
        attacked = false;
        healthBar.gameObject.SetActive(false);
        this.gameObject.SetActive(false);
    }

    // Plays hit particle system of given type (0 = Normal, 1 = Big, 2 = Fatal)
    public void PlayHitParticles(int type) { // An enum would be a better parameter here
        ParticleSystem hitParticles = particleSystems[type];

        if (faceDirection == FaceDirection.Left) { // BUG: Sometimes this if statement is skipped. FaceDirection issue.
            ParticleSystem.VelocityOverLifetimeModule velocityModule = hitParticles.velocityOverLifetime;
            ParticleSystem.MinMaxCurve velocityCurve = velocityModule.x;
            velocityCurve.constant *= -1; // Reverse velocity direction
            velocityModule.x = velocityCurve;
        }

        hitParticles.Play();
    }

    public void PlayDeathParticles() {
        particleSystems[6].Play();
    }

    public void StopDeathParticles() {
        particleSystems[6].Stop();
    }

    // 
    public void PlayCastParticles(int type) {
        ParticleSystem castParticles = particleSystems[type];

        if (faceDirection == FaceDirection.Left) { // BUG: Sometimes this if statement is skipped. FaceDirection issue.
            Vector3 xPos = castParticles.transform.position;
            xPos.x *= -1; // Reverse direction
            castParticles.transform.position = xPos;
        }

        castParticles.Play();
    }

    // Stop all cast particles
    public void StopCastParticles() {
        particleSystems[4].Stop();
        particleSystems[5].Stop();
    }

}
