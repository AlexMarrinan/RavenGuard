using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseMenu : MonoBehaviour
{
    public List<MenuButton> buttons;
    public int buttonIndex = 0;
    public MenuDirection menuDirection;
    public Image highlighImage;
    public virtual void Move(Vector2 direction){
        if (menuDirection == MenuDirection.Vertical){
            buttonIndex += (int)direction.y;
        }else if (menuDirection == MenuDirection.Horizontal){
            buttonIndex += (int)direction.x;
        }

        if (buttonIndex < 0){
            buttonIndex = buttons.Count -1;
        }else if (buttonIndex >= buttons.Count){
            buttonIndex = 0;
        }
        SetHighlight();
    }
    protected MenuButton GetCurrentButton(){
        return buttons[buttonIndex];
    }
    public virtual void Reset(){
        buttonIndex = 0;
        SetHighlight();
    }
    protected void SetHighlight(){
        highlighImage.transform.position = GetCurrentButton().transform.position;
    }
    public virtual void Select(){

    }
}

public enum MenuDirection {
    Vertical,
    Horizontal
}
