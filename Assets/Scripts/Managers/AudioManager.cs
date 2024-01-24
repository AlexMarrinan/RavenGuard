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
    [SerializeField]
    private AudioClip confirmSound, moveSound, moveUISound, cancelSound, selectSound, blockSound;
    [SerializeField]
    private List<AudioClip> grassClips, bridgeClips, meleeClips, arrowClips, magicClips; 
    [SerializeField]
    private AudioSource source;
    [Range(0.0f, 1.0f)]
    public float audioVolume = 1f;
    void Awake()
    {
//        Debug.Log("Audio awake");
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
        PlaySound(confirmSound, 0.55f * audioVolume);
    }
    public void PlaySelect(){
        PlaySound(selectSound);
    }

    public void PlayMove(){
        PlaySound(moveSound);
    }

    internal void PlayCancel(){
        PlaySound(cancelSound);
    }

    internal void PlayMoveUI(){
        PlaySound(moveUISound);
    }
    internal void PlayBlock(){
        PlaySound(blockSound);
    }
    public IEnumerator PlayTileSound(BaseUnit unit, BaseTile tile){
        Debug.Log(tile.editorType);
        switch (tile.editorType){
            case TileEditorType.Grass:
                yield return PlayGrass(unit);
                break;
            case TileEditorType.Forest:
                yield return PlayGrass(unit);
                break;
            case TileEditorType.Bridge:
                yield return PlayBridge(unit);
                break;
            case TileEditorType.Water:
                yield return PlayBridge(unit);
                break;
        }
        yield return null;
    }
    internal IEnumerator PlayGrass(BaseUnit unit){
        Debug.Log("Grass sound played");
        int randomIndex = UnityEngine.Random.Range(0, grassClips.Count);
        yield return unit.PlayRandomPitchSound(grassClips[randomIndex], 0.2f);
        //source.clip = grassClips[randomIndex];
    }
    internal IEnumerator PlayBridge(BaseUnit unit){
        Debug.Log("Bridge sound played");
        int randomIndex = UnityEngine.Random.Range(0, bridgeClips.Count);
        yield return unit.PlayRandomPitchSound(bridgeClips[randomIndex], 0.5f);
        //source.clip = grassClips[randomIndex];
    }

    internal void PlayMelee()
    {
        Debug.Log("Melee sound played");
        source.pitch = UnityEngine.Random.Range(0.7f, 1.3f);
        source.PlayOneShot(meleeClips[UnityEngine.Random.Range(0, meleeClips.Count)], 0.75f * audioVolume);
    }

    internal void PlayArcher()
    {
        Debug.Log("Arrow sound played");
        source.pitch = UnityEngine.Random.Range(0.7f, 1.3f);
        source.PlayOneShot(arrowClips[UnityEngine.Random.Range(0, arrowClips.Count)], 0.75f * audioVolume);
    }

    internal void PlayMagic()
    {
         Debug.Log("Magic sound played");
        source.pitch = UnityEngine.Random.Range(0.7f, 1.3f);
        source.PlayOneShot(magicClips[UnityEngine.Random.Range(0, magicClips.Count)], 0.75f * audioVolume);
    }


}
