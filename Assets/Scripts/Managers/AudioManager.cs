using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    [Header("UI Sounds")]
    [SerializeField]
    private AudioClip confirmUISound;
        [SerializeField]
    private AudioClip moveUISound; 
    [SerializeField]

    private AudioClip cancelUISound;
    [SerializeField]
    private AudioClip selectUISound;
    [SerializeField]
    private AudioClip blockUISound;

    [SerializeField]
    private AudioClip uiEquip, uiUnequip, uiUpgradeSkill, uiBuyParagon, uiOpenPause, uiOpenInventory;
    [Header("Board Sounds")]
    [SerializeField]
    private AudioClip boardConfirm;
    [SerializeField]
    private AudioClip boardMove, boardSelect, boardUnselect, boardUnitMove, runWin, levelWin, runLose;
    [SerializeField]
    private AudioClip recieveGold, chestOpen, chestItemGet;
    [Header("Battle Sounds")]
    [SerializeField]
    private AudioClip meleeHit;
    [SerializeField]
    private AudioClip meleeCrit, rangedHit, rangedCrit, magicHit, magicCrit, block, battleIntro, battleDeath, battleLevelUp;
    [Header("Active Skills Sounds")]
    [SerializeField]
    private AudioClip skillMove;
    [SerializeField]
    private AudioClip skillActivate, skillAttack, skillHeal, skillBuff, skillDebuff;
    [Header("Overworld Map Sounds")]
    [SerializeField]
    private AudioClip mapOpen;
    [SerializeField]
    private AudioClip mapClose, mapCursor, mapSelectStage, mapMoveToStage;
    [SerializeField]
    private AudioSource source;
    [Range(0.0f, 1.0f)]
    public float audioVolume = 1f;
    void Awake()
    {
//        Debug.Log("Audio awake");
        // if (instance != null){
        //     Destroy(instance);
        //     instance = this;
        // }
        instance = this;
    }
    private void PlaySound(AudioClip clip){
        PlaySound(clip, 0.75f * audioVolume);
    }
    private void PlaySound(AudioClip clip, float pitch){
        source.pitch = 1.0f;
        source.PlayOneShot(clip, pitch);
    }
    public void PlayConfirm(){
        PlaySound(confirmUISound, 0.55f * audioVolume);
    }
    public void PlaySelect(){
        PlaySound(selectUISound);
    }

    public void PlayMove(){
        PlaySound(moveUISound);
    }

    internal void PlayCancel(){
        PlaySound(cancelUISound);
    }

    internal void PlayMoveUI(){
        PlaySound(moveUISound);
    }
    internal void PlayBlock(){
        PlaySound(blockUISound);
    }
    public IEnumerator PlayTileSound(BaseUnit unit, BaseTile tile){
//        Debug.Log(tile.editorType);
        switch (tile.editorType){
            case TileEditorType.Grass:
                yield return PlayBoardMove(unit);
                break;
            case TileEditorType.Forest:
                yield return PlayBoardMove(unit);
                break;
            case TileEditorType.Bridge:
                yield return PlayBoardMove(unit);
                break;
            case TileEditorType.Water:
                yield return PlayBoardMove(unit);
                break;
        }
        yield return null;
    }
    internal IEnumerator PlayBoardMove(BaseUnit unit){
  //      Debug.Log("Bridge sound played");
        yield return unit.PlayRandomPitchSound(boardMove, 0.5f * audioVolume);
        //source.clip = grassClips[randomIndex];
    }

    internal void PlayMelee()
    {
    //    Debug.Log("Melee sound played");
        source.pitch = UnityEngine.Random.Range(0.7f, 1.3f);
        source.PlayOneShot(meleeHit, 0.75f * audioVolume);
    }

    internal void PlayArcher()
    {
      //  Debug.Log("Arrow sound played");
        source.pitch = UnityEngine.Random.Range(0.7f, 1.3f);
        source.PlayOneShot(rangedHit, 0.75f * audioVolume);
    }

    internal void PlayMagic()
    {
        // Debug.Log("Magic sound played");
        source.pitch = UnityEngine.Random.Range(0.7f, 1.3f);
        source.PlayOneShot(magicHit, 0.75f * audioVolume);
    }


}
