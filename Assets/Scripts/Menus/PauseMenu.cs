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
            case 1: SkipeTurn(); break;
            case 2: Quit(); break;
        }
    }
    private void Resume(){
        MenuManager.instance.TogglePauseMenu();
    }
    private void SkipeTurn(){
        MenuManager.instance.TogglePauseMenu();
        MenuManager.instance.ToggleLevelEndMenu();
        return;
        MenuManager.instance.TogglePauseMenu();
        if (GameManager.instance.gameState == GameState.EnemiesTurn){
            GameManager.instance.ChangeState(GameState.HeroesTurn);
        }else{
            GameManager.instance.ChangeState(GameState.EnemiesTurn);
        }
    }
    private void Quit(){
        Debug.Log("Quitting...");
        SceneManager.LoadScene("MainMenu");
    }
}
