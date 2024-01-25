using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitDot : MonoBehaviour
{
    public BaseUnit unit;
    [SerializeField]
    private Image image;
    public void SetColor(Color color){
        image.color = color;
    }
}


public enum UnitState {
    AwaitingOrders,
    Moved,
    Done
}
