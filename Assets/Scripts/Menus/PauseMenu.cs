using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : BaseMenu
{

    public override void Select()
    {
        switch(buttonIndex){
            case 0: Resume(); break;
            case 1: HowToPlay(); break;
            case 2: Quit(); break;
        }
    }
    private void Resume(){
        MenuManager.instance.TogglePauseMenu();
    }
    private void HowToPlay(){
        // MenuManager.instance.TogglePauseMenu();
        // MenuManager.instance.ToggleLevelEndMenu();
        // return;
        MenuManager.instance.ToggleHowToPlayMenu();
    }
    private void Quit(){
        Debug.Log("Quitting...");
        SceneManager.LoadScene("Town");
        // SceneManager.LoadScene("MainMenu");
    }
}
