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
    public AudioSource musicSource;
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
        }else{
            musicSource.clip = castleBGM;
        }
        musicSource.Play();
    }
}
