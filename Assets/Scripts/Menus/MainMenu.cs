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
            case 1: Quit(); break;
       }
    }
    private void Play(){
        SceneManager.LoadScene("StartScene");
    }
    private void Options(){
        //TODO: OPTIONS MENU
    }
    private void Quit(){
        Debug.Log("Quitting...");
        Application.Quit();
    }
}
