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
    public bool sideMenu = false;
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
        }else if (menuDirection == MenuDirection.Flex){
            MenuButton currentButton = buttons[buttonIndex];
            MenuButton nextButton = null;
            if (direction.x < -0.5){
                nextButton = currentButton.leftButton;
            }else if (direction.x > 0.5){
                nextButton = currentButton.rightButton;
            }else if (direction.y > -0.5){
                nextButton = currentButton.upButton;
            }else if (direction.y < 0.5){
                nextButton = currentButton.downButton;
            }
            buttonIndex = buttons.IndexOf(nextButton);
        }
        if (!buttons[buttonIndex].IsOn()){
            Move(direction);
            return;
        }
        SetHighlight();
    }
    protected MenuButton GetCurrentButton(){
        if (buttonIndex >= buttons.Count || buttonIndex < 0){
            return null;
        }
        return buttons[buttonIndex];
    }
    public virtual void Reset(){
        buttonIndex = 0;
        SetHighlight();
    }
    protected void SetHighlight(){
        EnableHighlight(true);
        if (menuDirection == MenuDirection.Flex){
            buttons.ForEach(b => b.SetHighlight(false));
            //buttons.ForEach(b => b.bgimage.color = new Color(255, 255, 255));
//            Debug.Log(buttons[buttonIndex]);
            buttons[buttonIndex].SetHighlight(true);
            //buttons[buttonIndex].bgimage.color = c;
        }else{
            highlighImage.transform.position = GetCurrentButton().transform.position;
        }
    }
    public void EnableHighlight(bool enable=true){
        if (menuDirection == MenuDirection.Flex){
            buttons.ForEach(b => b.SetHighlight(false));
            //buttons.ForEach(b => b.bgimage.color = new Color(255, 255, 255));
//            Debug.Log(buttons[buttonIndex]);
            buttons[buttonIndex].SetHighlight(enable);
        }else{
            highlighImage.gameObject.SetActive(enable);
        }    
    }
    public virtual void Select(){

    }
    public void SetFirstIndex(){
        int index = 0;
        foreach (MenuButton menuButton in buttons){
            if (menuButton.IsOn()){
                buttonIndex = index;
                return;
            }
            index += 1;
        }
    }
}

public enum MenuDirection {
    Vertical,
    Horizontal,
    Both,
    Flex,
}
