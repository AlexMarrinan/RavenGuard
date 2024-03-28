using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OverworldMap : BaseMenu
{

    // Selection options
    public override void Select()
    {
        // Up to three node options can be available at once
        switch(buttonIndex){
            case 0: Node1(); break;
            case 1: Node2(); break;
            case 2: Node3(); break;
       }
    }
    private void Node1(){
        SceneManager.LoadScene("Level 1");
        // TODO: Select random level

        // Progressing through nodes
    }
    private void Node2(){
        SceneManager.LoadScene("Level 2");
        // TODO: Select random level
    }
    private void Node3(){
        SceneManager.LoadScene("Level 3");
        // TODO: Select random level
    }
}
