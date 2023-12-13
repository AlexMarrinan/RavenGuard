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
    private AudioClip confirmSound, moveSound, moveUISound, cancelSound, selectSound;
    [SerializeField]
    private List<AudioClip> grassClips, bridgeClips; 
    [SerializeField]
    private AudioSource source;
    void Awake()
    {
        Debug.Log("Audio awake");
        instance = this;
    }

    public void PlayConfirm(){
        source.PlayOneShot(confirmSound, 0.75f);
    }
    public void PlaySelect(){
        source.PlayOneShot(selectSound, 0.75f);
    }

    public void PlayMove(){
        source.PlayOneShot(moveSound, 0.75f);
    }

    internal void PlayCancel(){
        source.PlayOneShot(cancelSound, 0.75f);
    }

    internal void PlayMoveUI(){
        source.PlayOneShot(moveUISound, 0.75f);
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
        yield return unit.PlaySound(grassClips[randomIndex], 0.2f);
        //source.clip = grassClips[randomIndex];
    }
    internal IEnumerator PlayBridge(BaseUnit unit){
        Debug.Log("Bridge sound played");
        int randomIndex = UnityEngine.Random.Range(0, bridgeClips.Count);
        yield return unit.PlaySound(bridgeClips[randomIndex], 0.5f);
        //source.clip = grassClips[randomIndex];
    }
}
