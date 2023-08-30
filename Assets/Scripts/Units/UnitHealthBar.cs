using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class UnitHealthBar : MonoBehaviour
{
    BaseUnit attachedUnit;
    public Image image;
    public TextMeshProUGUI text;
    public Text basicText;

    [SerializeField]
    public Vector3 offset = new Vector3(0, -0.5f, 0);    
    private void Awake(){
        text = GetComponentInChildren<TextMeshProUGUI>();
    }
    public void SetAttachedUnit(BaseUnit attachedUnit){
        this.attachedUnit = attachedUnit;
    }
    private void Update(){
    }

    public void RenderHealth(){
        if (attachedUnit == null){
            return;
        }
        image.fillAmount = (float)attachedUnit.health / (float)attachedUnit.maxHealth;
        Vector3 viewportPosition = Camera.main.WorldToScreenPoint(attachedUnit.transform.position + offset);
        if (this.transform.position != viewportPosition){
            this.transform.position = viewportPosition;
        }
        text.SetText(attachedUnit.health + " / " + attachedUnit.maxHealth);
    }
}
