using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitDot : MonoBehaviour
{
    public BaseUnit unit;
    private Image image;
    public void Start(){
        image = GetComponent<Image>();
    }
    public void SetColor(Color color){
        image.color = color;
    }
}


public enum UnitState {
    AwaitingOrders,
    Moved,
    Done
}
