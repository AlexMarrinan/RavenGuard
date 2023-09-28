using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : BaseMenu
{

    public override void Select()
    {
        switch(buttonIndex){
            case 0: Play(); break;
            case 1: Options(); break;
            case 2: Quit(); break;
        }
    }
    private void Play(){
        SceneManager.LoadScene("SampleScene");
    }
    private void Options(){
        SceneManager.LoadScene("BattleScene");
    }
    private void Quit(){
        Debug.Log("Quitting...");
        Application.Quit();
    }
}
