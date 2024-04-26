using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;
    [Header("Town Music")]
    [SerializeField]
    private AudioClip townBGM;
    [SerializeField]
    private AudioClip mapBGM;
    [Header("Mountain Range Music")]
    [SerializeField]
    private AudioClip mountRangeBGM; 
    [SerializeField]
    private AudioClip mountRangeBattleBGM; 
    [Header("Castle Music")]
    [SerializeField]
    private AudioClip castleBGM; 
    [SerializeField]
    private AudioClip castleBattleBGM;
    public AudioSource musicSource, musicSourceBattle, musicSourceMap;

    private AudioSource currentSource;
    [Range(0.0f, 1.0f)]
    public float musicVolume = 1f;
    void Awake()
    {
        instance = this;
    }
    public void SetUpMusic(){  
        musicSource.clip = mountRangeBGM;
        musicSource.volume = 0f;
        musicSource.Play();

        musicSource.clip = mountRangeBattleBGM;
        musicSourceBattle.volume = 0f;
        musicSourceBattle.Play();

        musicSourceMap.clip = mapBGM;
        musicSourceMap.volume = 0f;
        musicSourceMap.Play();
    }

    public void StartLevelMusic(LevelData levelData){
        Debug.Log("Starting level music");
        LevelTheme levelTheme = levelData.levelTheme;
        if (levelTheme == LevelTheme.MountainRange){
            musicSource.clip = mountRangeBGM;
            musicSourceBattle.clip = mountRangeBattleBGM;
        }else{
            musicSource.clip = castleBGM;
            musicSourceBattle.clip = castleBattleBGM;
        }
        musicSource.Play();
        musicSourceBattle.Play();
        // Debug.Log(levelTheme);
        StartCoroutine(FadeTracks(musicSource, 0.5f));
    }
    public void StartBattleMusic(){
        StartCoroutine(FadeTracks(musicSourceBattle));
    }
    public void StopBattleMusic(){
        StartCoroutine(FadeTracks(musicSource));
    }
    public void StartMapMusic(){
        StartCoroutine(FadeTracks(musicSourceMap, 0.5f));
    }
    public IEnumerator FadeTracks(AudioSource fadeIn, float duration = 0.08f){
        Debug.Log("Tracks fading...");
        float currentTime = 0;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            fadeIn.volume = Mathf.Lerp(0f, musicVolume, currentTime / duration);
            if (currentSource != null){
                currentSource.volume = Mathf.Lerp(musicVolume, 0f, currentTime / duration);
            }
            yield return null;
        }
        currentSource = fadeIn;
        yield return null;
    }
}
