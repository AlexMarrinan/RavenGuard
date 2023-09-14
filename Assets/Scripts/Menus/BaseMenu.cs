using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseMenu : MonoBehaviour
{
    public List<MenuButton> buttons;
    public int xCount, yCount;
    public int buttonIndex = 0;
    public MenuDirection menuDirection;
    public Image highlighImage;
    public virtual void Move(Vector2 direction){
        if (menuDirection == MenuDirection.Vertical){
            buttonIndex -= (int)direction.y;
        }else if (menuDirection == MenuDirection.Horizontal){
            buttonIndex += (int)direction.x;
        }
        else if (menuDirection == MenuDirection.Both){
            int currX = buttonIndex % xCount;
            int currY = buttonIndex / xCount;

            var newX = currX + direction.x;
            var newY = currY + -direction.y;

            if (newX >= xCount){
                newX = 0;
            }else if (newX < 0){
                newX = xCount - 1;
            }

            if (newY >= yCount){
                newY = 0;
            }else if (newY < 0){
                newY = yCount - 1;
            }

            buttonIndex = (int)newX + (int)newY*xCount;
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
    Horizontal,
    Both,
}
