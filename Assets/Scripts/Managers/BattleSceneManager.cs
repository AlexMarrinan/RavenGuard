using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSceneManager : MonoBehaviour
{
    public static BattleSceneManager instance;
    void Awake()
    {
        instance = this;
    }
    public void StartBattle(){
        MenuManager.instance.menuState = MenuState.Battle;
    }
}
