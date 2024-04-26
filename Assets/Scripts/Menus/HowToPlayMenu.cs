using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HowToPlayMenu : BaseMenu
{
    public List<Sprite> sprites;
    public Image image;
    public override void Select()
    {
        MenuManager.instance.toggleHTPMenu();
    }
    public override void Move(Vector2 direction){
        base.Move(direction);
        image.sprite = sprites[buttonIndex];
    }

    public void Reset(){
        image.sprite = sprites[0];
        buttonIndex = 0;
    }
}
