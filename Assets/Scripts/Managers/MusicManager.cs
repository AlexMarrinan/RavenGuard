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
    public AudioSource musicSource, musicSourceBattle;
    [Range(0.0f, 1.0f)]
    public float musicVolume = 1f;
    void Awake()
    {
        instance = this;
    }
    public void StartMusic(LevelData levelData){
        LevelTheme levelTheme = levelData.levelTheme;
        if (levelTheme == LevelTheme.MountainRange){
            musicSource.clip = mountRangeBGM;
            musicSourceBattle.clip = mountRangeBattleBGM;
        }else{
            musicSource.clip = castleBGM;
            musicSourceBattle.clip = castleBattleBGM;
        }
        musicSource.volume = musicVolume;
        musicSource.Play();

        musicSourceBattle.volume = 0f;
        musicSourceBattle.Play();
    }

    public void StartBattle(){
        StartCoroutine(FadeTracks(musicSourceBattle, musicSource));
        // musicSource.volume = 0f;
        // musicSourceBattle.volume = musicVolume;
    }

    public void StopBattle(){
        StartCoroutine(FadeTracks(musicSource, musicSourceBattle));
        // musicSource.volume = musicVolume;
        // musicSourceBattle.volume = 0f;
    }

    public IEnumerator FadeTracks(AudioSource fadeIn, AudioSource fadeOut){

        float duration = 0.08f;
        float currentTime = 0;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            fadeIn.volume = Mathf.Lerp(0f, musicVolume, currentTime / duration);
            fadeOut.volume = Mathf.Lerp(musicVolume, 0f, currentTime / duration);
            yield return null;
        }
    }
}
